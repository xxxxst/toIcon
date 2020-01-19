using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toIconCom.control;

namespace ctoIcon {
	class Program {
		static void Main(string[] args) {
			//args = new string[] { "-s", "ccc/down.png", "--dst=\"aaa/\"", "-t=auto", "-o", "jump", "-b", "24;32" };
			//args = new string[] { "-s", "toIcon.exe" };
			(new MainCtl()).runConsole(args);
		}
	}
}
