using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csharpHelp.util {
	public class IniSection : Dictionary<string, string> {
		public new string this[string key] {
			get {
				if(!ContainsKey(key)) {
					return "";
				}
				string val = base[key];
				return (val == null) ? "" : val;
			}
			set {
				base[key] = value;
			}
		}

		public int getInt(string key, int def = 0) {
			string val = this[key];
			bool isOk = int.TryParse(val, out int rst);
			return isOk ? rst : 0;
		}

		public void setInt(string key, int val) {
			this[key] = val.ToString();
		}

		public bool getBool(string key, bool def = false) {
			string val = this[key];
			if(val == null || val == "") { return def; }
			return val.ToLower()[0] == 't';
		}

		public void setBool(string key, bool val) {
			this[key] = val.ToString();
		}
	}

	public class IniParse {
		string path = "";
		Encoding encoding = null;

		Dictionary<string, IniSection> mapData = new Dictionary<string, IniSection>();

		Dictionary<string, IniSection> mapPreData = new Dictionary<string, IniSection>();

		public IniParse(string _path = "", Encoding _encoding = null) {
			path = _path;
			encoding = _encoding;

			if(path != "") {
				format(path, encoding);
			}
		}

		public IniSection this[string key] {
			get {
				if(!mapData.ContainsKey(key)) {
					var rst = new IniSection();
					mapData[key] = rst;
					return rst;
				}
				return mapData[key];
			}
		}

		public IniSection global {
			get {
				if(!mapData.ContainsKey("")) {
					var rst = new IniSection();
					mapData[""] = rst;
					return rst;
				}
				return mapData[""];
			}
		}

		public void format(string _path, Encoding encoding = null) {
			path = _path;

			if(!File.Exists(_path)) {
				return;
			}

			using(FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				format(fs, encoding);
			}
		}

		public void format(Stream s, Encoding encoding = null) {
			mapData = new Dictionary<string, IniSection>();

			string data = readStream(s, encoding);
			string[] arr = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

			Regex regNote = new Regex(@"^[\s]*[#;]");
			Regex regSection = new Regex("^[\\s]*\\[(.*?)\\][\\s#;]?$");
			Regex regKeyValue = new Regex("^[\\s]*(.*?)[\\s]*=[\\s]*\"([^\"]*)\"[\\s#;]?");
			Regex regKeyValue2 = new Regex("^[\\s]*(.*?)[\\s]*=[\\s]*([^\\s#;]*)[\\s#;]?");

			string lastSection = "";
			IniSection mapSection = new IniSection();

			IniSection mapPreDataSection = new IniSection();
			string preData = "";

			for(int i = 0; i < arr.Length; ++i) {
				if(arr[i] == "") {
					preData += arr[i] + "\r\n";
					continue;
				}

				//note
				if(regNote.IsMatch(arr[i])) {
					preData += arr[i] + "\r\n";
					continue;
				}

				//section
				Match mat = regSection.Match(arr[i]);
				if(mat.Success) {
					if(mat.Groups.Count >= 2) {
						mapData[lastSection] = mapSection;

						mapSection = new IniSection();

						if(preData != "") {
							mapPreDataSection[""] = preData;
							mapPreData[lastSection] = mapPreDataSection;
							preData = "";
						}
						mapPreDataSection = new IniSection();

						lastSection = mat.Groups[1].Value;
					}
					continue;
				}

				//key="value"
				mat = regKeyValue.Match(arr[i]);

				//key=value
				if(!mat.Success) {
					mat = regKeyValue2.Match(arr[i]);
				}

				//failed
				if(!mat.Success || mat.Groups.Count < 3) {
					preData += arr[i] + "\r\n";
					continue;
				}

				string key = mat.Groups[1].Value;
				string val = mat.Groups[2].Value;
				mapSection[key] = val;

				if(preData != "") {
					mapPreDataSection[key] = preData;
					preData = "";
				}
				//Debug.WriteLine(mat.Groups.Count + "," + lastSection + "," + mat.Groups[1].Value + "," + mat.Groups[2].Value);
			}

			mapData[lastSection] = mapSection;

			if(preData != "") {
				mapPreDataSection[""] = preData;
				mapPreData[lastSection] = mapPreDataSection;
			}
		}

		public void save() {
			using(FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				save(fs, encoding);
			}
		}

		public void save(Stream s, Encoding encoding = null) {
			string rst = "";
			string tmpSectionData = "";
			foreach(string secName in mapData.Keys) {
				if(mapData[secName].Count > 0 && mapPreData.ContainsKey(secName)) {
					if(mapPreData[secName].ContainsKey("")) {
						string preData = mapPreData[secName][""];
						rst += preData;
					}
				}

				if(rst.Length >= 4 && rst.Substring(rst.Length - 4) != "\r\n\r\n") {
					rst += "\r\n";
				}

				if(secName == "") {
					tmpSectionData = rst;
					rst = "";
				} else {
					rst += "[" + secName + "]\r\n";
				}
				foreach(string key in mapData[secName].Keys) {
					string val = mapData[secName][key];
					if(mapPreData.ContainsKey(secName)) {
						if(mapPreData[secName].ContainsKey(key)) {
							string preData = mapPreData[secName][key];
							rst += preData;
						}
					}

					rst += key + "=" + val + "\r\n";
				}
				if(secName == "") {
					rst = rst + tmpSectionData;
				}
			}

			if(encoding == null) {
				encoding = Encoding.Default;
			}

			//Debug.WriteLine("aaa:" + rst);

			using(StreamWriter sr = new StreamWriter(s)) {
				sr.Write(rst);
			}
		}

		private string readStream(Stream s, Encoding encoding = null) {
			if(encoding == null) {
				encoding = Encoding.Default;
			}

			byte[] data = new byte[s.Length];
			s.Read(data, 0, (int)s.Length);

			return encoding.GetString(data);
		}

	}
}
