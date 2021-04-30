using csharpHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIconCom.model {
	[CmdRoot("ctoIcon.exe [-s][-d][-t][-o][-b][-m][-h]")]
	public class CmdMd {

		//[CmdSingle("eee", "eeeeee")]
		//public string eee = "";

		//[CmdSingle("fff", "ffffff")]
		//public string fff = "";

		//[CmdSingle("ggg", "gggggg")]
		//public List<string> ggg = null;

		//[CmdAttr("", "x", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcccccddddd")]
		//public bool aaa = true;

		//[CmdAttr("bbb", "", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcccccddddd中文中文")]
		//public bool bbb = true;

		//[CmdAttr("ccc", "y", "aaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbcc中文中文")]
		//public bool ccc = true;

		//[CmdAttr("ddd", "", "1111\r\naaaaabbbbbcccccdddddaaaaabbbbbcccccdddddaaaaabbbbbc中文中文")]
		//public bool ddd = true;

		[CmdAttr("src", "s", "multiple source file. file1 file2 ...")]
		public IList<string> srcPath = new List<string>();

		[CmdAttr("dst", "d", "out file directory, output to source directory if is empty")]
		public string dstDir = "";

		//[CmdAttr("filter", "f", "filter file if src is directry\r\ne.g. *.png|*.jpg")]
		//public string filter = "";

		[CmdAttr("type", "t", "out file type\r\noptional: auto,ico,bmp,jpg,png. default is auto:\r\nout ico if src is image\r\nout png if src is ico\r\nout png if src is orther file")]
		public string type = "auto";

		[CmdAttr("operate", "o", "operate when file exist\r\noptional:rename,jump,overwrite\r\ndefault is rename")]
		public string operate = "rename";

		[CmdAttr("bppSize", "b", "size and bpp, multiple out split by ';', bpp can be ignore\r\nusage: [size1],[bpp1];[size2]...\r\ne.g. 48,32;24,16;64\r\ndefault is 48")]
		public string bppSize = "";

		[CmdAttr("merge", "m", "merge output, Valid when the output format is ICO")]
		public bool merge = false;

		[CmdAttr("help", "h", "help")]
		public bool help = false;
	}
}
