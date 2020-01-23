using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toIconCom.control;

namespace toIcon {
	public class Program {
		[STAThreadAttribute]
		public static void Main(string[] args) {
			if (args.Length > 0) {
				(new MainCtl()).runConsole(args);
				return;
			}

			App.Main();
		}
	}
}
