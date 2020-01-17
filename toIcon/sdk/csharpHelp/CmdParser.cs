using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csharpHelp {
	public class CmdAttrMd {
		public CmdAttr attr;
		public MemberInfo info;
		public bool isBoolType = false;
		public object defValue = null;
		public Type listItemType = null;
		public bool isListType = false;
		public MethodInfo funListAdd = null;
		public MethodInfo funListClear = null;

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

				//Debug.WriteLine("111:" + miType.IsGenericType + "," + miType);
				if(miType.IsGenericType) {
					// type List<>
					try {
						md.isListType = true;
						Type[] arrItemType = miType.GetGenericArguments();
						if(arrItemType.Length > 0) {
							md.listItemType = arrItemType[0];
						} else {
							//md.listItemType = typeof(string);
						}
						object obj = getMemberValue(mi, tMd);
						if(obj == null) {
							md.funListAdd = miType.GetMethod("Add", arrItemType);
							md.funListClear = miType.GetMethod("Clear");
						} else {
							md.funListAdd = obj.GetType().GetMethod("Add", arrItemType);
							md.funListClear = obj.GetType().GetMethod("Clear");
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
			//string str = "";
			//string lastArrayKey = "";
			CmdAttrMd lastArrayMd = null;
			object lastLst = null;

			string lastKey = "";
			int noKeyMdIdx = 0;
			for(int i = 0; i < args.Length; ++i) {
				if(args[i] == "") {
					continue;
				}

				string keyValue = "";
				string key = "";
				string value = "";
				//remove "--","-"
				if(args[i].Substring(0, 2) == "--") {
					keyValue = args[i].Substring(2);
				} else if(args[i].Substring(0, 1) == "-") {
					keyValue = args[i].Substring(1);
				}

				CmdAttrMd md = null;

				// split key & value
				if(keyValue != "") {
					if(mapName.ContainsKey(keyValue)) {
						key = keyValue;
					} else {
						splitKeyValue(keyValue, out key, out value);
						value = value.Trim('\"');
					}

					// set value to last key
					if(lastKey != "") {
						if(mapName.ContainsKey(lastKey)) {
							md = mapName[lastKey];
							if(md.isBoolType) {
								setMemberValue(md.info, t, true);
							} else {
								setMemberValue(md.info, t, "");
							}
						}
					}

					lastKey = key;
					lastArrayMd = null;
					lastLst = null;
					if(keyValue == key) {
						continue;
					}
				} else {
					value = args[i];
				}

				if(lastKey == "") {
					if(lastArrayMd != null) {
						try {
							object obj = convertData(value, lastArrayMd.listItemType);
							//Debug.WriteLine("222:" + obj);
							lastArrayMd.funListAdd.Invoke(lastLst, new object[] { obj });
						} catch(Exception) { }
						continue;
					}

					if(noKeyMdIdx >= lstNoKeyMd.Count) {
						continue;
					}
					md = lstNoKeyMd[noKeyMdIdx];
					++noKeyMdIdx;
				} else {
					if(mapName.ContainsKey(lastKey)) {
						md = mapName[lastKey];
					}
				}
				lastKey = "";

				if(md == null) {
					continue;
				}

				if(md.funListAdd != null && md.listItemType != null) {
					try {
						lastArrayMd = md;

						lastLst = getMemberValue(md.info, t);
						if(lastLst != null) {
							md.funListClear.Invoke(lastLst, new object[] { });
						} else {
							lastLst = Activator.CreateInstance(getMemberType(md.info));
						}

						setMemberValue(md.info, t, lastLst);
						object obj = convertData(value, md.listItemType);
						//Debug.WriteLine("111:" + obj);
						lastArrayMd.funListAdd.Invoke(lastLst, new object[] { obj });
					} catch(Exception ex) {
						Debug.WriteLine(ex.ToString());
						lastArrayMd = null;
						lastLst = null;
					}
					continue;
				}
				if(md.isListType) {
					continue;
				}

				if(md.isBoolType) {
					setMemberValue(md.info, t, true);
					continue;
				}

				setData(t, md.info, value);
			}

			if(lastKey != "") {
				if(mapName.ContainsKey(lastKey)) {
					CmdAttrMd md = null;
					md = mapName[lastKey];
					if(md.isBoolType) {
						setMemberValue(md.info, t, true);
					} else {
						setMemberValue(md.info, t, "");
					}
				}
			}

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

	//[AttributeUsage(AttributeTargets.Class)]
	//public class CmdArray : CmdAttr {
	//	public CmdArray(string _name, string _shortName = "", string _desc = "") : base(_name, _shortName, _desc) {
	//		isArray = true;
	//	}
	//}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CmdSingle : CmdAttr {
		public CmdSingle(string _name, string _desc = "") : base("", "", _desc) {
			singleName = _name;
			type = CmdAttrType.Single;
		}
	}

	//[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	//public class CmdSingleArray : CmdAttr {
	//	public CmdSingleArray(string _name, string _desc = "") : base("", "", _desc) {
	//		singleName = _name;
	//		type = CmdAttrType.Single;
	//	}
	//}

	[AttributeUsage(AttributeTargets.Class)]
	public class CmdRoot : Attribute {
		public string desc = "";

		public CmdRoot(string _desc = "") {
			desc = _desc;
		}
	}
}
