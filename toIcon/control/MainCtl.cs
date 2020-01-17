using csharpHelp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toIcon.model;

namespace toIcon.control {
	public class MainCtl {
		public void runConsole(string[] args) {
			CmdParser<CmdMd> parser = new CmdParser<CmdMd>(args);
			var md = parser.getModel();
			Debug.WriteLine("aaa:" + md.srcPath);
			Debug.WriteLine("aaa:" + md.dstDir);
			Debug.WriteLine("aaa:" + md.type);
			Debug.WriteLine("aaa:" + md.operate);
			Debug.WriteLine("aaa:" + md.bppSize);
			Debug.WriteLine("aaa:" + md.help);

			//string str = "";
			//for(int i = 0; i < md.srcPath.Count; ++i) {
			//	str += md.srcPath[i] + ",";
			//}
			//Debug.WriteLine("aaa:" + str);

			if(md.help) {
				Console.WriteLine(parser.getHelp());
				return;
			}

			string help = parser.getHelp();
			//string help = parser.getHelp(it=> {
			//	switch(it.attr.name) {
			//		case "src": return "源文件路径。file1 file2 ...";
			//		case "dst": return "输出目录";
			//		case "type": return "输出文件格式\r\n可选项：auto,ico,bmp,jpg,png";
			//		case "operate": return "输出文件存在时的操作\r\n可选项：rename,jump,overwrite\r\nrename 为默认值";
			//		case "bppSize": return "bpp和输出文件尺寸，多个输出用';'分割\r\n[size1],[bpp1?];[size2]...\r\n例： 48,32;24,16;64";
			//		default: return it.attr.desc;
			//	}
			//});
			//Debug.WriteLine("--------------------------------------------------------------------------------");
			//Debug.WriteLine(help);
		}
	}
}
