using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csharpHelp {
	public class CmdAttrListMd {
		public Type itemType = null;
		public MethodInfo funAdd = null;
		public MethodInfo funClear = null;
		public bool isInited = false;
	}

	public class CmdAttrMd {
		public CmdAttr attr;
		public MemberInfo info;
		public bool isBoolType = false;
		public object defValue = null;

		public CmdAttrListMd listMd = null;

		public CmdAttrMd() { }
		public CmdAttrMd(CmdAttr _attr, MemberInfo _info) {
			attr = _attr;
			info = _info;
		}
	}

	public class CmdParser<T> where T:new() {
		string desc = "";
		T tMd = new T();
		Dictionary<string, CmdAttrMd> mapName = new Dictionary<string, CmdAttrMd>();
		List<CmdAttrMd> lstNoKeyMd = new List<CmdAttrMd>();

		List<CmdAttrMd> allKeyMd = new List<CmdAttrMd>();

		public CmdParser(string[] args) {
			init();
			parse(tMd, args);
		}

		public T getModel() {
			//T t = new T();
			//parse(t, args);
			return tMd;
		}

		private void init() {
			//Regex regListItem = new Regex(@".*?\[\[(.*?),");
			Type type = typeof(T);

			var arrFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var arrProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			MemberInfo[] arr = arrFields.Cast<MemberInfo>().Concat(arrProps).ToArray();
			
			if(type.IsDefined(typeof(CmdRoot), false)) {
				CmdRoot cmdRoot = type.GetCustomAttribute<CmdRoot>();
				desc = cmdRoot.desc;
			}

			foreach(var mi in arr) {
				if(!mi.IsDefined(typeof(CmdAttr), false)) {
					continue;
				}

				Type miType = getMemberType(mi);

				var attr = mi.GetCustomAttribute<CmdAttr>();
				CmdAttrMd md = new CmdAttrMd(attr, mi);
				
				if(miType.IsGenericType) {
					// type List<>
					try {
						md.listMd = new CmdAttrListMd();

						Type[] arrItemType = miType.GetGenericArguments();
						if(arrItemType.Length > 0) {
							md.listMd.itemType = arrItemType[0];
						} else {
						}
						object obj = getMemberValue(mi, tMd);
						if(obj == null) {
							md.listMd.funAdd = miType.GetMethod("Add", arrItemType);
							md.listMd.funClear = miType.GetMethod("Clear");
						} else {
							md.listMd.funAdd = obj.GetType().GetMethod("Add", arrItemType);
							md.listMd.funClear = obj.GetType().GetMethod("Clear");
						}
					} catch(Exception) { }
				} else {
					md.defValue = getMemberValue(mi, tMd);
				}

				allKeyMd.Add(md);

				if(attr.name == "" && attr.shortName == "") {
					lstNoKeyMd.Add(md);
					continue;
				}

				if(miType == typeof(bool)) {
					md.isBoolType = true;
				}

				mapName[attr.name] = md;
				mapName[attr.shortName] = md;
			}
		}

		private Type getMemberType(MemberInfo mi) {
			Type rst = null;
			if(mi.MemberType == MemberTypes.Field) {
				rst = (mi as FieldInfo).FieldType;
			} else if(mi.MemberType == MemberTypes.Property) {
				rst = (mi as PropertyInfo).PropertyType;
			} else {
				return null;
			}

			return rst;
		}

		private void setMemberValue(MemberInfo mi, object data, object value) {
			if(mi.MemberType == MemberTypes.Field) {
				(mi as FieldInfo).SetValue(data, value);
			} else if(mi.MemberType == MemberTypes.Property) {
				(mi as PropertyInfo).SetValue(data, value);
			}
		}

		private object getMemberValue(MemberInfo mi, object data) {
			object rst = null;
			if(mi.MemberType == MemberTypes.Field) {
				rst = (mi as FieldInfo).GetValue(data);
			} else if(mi.MemberType == MemberTypes.Property) {
				rst = (mi as PropertyInfo).GetValue(data);
			} else {
				return null;
			}

			return rst;
		}

		private void setData(object data, MemberInfo mi, string strValue) {
			Type type = getMemberType(mi);
			object val = getMemberValue(mi, data);
			if(val == null) {
				try {
					val = Activator.CreateInstance(type);
				} catch(Exception) { }
			}
			
			try {
				object rst = convertData(strValue, type, val);
				setMemberValue(mi, data, rst);
			} catch(Exception ex) {
				Debug.WriteLine(ex.ToString());
			}
		}

		private object convertData(string data, Type type, object defValue = null) {
			if(type.IsEnum) {
				try {
					return Enum.Parse(type, data);
				} catch(Exception) { }
				return defValue;
			}

			return Convert.ChangeType(data, type);
		}

		private void splitKeyValue(string keyVlaue, out string key, out string value) {
			int idx = keyVlaue.IndexOf('=');
			if(idx < 0) {
				key = keyVlaue;
				value = "";
				return;
			}

			key = keyVlaue.Substring(0, idx);
			value = keyVlaue.Substring(idx + 1);
		}

		private void parse(T t, string[] args) {
			CmdAttrMd lastMd = null;
			int noKeyMdIdx = 0;
			for(int i = 0; i < args.Length; ++i) {
				if(args[i] == "") {
					continue;
				}

				KeyValueType argType = getKeyValue(args[i], out string key, out string value);

				if(argType != KeyValueType.Value) {
					lastMd = findMd(key, ref noKeyMdIdx);
				}
				if(argType == KeyValueType.Value && lastMd == null) {
					lastMd = findMd(key, ref noKeyMdIdx);
				}

				if(lastMd == null) {
					continue;
				}

				if(argType == KeyValueType.Key && !lastMd.isBoolType) {
					continue;
				}

				setData(lastMd, t, value);

				if(lastMd.listMd == null) {
					lastMd = null;
				}
			}
		}

		enum KeyValueType {
			Key, Value, KeyValue
		}
		private KeyValueType getKeyValue(string arg, out string key, out string value) {
			string keyValue = "";
			key = "";
			value = "";

			if(arg.Substring(0, 2) == "--") {
				keyValue = arg.Substring(2);
			} else if(arg.Substring(0, 1) == "-") {
				keyValue = arg.Substring(1);
			}

			if(keyValue != "") {
				if(mapName.ContainsKey(keyValue)) {
					key = keyValue;
					return KeyValueType.Key;
				} else {
					splitKeyValue(keyValue, out key, out value);
					value = value.Trim('\"');
					return KeyValueType.KeyValue;
				}
			} else {
				value = arg;
				return KeyValueType.Value;
			}
		}

		private CmdAttrMd findMd(string key, ref int noKeyMdIdx) {
			CmdAttrMd md = null;
			if(mapName.ContainsKey(key)) {
				return mapName[key];
			}
			if(noKeyMdIdx >= lstNoKeyMd.Count) {
				return null;
			}

			md = lstNoKeyMd[noKeyMdIdx];
			++noKeyMdIdx;
			return md;
		}

		private void setData(CmdAttrMd md, T t, string value) {
			// list type
			if(md.listMd != null) {
				if(md.listMd.funAdd == null || md.listMd.itemType == null) {
					return;
				}

				try {
					//var lastArrayMd = md;

					var lastLst = getMemberValue(md.info, t);

					if(!md.listMd.isInited) {
						md.listMd.isInited = true;
						if(lastLst != null) {
							md.listMd.funClear.Invoke(lastLst, new object[] { });
						} else {
							lastLst = Activator.CreateInstance(getMemberType(md.info));
							setMemberValue(md.info, t, lastLst);
						}
					}

					object obj = convertData(value, md.listMd.itemType);
					//Debug.WriteLine("111:" + obj);
					md.listMd.funAdd.Invoke(lastLst, new object[] { obj });
				} catch(Exception ex) {
					Debug.WriteLine(ex.ToString());
				}
				return;
			}

			// bool type
			if(md.isBoolType) {
				setMemberValue(md.info, t, true);
				return;
			}

			// set value
			setData(t, md.info, value);
		}

		public string getHelp() {
			return getHelp((md) => md.attr.desc);
		}

		public string getHelp(Func<CmdAttrMd, string> funTranslateDesc) {
			const int maxAttrCount = 20;
			const int maxKeyCharCount = maxAttrCount - 2 - 2;
			const int maxColCount = 80;
			Regex regWChar = new Regex(@"[^\u0000-\u00ff]");
			Regex regChar = new Regex(@"[\u0000-\u00ff]");

			string strStart = "  ";
			string rst = "";
			string endl = "\r\n";

			if(desc != "") {
				rst += desc + endl + endl;
			}

			for(int i = 0; i < allKeyMd.Count; ++i) {
				var attr = allKeyMd[i].attr;
				string key = "";
				if(attr.shortName != "") {
					key += "-" + attr.shortName;
				}
				if(attr.name != "") {
					if(key != "") {
						key += ",";
					} else {
						key = "   ";
					}
					key += "--" + attr.name;
				}

				if(attr.type == CmdAttrType.Single) {
					key = attr.singleName;
				}
				
				if(key.Length >= maxKeyCharCount) {
					rst += strStart + key + endl + fillSpace(maxAttrCount);
				} else {
					rst += strStart + key + fillSpace(maxAttrCount - key.Length - strStart.Length);
				}

				string desc = funTranslateDesc(allKeyMd[i]);
				string[] arr = desc.Replace("\r\n", "\n").Split('\n');
				for(int j = 0; j < arr.Length; ++j) {
					string str = arr[j];
					string tmp = regWChar.Replace(str, "");
					int charCount = tmp.Length;
					int wcharCount = str.Length - tmp.Length;

					tmp = regChar.Replace(str, "1");
					tmp = regWChar.Replace(tmp, "2");
					int count = wcharCount * 2 + charCount;
					int maxValueCount = maxColCount - maxAttrCount;
					if(j != 0) {
						rst += fillSpace(maxAttrCount) + "  ";
						maxValueCount -= 2;
					}
					string line = "";
					int lineSingleCharLen = 0;
					for(int k = 0; k < str.Length; ++k) {
						if(tmp[k] == '1') {
							line += str[k];
							++lineSingleCharLen;
						} else {
							if(lineSingleCharLen + 2 > maxValueCount) {
								rst += line + endl + fillSpace(maxAttrCount);
								line = "";
								lineSingleCharLen = 0;
							}
							line += str[k];
							lineSingleCharLen += 2;
						}
						if(lineSingleCharLen >= maxValueCount) {
							rst += line + endl;
							if(k != str.Length - 1) {
								rst += fillSpace(maxAttrCount) + strStart;
							}
							line = "";
							lineSingleCharLen = 0;
						}
					}
					if(line != "") {
						rst += line + endl;
					}
				}

			}
			return rst;
		}

		private string fillSpace(int count) {
			string rst = "";
			for(int i = 0; i < count; ++i) {
				rst += " ";
			}
			return rst;
		}

		public void ergCmdAttr(Action<CmdAttrMd> funErgAttr) {
			for(int i = 0; i < allKeyMd.Count; ++i) {
				funErgAttr(allKeyMd[i]);
			}
		}
	}

	public enum CmdAttrType {
		Attr, Single,
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CmdAttr : Attribute {
		//public bool isArray = false;
		public CmdAttrType type = CmdAttrType.Attr;
		public string name = "";
		public string shortName = "";
		public string desc = "";
		public string singleName = "";

		public CmdAttr(string _name, string _shortName = "", string _desc = "") {
			name = _name;
			shortName = _shortName;
			desc = _desc;
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CmdSingle : CmdAttr {
		public CmdSingle(string _name, string _desc = "") : base("", "", _desc) {
			singleName = _name;
			type = CmdAttrType.Single;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class CmdRoot : Attribute {
		public string desc = "";

		public CmdRoot(string _desc = "") {
			desc = _desc;
		}
	}
}
