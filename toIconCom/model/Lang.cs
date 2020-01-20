using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIconCom.model {
	public class Lang {
		public static readonly Lang ins = new Lang();

		public Lang() {
			//if(IsSimpleChinese()) {
			//	return;
			//}

			langSetting = "Setting";
			langBpp = " Bpp";
			langExportMulti = "Export Multi";
			langDragImageHere = "Drag Image Here";
			langOk = "Ok";

			langStrPopWin1 = "File<";
			langStrPopWin2 = "> exist，sure to replace？";

			langReplace = "Replace";
			langReplaceAll = "Replace All";
			langJump = "Jump";
			langCancel = "Cancel";
			langRename = "Rename";
			langWhenFileExist = "When File Exist:";
			langOutType = "Out Type";
			langAuto = "Auto";

			langHelpSrc = "multiple source file. file1 file2 ...";
			langHelpDst = "out file directory, output to source directory if is empty";
			langHelpType = "out file type\r\noptional: auto,ico,bmp,jpg,png. default is auto:\r\nout png if src is ico\r\nout ico if src is image\r\nout png if src is orther file";
			langHelpOperate = "operate when file exist\r\noptional: rename,jump,overwrite\r\ndefault is rename";
			langHelpBppSize = "size and bpp, multiple out split by ';', bpp can be ignore\r\nusage: [size1],[bpp1];[size2]...\r\ne.g. 48,32;24,16;64\r\ndefault is 48,32";
			langHelpHelp = "help";
		}

		public static bool IsSimpleChinese() {
			return System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN";
		}

		//public string langSetting = "Setting";
		//public string langBpp = " Bpp";
		//public string langExportMulti = "Export Multi";
		//public string langDragImageHere = "Drag Image Here";
		//public string langOk = "Ok";

		//public string langStrPopWin1 = "File<";
		//public string langStrPopWin2 = "> exist，sure to replace？";

		//public string langReplace = "Replace";
		//public string langReplaceAll = "Replace All";
		//public string langJump = "Jump";
		//public string langCancel = "Cancel";
		//public string langRename = "Rename";
		//public string langWhenFileExist = "When File Exist:";
		//public string langOutType = "Out Type";
		//public string langAuto = "Auto";

		//public string langHelpSrc = "multiple source file. file1 file2 ...";
		//public string langHelpDst = "out file directory, output to source directory if is empty";
		//public string langHelpType = "out file type\r\noptional: auto,ico,bmp,jpg,png. default is auto:\r\nout png if src is ico\r\nout ico if src is image\r\nout png if src is orther file";
		//public string langHelpOperate = "operate when file exist\r\noptional:rename,jump,overwrite\r\ndefault is rename";
		//public string langHelpBppSize = "size and bpp, multiple out split by ';', bpp can be ignore\r\nusage: [size1],[bpp1];[size2]...\r\ne.g. 48,32;24,16;64\r\ndefault is 48";
		//public string langHelpHelp = "help";

		public string langSetting = "设置";
		public string langBpp = "位";
		public string langExportMulti = "导出多个";
		public string langDragImageHere = "拖拽图片到这里";
		public string langOk = "确定";

		public string langStrPopWin1 = "文件<";
		public string langStrPopWin2 = "> 已存在，确定替换？";

		public string langReplace = "替换";
		public string langReplaceAll = "替换所有";
		public string langJump = "跳过";
		public string langCancel = "取消";
		public string langRename = "重命名";
		public string langWhenFileExist = "文件存在时：";
		public string langOutType = "输出格式";
		public string langAuto = "自动";

		public string langHelpSrc = "源文件或目录路径。file1 file2 ...";
		public string langHelpDst = "输出目录，如果为空则输出到源文件目录";
		public string langHelpType = "输出文件格式\r\n可选项：auto,ico,bmp,jpg,png。默认为auto：\r\n当源文件为ico时输出png\r\n当源文件为图片时输出ico\r\n当源文件为其他文件时输出png";
		public string langHelpOperate = "输出文件存在时的操作\r\n可选项：rename,jump,overwrite\r\n为默认值rename";
		public string langHelpBppSize = "尺寸和bpp，多个输出用';'分割，bpp可忽略\r\n用法：[size1],[bpp1?];[size2]...\r\n例： 48,32;24,16;64\r\n默认为 48,32";
		public string langHelpHelp = "帮助";
	}
}
