using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIcon.model {
	public class Lang {
		public static readonly Lang ins = new Lang();

		//string langSetting = "Setting";
		//string langBpp = " Bpp";
		//string langExportMulti = "Export Multi";
		//string langDragImageHere = "Drag Image Here";

		//public string langStrPopWin1 = "File<";
		//public string langStrPopWin2 = "> exist，sure to replace？";

		//public string langReplace = "Replace";
		//public string langReplaceAll = "Replace All";
		//public string langJump = "Jump";
		//public string langCancel = "Cancel";
		//public string langRename = "Rename";

		public string langSetting = "设置";
		public string langBpp = "位";
		public string langExportMulti = "导出多个";
		public string langDragImageHere = "拖拽图片到这里";

		public string langStrPopWin1 = "文件<";
		public string langStrPopWin2 = "> 已存在，确定替换？";

		public string langReplace = "替换";
		public string langReplaceAll = "替换所有";
		public string langJump = "跳过";
		public string langCancel = "取消";
		public string langRename = "重命名";
	}
}
