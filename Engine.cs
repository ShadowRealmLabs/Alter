using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

// Local, user-initiated access to a running process. No driver or code injection.
sealed class MemorySession : IDisposable
{
	const int ReadAccess = 0x0010, WriteAccess = 0x0020, OperationAccess = 0x0008, QueryAccess = 0x0400;
	IntPtr handle;
	public Process Process
	{
		get; private set;
	}
	public bool Connected
	{
		get
		{
			return handle != IntPtr.Zero && !Process.HasExited;
		}
	}
	[DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr OpenProcess(int access, bool inherit, int pid);
	[DllImport("kernel32.dll", SetLastError = true)] static extern bool CloseHandle(IntPtr h);
	[DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr h, IntPtr address, byte[] buffer, int size, out IntPtr read);
	[DllImport("kernel32.dll", SetLastError = true)] static extern bool WriteProcessMemory(IntPtr h, IntPtr address, byte[] buffer, int size, out IntPtr written);
	[DllImport("kernel32.dll", SetLastError = true)] static extern bool VirtualProtectEx(IntPtr h, IntPtr address, UIntPtr size, uint protection, out uint oldProtection);
	[DllImport("kernel32.dll", SetLastError = true)] static extern bool FlushInstructionCache(IntPtr h, IntPtr address, UIntPtr size);
	[DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr VirtualQueryEx(IntPtr h, IntPtr address, out MEMORY_BASIC_INFORMATION info, IntPtr length);
	[StructLayout(LayoutKind.Sequential)]
	struct MEMORY_BASIC_INFORMATION
	{
		public IntPtr BaseAddress, AllocationBase; public uint AllocationProtect; public UIntPtr RegionSize; public uint State, Protect, Type;
	}
	public MemorySession(string processName)
	{
		string name = Path.GetFileNameWithoutExtension(processName ?? "");
		if (String.IsNullOrWhiteSpace(name))
			throw new InvalidOperationException("Enter a process name first.");
		var found = Process.GetProcessesByName(name);
		if (found.Length == 0)
			throw new InvalidOperationException("Start " + processName + " before connecting.");
		if (found.Length > 1)
			throw new InvalidOperationException("More than one " + processName + " process is running. Close the extra instances before connecting.");
		Process = found[0];
		handle = OpenProcess(ReadAccess | WriteAccess | OperationAccess | QueryAccess, false, Process.Id);
		if (handle == IntPtr.Zero)
			throw new InvalidOperationException("Windows denied access to " + processName + " (error " + Marshal.GetLastWin32Error() + ").");
	}
	public void Dispose()
	{
		if (handle != IntPtr.Zero)
		{
			CloseHandle(handle);
			handle = IntPtr.Zero;
		}
	}
	public long Resolve(string expression)
	{
		if (String.IsNullOrWhiteSpace(expression))
			throw new FormatException("Enter an address or module+offset.");
		string s = expression.Trim();
		int plus = s.LastIndexOf('+');
		if (plus > 0)
		{
			string module = s.Substring(0, plus).Trim();
			long offset = ParseNumber(s.Substring(plus + 1));
			foreach (ProcessModule m in Process.Modules)
				if (String.Equals(m.ModuleName, module, StringComparison.OrdinalIgnoreCase))
					return checked(m.BaseAddress.ToInt64() + offset);
			throw new InvalidOperationException("Module not found: " + module);
		}
		return ParseNumber(s);
	}
	static long ParseNumber(string text)
	{
		string s = text.Trim();
		if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			return long.Parse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		return long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
	}
	public byte[] ReadBytes(long address, int count)
	{
		var b = new byte[count];
		IntPtr n;
		if (!ReadProcessMemory(handle, new IntPtr(address), b, count, out n) || n.ToInt64() != count)
			throw new IOException("Could not read address 0x" + address.ToString("X"));
		return b;
	}
	public void WriteBytes(long address, byte[] bytes)
	{
		IntPtr n;
		if (!WriteProcessMemory(handle, new IntPtr(address), bytes, bytes.Length, out n) || n.ToInt64() != bytes.Length)
			throw new IOException("Could not write address 0x" + address.ToString("X") + " (error " + Marshal.GetLastWin32Error() + ").");
	}
	public void WriteCodeBytes(long address, byte[] bytes)
	{
		if ((address & 0xFFF) + bytes.Length > 4096)
			throw new InvalidOperationException("A patch cannot cross a memory page.");
		uint old;
		if (!VirtualProtectEx(handle, new IntPtr(address), new UIntPtr((uint)bytes.Length), 0x40, out old))
			throw new IOException("Could not change page protection (error " + Marshal.GetLastWin32Error() + ").");
		try
		{
			WriteBytes(address, bytes);
			FlushInstructionCache(handle, new IntPtr(address), new UIntPtr((uint)bytes.Length));
		}
		finally { uint ignored; VirtualProtectEx(handle, new IntPtr(address), new UIntPtr((uint)bytes.Length), old, out ignored); }
	}
	public static byte[] Encode(string type, string value)
	{
		if (type == "Int32")
			return BitConverter.GetBytes(int.Parse(value, CultureInfo.InvariantCulture));
		if (type == "Float")
			return BitConverter.GetBytes(float.Parse(value, CultureInfo.InvariantCulture));
		if (type == "Double")
			return BitConverter.GetBytes(double.Parse(value, CultureInfo.InvariantCulture));
		throw new FormatException("Unsupported value type: " + type);
	}
	public string ReadValue(string address, string type)
	{
		long a = Resolve(address);
		byte[] b = ReadBytes(a, type == "Double" ? 8 : 4);
		if (type == "Int32")
			return BitConverter.ToInt32(b, 0).ToString(CultureInfo.InvariantCulture);
		if (type == "Float")
			return BitConverter.ToSingle(b, 0).ToString(CultureInfo.InvariantCulture);
		if (type == "Double")
			return BitConverter.ToDouble(b, 0).ToString(CultureInfo.InvariantCulture);
		throw new FormatException("Unsupported value type.");
	}
	public void WriteValue(string address, string type, string value)
	{
		WriteBytes(Resolve(address), Encode(type, value));
	}
	public List<long> FirstScan(string type, string value)
	{
		byte[] pattern = Encode(type, value);
		var hits = new List<long>();
		long cursor = 0;
		long scanned = 0;
		const long limit = 1024L * 1024 * 1024;
		const int maxHits = 500000;
		while (cursor < 0x7FFFFFFF0000L && scanned < limit && hits.Count < maxHits)
		{
			MEMORY_BASIC_INFORMATION info;
			IntPtr result = VirtualQueryEx(handle, new IntPtr(cursor), out info, new IntPtr(Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))));
			if (result == IntPtr.Zero)
				break;
			long start = info.BaseAddress.ToInt64();
			long size = (long)info.RegionSize.ToUInt64();
			long next = start + size;
			if (size <= 0 || next <= cursor)
				break;
			bool readable = info.State == 0x1000 && (info.Protect & 0x101) == 0 && (info.Protect & 0xFF) != 0;
			if (readable)
			{
				long end = Math.Min(next, start + Math.Min(size, limit - scanned));
				for (long at = start; at < end && hits.Count < maxHits; at += 65536)
				{
					int count = (int)Math.Min(65536, end - at);
					if (count < pattern.Length)
						break;
					byte[] block = new byte[count];
					IntPtr got;
					if (ReadProcessMemory(handle, new IntPtr(at), block, count, out got))
					{
						int length = (int)got.ToInt64();
						for (int i = 0; i <= length - pattern.Length; i += 4)
						{
							bool match = true;
							for (int j = 0; j < pattern.Length; j++)
								if (block[i + j] != pattern[j])
								{
									match = false;
									break;
								}
							if (match)
							{
								hits.Add(at + i);
								if (hits.Count >= maxHits)
									break;
							}
						}
					}
					scanned += count;
				}
			}
			cursor = next;
		}
		return hits;
	}
	public List<long> NextScan(List<long> prior, string type, string value)
	{
		byte[] pattern = Encode(type, value);
		var next = new List<long>();
		foreach (long address in prior)
		{
			try
			{
				byte[] found = ReadBytes(address, pattern.Length);
				bool match = true;
				for (int i = 0; i < pattern.Length; i++)
					if (found[i] != pattern[i])
					{
						match = false;
						break;
					}
				if (match)
					next.Add(address);
			}
			catch { }
		}
		return next;
	}
}
