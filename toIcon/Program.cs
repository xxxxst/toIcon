using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toIcon.control;

namespace toIcon {
	public class Program {
		[STAThread]
		public static void Main(string[] args) {
			args = new string[] { "-s", "aa.txt", "bb.txt", "cc.txt", "--dst=\"aaa/\"", "-t=auto", "-o", "jump", "-b", "32,1;16", "-h" };
			if(args.Length > 0) {
				(new MainCtl()).runConsole(args);
				return;
			}

			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
