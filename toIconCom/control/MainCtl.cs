using csharpHelp;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using toIconCom.model;

namespace toIconCom.control {
	public class MainCtl {
		//[DllImport("Kernel32.dll")]
		//public static extern bool AttachConsole(int processId);
		//[DllImport("Kernel32.dll")]
		//public static extern bool AllocConsole();

		//[DllImport("kernel32.dll")]
		//private static extern bool FreeConsole();

		//[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		//private static extern IntPtr GetStdHandle(int nStdHandle);

		public void runConsole(string[] args) {
			//AllocConsole();
			//AttachConsole(-1);

			//const int STD_OUTPUT_HANDLE = -11;
			//var stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
			//var safeFileHandle = new SafeFileHandle(stdHandle, true);
			//var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
			//var standardOutput = new StreamWriter(fileStream) { AutoFlush = true };
			//Console.SetOut(standardOutput);

			CmdParser<CmdMd> parser = new CmdParser<CmdMd>(args);
			var md = parser.getModel();
			//Debug.WriteLine("    aaa:" + string.Join(" ", args));
			//Debug.WriteLine("srcPath:" + string.Join(",", md.srcPath));
			//Debug.WriteLine(" dstDir:" + md.dstDir);
			//Debug.WriteLine("   type:" + md.type);
			//Debug.WriteLine("operate:" + md.operate);
			//Debug.WriteLine("bppSize:" + md.bppSize);
			//Debug.WriteLine("   help:" + md.help);
			//Debug.WriteLine("    eee:" + md.eee);
			//Debug.WriteLine("    fff:" + md.fff);
			//Debug.WriteLine("    ggg:" + string.Join(",", md.ggg));
			//Debug.WriteLine("");

			//string str = "";
			//for(int i = 0; i < md.srcPath.Count; ++i) {
			//	str += md.srcPath[i] + ",";
			//}
			//Debug.WriteLine("aaa:" + str);

			if(md.help || args.Length == 0) {
				Console.WriteLine(getHelp(parser));
				return;
			}

			try {
				(new IconCtl()).convert(md.srcPath.ToArray(), md.dstDir, md.bppSize, md.type, md.operate, md.merge);
			} catch(Exception ex) {
				Console.WriteLine("Failed");
				Console.WriteLine(ex.ToString());
			}
			
			//string help = parser.getHelp();
			//string help = parser.getHelp(it=> {
			//	switch(it.attr.name) {
			//		case "src": return "源文件或目录路径。file1 file2 ...";
			//		case "dst": return "输出目录，如果为空则输出到源文件目录";
			//		case "type": return "输出文件格式\r\n可选项：auto,ico,bmp,jpg,png";
			//		case "operate": return "输出文件存在时的操作\r\n可选项：rename,jump,overwrite\r\nrename 为默认值";
			//		case "bppSize": return "bpp和输出文件尺寸，多个输出用';'分割\r\n[size1],[bpp1?];[size2]...\r\n例： 48,32;24,16;64";
			//		default: return it.attr.desc;
			//	}
			//});
			//Debug.WriteLine("--------------------------------------------------------------------------------");
			//Debug.WriteLine(help);
		}

		private string getHelp(CmdParser<CmdMd> parser) {
			string help = parser.getHelp(it => {
				switch(it.attr.name) {
					case "src": return Lang.ins.langHelpSrc;
					case "dst": return Lang.ins.langHelpDst;
					case "type": return Lang.ins.langHelpType;
					case "operate": return Lang.ins.langHelpOperate;
					case "bppSize": return Lang.ins.langHelpBppSize;
					case "merge": return Lang.ins.langMergeOutput;
					case "help": return Lang.ins.langHelpHelp;
					default: return it.attr.desc;
				}
			});

			return help;
		}
	}
}
