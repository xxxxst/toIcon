using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIcon.util {
	public class SysConst {
		public static string rootPath() {
			return AppDomain.CurrentDomain.BaseDirectory;
		}

		public static string configPath() {
			return rootPath() + "/config.ini";
		}
	}
}
