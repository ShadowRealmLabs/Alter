using System;
using System.Linq;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

// The local creator's programmable building blocks. Game-specific addresses and logic
// are supplied by the trainer author; this API does not discover cheats automatically.
public sealed class TrainerApi
{
	readonly MemorySession memory;
	readonly Dictionary<long, byte[]> patches = new Dictionary<long, byte[]>();
	static readonly Dictionary<string, float[]> positions = new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
	internal TrainerApi(MemorySession session)
	{
		memory = session;
	}
	public long Resolve(string address)
	{
		return memory.Resolve(address);
	}
	public long Pointer(string baseAddress, params long[] offsets)
	{
		long address = Resolve(baseAddress);
		foreach (long offset in offsets)
			address = checked(BitConverter.ToInt64(memory.ReadBytes(address, 8), 0) + offset);
		return address;
	}
	public byte[] ReadBytes(long address, int count)
	{
		if (count < 1 || count > 4096)
			throw new ArgumentOutOfRangeException("count");
		return memory.ReadBytes(address, count);
	}
	public byte[] ReadBytes(string address, int count)
	{
		return ReadBytes(Resolve(address), count);
	}
	public void WriteBytes(long address, byte[] bytes)
	{
		if (bytes == null || bytes.Length < 1 || bytes.Length > 4096)
			throw new ArgumentOutOfRangeException("bytes");
		memory.WriteBytes(address, bytes);
	}
	public void WriteBytes(string address, byte[] bytes)
	{
		WriteBytes(Resolve(address), bytes);
	}
	public int ReadInt32(string address)
	{
		return BitConverter.ToInt32(ReadBytes(address, 4), 0);
	}
	public float ReadFloat(string address)
	{
		return BitConverter.ToSingle(ReadBytes(address, 4), 0);
	}
	public double ReadDouble(string address)
	{
		return BitConverter.ToDouble(ReadBytes(address, 8), 0);
	}
	public void WriteInt32(string address, int value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void WriteFloat(string address, float value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void WriteDouble(string address, double value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void WriteInt32(long address, int value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void WriteFloat(long address, float value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void WriteDouble(long address, double value)
	{
		WriteBytes(address, BitConverter.GetBytes(value));
	}
	public void Patch(string address, string hexBytes)
	{
		Patch(Resolve(address), hexBytes);
	}
	public void Patch(long address, string hexBytes)
	{
		byte[] replacement = ParseHex(hexBytes);
		if (!patches.ContainsKey(address))
			patches.Add(address, memory.ReadBytes(address, replacement.Length));
		memory.WriteCodeBytes(address, replacement);
	}
	public void RestorePatches()
	{
		foreach (var patch in patches)
			try
			{
				memory.WriteCodeBytes(patch.Key, patch.Value);
			}
			catch { }
		patches.Clear();
	}
	string PositionKey(string slot)
	{
		return memory.Process.Id + "|" + slot;
	}
	public void SavePosition(string slot, string x, string y, string z)
	{
		positions[PositionKey(slot)] = new[] { ReadFloat(x), ReadFloat(y), ReadFloat(z) };
	}
	public void LoadPosition(string slot, string x, string y, string z)
	{
		float[] p;
		if (!positions.TryGetValue(PositionKey(slot), out p))
			throw new InvalidOperationException("No saved position in slot " + slot);
		WriteFloat(x, p[0]);
		WriteFloat(y, p[1]);
		WriteFloat(z, p[2]);
	}
	public long FindPattern(string moduleName, string pattern)
	{
		var tokens = pattern.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length < 2 || tokens.Length > 256)
			throw new FormatException("Pattern needs 2–256 bytes.");
		var bytes = new byte[tokens.Length];
		var mask = new bool[tokens.Length];
		for (int i = 0; i < tokens.Length; i++)
		{
			if (tokens[i] == "?" || tokens[i] == "??")
				continue;
			bytes[i] = Convert.ToByte(tokens[i], 16);
			mask[i] = true;
		}
		System.Diagnostics.ProcessModule module = null;
		foreach (System.Diagnostics.ProcessModule m in memory.Process.Modules)
			if (String.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
			{
				module = m;
				break;
			}
		if (module == null)
			throw new InvalidOperationException("Module not found: " + moduleName);
		long start = module.BaseAddress.ToInt64(), size = Math.Min((long)module.ModuleMemorySize, 256L * 1024 * 1024);
		const int chunk = 65536;
		for (long offset = 0; offset < size; offset += chunk)
		{
			int count = (int)Math.Min(chunk + bytes.Length - 1, size - offset);
			byte[] block;
			try
			{
				block = memory.ReadBytes(start + offset, count);
			}
			catch { continue; }
			int end = (int)Math.Min(chunk, block.Length - bytes.Length + 1);
			for (int at = 0; at < end; at++)
			{
				bool hit = true;
				for (int i = 0; i < bytes.Length; i++)
					if (mask[i] && block[at + i] != bytes[i])
					{
						hit = false;
						break;
					}
				if (hit)
					return start + offset + at;
			}
		}
		throw new InvalidOperationException("Pattern was not found in " + moduleName);
	}
	static byte[] ParseHex(string input)
	{
		var parts = input.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length < 1 || parts.Length > 4096)
			throw new FormatException("Enter 1–4096 hex bytes.");
		return parts.Select(x => Convert.ToByte(x, 16)).ToArray();
	}
}

sealed class ScriptRuntime
{
	readonly TrainerApi api;
	readonly Type program;
	public ScriptRuntime(MemorySession session, Feature feature)
	{
		api = new TrainerApi(session);
		program = Compile(feature);
	}
	public static void Validate(Feature feature)
	{
		Compile(feature);
	}
	static Type Compile(Feature feature)
	{
		string code = "using System; public static class AlterFeatureProgram { public static void On(TrainerApi api,double value) {\n" + (feature.scriptOn ?? "") + "\n} public static void Off(TrainerApi api,double value) {\n" + (feature.scriptOff ?? "") + "\n} public static void Tick(TrainerApi api,double value) {\n" + (feature.scriptTick ?? "") + "\n} }";
		using (var provider = new CSharpCodeProvider())
		{
			var options = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true, TreatWarningsAsErrors = false };
			options.ReferencedAssemblies.Add("System.dll");
			options.ReferencedAssemblies.Add(typeof(TrainerApi).Assembly.Location);
			var result = provider.CompileAssemblyFromSource(options, code);
			if (result.Errors.HasErrors)
				throw new FormatException(String.Join("\n", result.Errors.Cast<CompilerError>().Where(e => !e.IsWarning).Take(6).Select(e => "Line " + e.Line + ": " + e.ErrorText)));
			return result.CompiledAssembly.GetType("AlterFeatureProgram", true);
		}
	}
	public void On(double value)
	{
		Invoke("On", value);
	}
	public void Off(double value)
	{
		try
		{
			Invoke("Off", value);
		}
		finally { api.RestorePatches(); }
	}
	public void Tick(double value)
	{
		Invoke("Tick", value);
	}
	void Invoke(string method, double value)
	{
		try
		{
			program.GetMethod(method).Invoke(null, new object[] { api, value });
		}
		catch (TargetInvocationException e) { throw e.InnerException ?? e; }
	}
}
