using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// Reads the option text embedded in some FLiNG executables. It never runs the EXE.
sealed class FlingDetectedOption {
 public string Hotkey,Name,Category;
}
static class FlingOptionExtractor {
 static readonly Regex OptionLine=new Regex(@"^\s*(?<key>(?:(?:Ctrl|Alt|Shift)\+)*(?:Num\s*(?:[0-9+*/.\-])|F(?:[1-9]|1[0-9]|2[0-4])|PageUp|PageDown|Home|End|Insert|Delete|[A-Z]))\s+-\s+(?<name>.+)$",RegexOptions.IgnoreCase|RegexOptions.Compiled);
 public static List<FlingDetectedOption> Extract(string path){
  var info=new FileInfo(path);if(info.Length>50*1024*1024)throw new InvalidOperationException("Trainer EXE exceeds the 50 MB import limit.");
  byte[] bytes=File.ReadAllBytes(path);var strings=UnicodeStrings(bytes).Concat(AsciiStrings(bytes)).ToList();
  if(!strings.Any(s=>s.IndexOf("FLiNG",StringComparison.OrdinalIgnoreCase)>=0||s.IndexOf("FLiNGTrainer",StringComparison.OrdinalIgnoreCase)>=0))return new List<FlingDetectedOption>();
  var result=new List<FlingDetectedOption>();var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
  foreach(string s in strings){var match=OptionLine.Match(s);if(!match.Success)continue;string key=Regex.Replace(match.Groups["key"].Value.Trim(),@"\s+"," ");string name=match.Groups["name"].Value.Trim();int note=name.IndexOf(" **",StringComparison.Ordinal);if(note>=0)name=name.Substring(0,note).Trim();int newline=name.IndexOf("\\n",StringComparison.Ordinal);if(newline>=0)name=name.Substring(0,newline).Trim();if(name.Length<3||name.Length>90||name[0]=='<'||name.IndexOf('<')>=0)continue;
   if(seen.Add(key+"|"+name))result.Add(new FlingDetectedOption{Hotkey=key,Name=name,Category=Category(name)});
  }
  return result.Count>=2?result:new List<FlingDetectedOption>();
 }
 static IEnumerable<string> UnicodeStrings(byte[] data){for(int parity=0;parity<2;parity++){var text=new StringBuilder();for(int i=parity;i+1<data.Length;i+=2){int c=data[i],high=data[i+1];if(high==0&&c>=32&&c<=126){if(text.Length<4096)text.Append((char)c);}else{if(text.Length>=8)yield return text.ToString();text.Clear();}}if(text.Length>=8)yield return text.ToString();}}
 static IEnumerable<string> AsciiStrings(byte[] data){var text=new StringBuilder();foreach(byte c in data){if(c>=32&&c<=126){if(text.Length<4096)text.Append((char)c);}else{if(text.Length>=8)yield return text.ToString();text.Clear();}}if(text.Length>=8)yield return text.ToString();}
 static string Category(string name){string n=name.ToLowerInvariant();if(n.Contains("ammo")||n.Contains("item")||n.Contains("component")||n.Contains("money")||n.Contains("weight")||n.Contains("equipment"))return "Inventory";if(n.Contains("health")||n.Contains("stamina")||n.Contains("ram")||n.Contains("player")||n.Contains("movement")||n.Contains("jump")||n.Contains("speed"))return "Player";if(n.Contains("damage")||n.Contains("kill")||n.Contains("reload")||n.Contains("recoil")||n.Contains("accuracy")||n.Contains("stealth"))return "Combat";if(n.Contains("xp")||n.Contains("level")||n.Contains("skill")||n.Contains("perk")||n.Contains("cred")||n.Contains("attribute"))return "Progression";if(n.Contains("time")||n.Contains("weather")||n.Contains("gravity")||n.Contains("fov")||n.Contains("location")||n.Contains("teleport")||n.Contains("fly"))return "World";return "Other";}
}
