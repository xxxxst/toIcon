using csharpHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIcon.model {
	[CmdRoot("toIcon [-s][-d][-t][-o][-b][-h]")]
	public class CmdMd {

		//[CmdSingle("eee", "eeeeee")]
		//public string eee = "";

		//[CmdSingle("fff", "ffffff")]
		//public string fff = "";

		//[CmdAttr("", "x", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcccccddddd")]
		//public bool aaa = true;

		//[CmdAttr("bbb", "", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcccccddddd中文中文")]
		//public bool bbb = true;

		//[CmdAttr("ccc", "y", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcc中文中文")]
		//public bool ccc = true;

		//[CmdAttr("ddd", "", "1111\r\naaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbc中文中文")]
		//public bool ddd = true;

		[CmdAttr("src", "s", "source file path. file1 file2 ...")]
		public IList<string> srcPath = new List<string>();

		//[CmdAttr("src", "s", "source file path")]
		//public string srcPath = "";

		[CmdAttr("dst", "d", "dist file directory")]
		public string dstDir = "";

		[CmdAttr("type", "t", "out file type\r\noptional:auto,ico,bmp,jpg,png")]
		public string type = "auto";

		[CmdAttr("operate", "o", "operate when file exist\r\noptional:rename,jump,overwrite\r\nrename is default")]
		public string operate = "rename";

		[CmdAttr("bppSize", "b", "bpp and size, multiple output split by ';'\r\n[size1],[bpp1];[size2]...\r\ne.g. 48,32;24,16;64")]
		public string bppSize = "";

		[CmdAttr("help", "h", "help")]
		public bool help = false;
	}
}
