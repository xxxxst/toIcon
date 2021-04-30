using csharpHelp.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using toIcon.model;
using toIcon.util;
using toIconCom.control;
using toIconCom.model;
using toIconCom.util;

namespace toIcon.view {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window {
		public static MainWindow ins = null;

		int[] lstSize = new int[] { 256, 128, 96, 72, 64, 48, 32, 24, 16 };
		
		CheckBox[][] arrCheckBox = new CheckBox[0][];

		IconCtl iconCtl = new IconCtl();

		//Regex regSupportImage = new Regex("(.png$)|(.bmp$)|(.jpg$)|(.ico$)");
		DragDropEffects nowCursor = DragDropEffects.Copy;
		
		bool isMiniMode = false;

		double normalWidth = 0;
		double normalHeight = 0;
		double miniWidth = 200;
		double miniHeight = 210;

		IniParse parser = null;
		//string srcPath = "";

		//string langSetting = "设置";
		//string langBpp = "位";
		//string langExportMulti = "导出多个";
		//string langDragImageHere = "拖拽图片到这里";

		public MainWindow() {
			InitializeComponent();

			ins = this;

			//set icon
			//Uri iconUri = new Uri(LocalRes.icon16(), UriKind.RelativeOrAbsolute);
			//Icon = BitmapFrame.Create(iconUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

			lblVersion.Content = "v" + ComSysConst.version;

			normalWidth = Width;
			normalHeight = Height;

			Width = miniWidth;
			Height = miniHeight;

			grdMini.Visibility = Visibility.Visible;
			grdBox.Visibility = Visibility.Collapsed;

			arrCheckBox = new CheckBox[][] {
				new CheckBox[]{ chk_0_0, chk_0_1, chk_0_2, chk_0_3, chk_0_4, chk_0_5, chk_0_6, chk_0_7, chk_0_8 },
				new CheckBox[]{ chk_1_0, chk_1_1, chk_1_2, chk_1_3, chk_1_4, chk_1_5, chk_1_6, chk_1_7, chk_1_8 },
				new CheckBox[]{ chk_2_0, chk_2_1, chk_2_2, chk_2_3, chk_2_4, chk_2_5, chk_2_6, chk_2_7, chk_2_8 },
			};

			//for(int i = 0; i < lstSize.Length; ++i) {
			//	int size = Math.Min(lstSize[i], 128);
			//	int width = Math.Max(size + 2, 64);

			//	ColumnDefinition rd = new ColumnDefinition();
			//	rd.Width = new GridLength(width);
			//	grdBox.ColumnDefinitions.Insert(grdBox.ColumnDefinitions.Count-1, rd);
			//}

			//for(int i = 0; i < lstSize.Length; ++i) {
			//	createLabel(lstSize[i], 0, i + 1);

			//	createImage(lstImg32, lstSize[i], 1, i + 1);
			//	createImage(lstImg8, lstSize[i], 2, i + 1);
			//	createImage(lstImg4, lstSize[i], 3, i + 1);
			//}

			cbxOutType.Items.Add(Lang.ins.langAuto);
			cbxOutType.Items.Add("ico");
			cbxOutType.Items.Add("png");
			cbxOutType.Items.Add("jpg");
			cbxOutType.Items.Add("bmp");
			cbxOutType.SelectedIndex = 0;

			cbxOperate.Items.Add(Lang.ins.langRename);
			cbxOperate.Items.Add(Lang.ins.langReplace);
			cbxOperate.Items.Add(Lang.ins.langJump);
			cbxOperate.SelectedIndex = 0;

			loadConfig();
			onLangUpdate();

			updateSettingText();
		}

		private void onLangUpdate() {
			lblLeftTitle32.Content = 32 + Lang.ins.langBpp;
			lblLeftTitle8.Content = 8 + Lang.ins.langBpp;
			lblLeftTitle4.Content = 4 + Lang.ins.langBpp;
			btnSetting.Content = Lang.ins.langSetting + "48*48 | 32" + Lang.ins.langBpp;
			lblDragImage.Content = Lang.ins.langDragImageHere;
			lblOutType.Content = Lang.ins.langOutType;
			lblWhenFileExist.Content = Lang.ins.langWhenFileExist;
			chkMergeOutput.Content = Lang.ins.langMergeOutput;
			chkMergeOutput.ToolTip = Lang.ins.langIcoValid;
			lblMerge.Content = Lang.ins.langMerge;
			//lblWhenFileExist2.Content = Lang.ins.langWhenFileExist;
			btnOk.Content = Lang.ins.langOk;
			cbxOutType.ToolTip = Lang.ins.langAutoDesc;
			btnMoreSetting.Content = Lang.ins.langMoreSetting;

			int idx = cbxOutType.SelectedIndex;
			cbxOutType.Items.Clear();
			cbxOutType.Items.Add(Lang.ins.langAuto);
			cbxOutType.Items.Add("ico");
			cbxOutType.Items.Add("png");
			cbxOutType.Items.Add("jpg");
			cbxOutType.Items.Add("bmp");
			cbxOutType.SelectedIndex = idx;

			int idx2 = cbxOperate.SelectedIndex;
			cbxOperate.Items.Clear();
			cbxOperate.Items.Add(Lang.ins.langRename);
			cbxOperate.Items.Add(Lang.ins.langReplace);
			cbxOperate.Items.Add(Lang.ins.langJump);
			cbxOperate.SelectedIndex = idx2;
		}

		public void updateLang(string lang) {
			if (lang == Lang.ins.nowLang) {
				return;
			}

			Lang.ins.setLang(lang);
			onLangUpdate();
		}

		private int getInt(string str, int def = 0) {
			bool isOk = int.TryParse(str, out int rst);
			return isOk ? rst : def;
		}

		private void loadConfig() {
			try {
				string path = SysConst.configPath();
				parser = new IniParse(path);

				if(!File.Exists(path)) {
					WindowStartupLocation = WindowStartupLocation.CenterScreen;
					return;
				}

				Left = parser["win"].getInt("x", 100);
				Top = parser["win"].getInt("y", 100);

				// language
				string str = parser["win"]["language"];
				Lang.ins.setLang(str);

				str = parser["config"]["miniMode"];
				if(str != "" && str != "0") {
					setMiniMode(true);
				}

				str = parser["config"]["checked"];
				string[] arr = str.Split('$');
				for(int i = 0; i < arr.Length; ++i) {
					var tmp = arr[i].Split(',');
					if(tmp.Length < 2) {
						continue;
					}
					var r = getInt(tmp[0]);
					var c = getInt(tmp[1]);
					if(r < 0 || r >= arrCheckBox.Length) {
						continue;
					}
					if(c < 0 || c >= arrCheckBox[r].Length) {
						continue;
					}

					arrCheckBox[r][c].IsChecked = true;
				}

				// outType
				str = parser["config"]["outType"];
				int.TryParse(str, out int outType);
				if(outType < 0 || outType >= cbxOutType.Items.Count) {
					outType = 0;
				}
				cbxOutType.SelectedIndex = outType;
				lblShowOutType.Content = cbxOutType.Items[outType].ToString();

				// operate
				str = parser["config"]["operate"];
				int.TryParse(str, out int operate);
				if(operate < 0 || operate >= cbxOperate.Items.Count) {
					operate = 0;
				}
				cbxOperate.SelectedIndex = operate;
				lblShowOperate.Content = cbxOperate.Items[operate].ToString();

				// merge output
				str = parser["config"]["mergeOutput"];
				int.TryParse(str, out int mergeOutput);
				chkMergeOutput.IsChecked = (mergeOutput != 0);
				stkMergeOutput.Visibility = (chkMergeOutput.IsChecked == true ? Visibility.Visible : Visibility.Collapsed);

			} catch(Exception ex) { Debug.WriteLine(ex); }
		}

		private void saveConfig() {
			try {
				parser["win"].setInt("x", (int)Left);
				parser["win"].setInt("y", (int)Top);
				parser["win"]["language"] = Lang.ins.nowLang;

				if (isMiniMode) {
					//parser["config"]["miniMode"] = miniSelectRow + "," + miniSelectCol;
					parser["config"]["miniMode"] = "1";
				} else {
					parser["config"]["miniMode"] = "";
				}

				string str = "";
				for(int i = 0; i < arrCheckBox.Length; ++i) {
					for(int j = 0; j < arrCheckBox[i].Length; ++j) {
						if(arrCheckBox[i][j].IsChecked != true) {
							continue;
						}
						if(str != "") { str += "$"; }
						str += i + "," + j;
					}
				}
				parser["config"]["checked"] = str;
				parser["config"]["outType"] = "" + cbxOutType.SelectedIndex;
				parser["config"]["operate"] = "" + cbxOperate.SelectedIndex;
				parser["config"]["mergeOutput"] = (chkMergeOutput.IsChecked == true ? "1" : "0");

				parser.save();
			} catch(Exception ex) { Debug.WriteLine(ex); }
		}

		private void Window_DragEnter(object sender, DragEventArgs e) {
			e.Handled = true;

			nowCursor = DragDropEffects.None;

			if(!isMiniMode) {
				return;
			}

			string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
			if(docPath.Length <= 0) {
				return;
			}
			
			//if(!regSupportImage.IsMatch(docPath[0])) {
			//	return;
			//}
			
			nowCursor = DragDropEffects.Copy;
		}

		private void Window_Drop(object sender, DragEventArgs e) {
			//拖拽文件
			try {
				string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
				if(docPath.Length <= 0) {
					return;
				}

				//if(!regSupportImage.IsMatch(docPath[0])) {
				//	return;
				//}

				//srcPath = docPath[0];

				if(isMiniMode) {
					export(docPath);
				}

			} catch(Exception ex) {
				Debug.WriteLine(ex.ToString());
			}
		}

		private bool isDefaultIcon(int r, int c) {
			return (r==0 && c==5);
		}

		private string getBppSizeStr() {
			string rst = "";
			for(int i = 0; i < lstSize.Length; ++i) {
				for(int j = 0; j < arrCheckBox.Length; ++j) {
					var chk = arrCheckBox[j][i];
					if(chk.IsChecked != true) {
						continue;
					}
					int bpp = getIcoBppByArrayIdx(j);
					rst += (rst == "") ? "" : ";";
					rst += $"{lstSize[i]},{bpp}";
				}
			}
			return rst;
		}

		private string getOutType() {
			switch(cbxOutType.SelectedIndex) {
				case 1: return "ico";
				case 2: return "png";
				case 3: return "jpg";
				case 4: return "bmp";
				default: return "auto";
			}
		}

		private string getOperateStr() {
			switch(cbxOperate.SelectedIndex) {
				case 1: return "overwrite";
				case 2: return "jump";
				default: return "rename";
			}
		}

		private void export(string[] srcMultiPath) {
			string bppSize = getBppSizeStr();
			string outType = getOutType();
			string operate = getOperateStr();
			bool isMergeOutput = (chkMergeOutput.IsChecked == true);

			iconCtl.convert(srcMultiPath, "", bppSize, outType, operate, isMergeOutput);
			return;

			//if(!File.Exists(srcPath)) {
			//	return;
			//}

			//string dir = System.IO.Path.GetDirectoryName(srcPath);
			//string srcSuffix = System.IO.Path.GetExtension(srcPath).ToLower();

			//Popwin win = null;

			//const int defIconSize = 48;
			//const int defIconBpp = 32;
			//List<string> lstFile = new List<string>();
			//List<int> lstIcoSize = new List<int>();
			//List<int> lstIcoBpp = new List<int>();

			//// find all select icon to output
			//for(int i = 0; i < lstSize.Length; ++i) {
			//	for(int j = 0; j < arrCheckBox.Length; ++j) {
			//		var chk = arrCheckBox[j][i];
			//		if(chk.IsChecked != true) {
			//			continue;
			//		}

			//		// file name
			//		string fileName = System.IO.Path.GetFileNameWithoutExtension(srcPath);
			//		int bpp = getIcoBppByArrayIdx(j);
			//		if(!isDefaultIcon(j, i)) {
			//			fileName += "_" + lstSize[i] + "_" + bpp;
			//		}
			//		//if(!isDefaultBpp(j)) {
			//		//	fileName += "_" + bpp;
			//		//}
			//		fileName += ".ico";

			//		lstFile.Add(fileName);
			//		lstIcoSize.Add(lstSize[i]);
			//		lstIcoBpp.Add(bpp);
			//	}
			//}

			//// output one file, use default file name
			//if(lstFile.Count == 1) {
			//	lstFile[0] = System.IO.Path.GetFileNameWithoutExtension(srcPath) + ".ico";
			//}

			//// add default icon(48*48)
			//if(lstFile.Count == 0) {
			//	string fileName = System.IO.Path.GetFileNameWithoutExtension(srcPath);
			//	lstFile.Add(fileName + ".ico");
			//	lstIcoSize.Add(defIconSize);
			//	lstIcoBpp.Add(defIconBpp);
			//}
			
			//bool allReplace = false;
			//for(int i = 0; i < lstFile.Count; ++i) {
			//	// file name
			//	string fileName = lstFile[i];
			//	string dstPath = dir + "\\" + fileName;

			//	// pop win
			//	if(!allReplace && File.Exists(dstPath)) {
			//		if(win == null) {
			//			win = new Popwin();
			//		}
			//		win.show(this, fileName);
			//		switch(win.type) {
			//			case Popwin.SelecType.Cancel: win.Close(); return;
			//			case Popwin.SelecType.Jump: continue;
			//			case Popwin.SelecType.ReplaceAll: allReplace = true; break;
			//		}
			//	}

			//	int icoSize = lstIcoSize[i];
			//	//int type = lstIcoType[i];
			//	int bpp = lstIcoBpp[i];

			//	// save
			//	try {
			//		FIBITMAP dib;
			//		switch(srcSuffix) {
			//			case ".ico": {
			//				dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_ICO, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			//				break;
			//			}
			//			case ".bmp": {
			//				dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			//				break;
			//			}
			//			case ".jpg": {
			//				dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			//				break;
			//			}
			//			case ".png": default: {
			//				dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			//				break;
			//			}
			//		}
			//		//FIBITMAP dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
			//		uint width = FreeImage.GetWidth(dib);
			//		uint height = FreeImage.GetHeight(dib);
			//		//FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_BICUBIC);
					
			//		FIBITMAP dibOut = formatImage(dib, icoSize, bpp);
					
			//		FreeImage.Save(FREE_IMAGE_FORMAT.FIF_ICO, dibOut, dstPath, FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE);
			//		//bool isOk = FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, dibOut, dstPath + ".png", FREE_IMAGE_SAVE_FLAGS.PNG_INTERLACED);
			//		FreeImage.Unload(dibOut);
			//		FreeImage.Unload(dib);
			//	} catch(Exception ex) { Debug.WriteLine(ex.ToString()); }
			//}

			//if(win != null) {
			//	win.Close();
			//}

		}

		private int getIcoBppByArrayIdx(int idx) {
			if(idx == 1) {
				return 8;
			} else if(idx == 2) {
				return 4;
			}

			return 32;
		}

		//private FIBITMAP formatImage(FIBITMAP dib, int icoSize, int dstBpp) {
		//	const int hideOpacity = 128;
			
		//	//FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_BICUBIC);
		//	FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_LANCZOS3);

		//	if(dstBpp >= 32) {
		//		return dibTmp;
		//	}

		//	int paletteSize = (int)Math.Pow(2, dstBpp);
		//	byte lastPaletteIdx = (byte)(paletteSize - 1);

		//	// quantize ico
		//	// reserve last one palette, to set transparent
		//	//FIBITMAP dibTmp2 = FreeImage.ConvertColorDepth(dibTmp, FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP);
		//	FIBITMAP dibOut = FreeImage.ColorQuantizeEx(dibTmp, FREE_IMAGE_QUANTIZE.FIQ_WUQUANT, paletteSize - 1, null, dstBpp);

		//	// set transparent color index to last one palette
		//	RGBQUAD rgb = new RGBQUAD();
		//	byte val = 0;
		//	for(int m = icoSize - 1; m >= 0; --m) {
		//		string str = "";
		//		for(int n = icoSize - 1; n >= 0; --n) {
		//			FreeImage.GetPixelIndex(dibOut, (uint)n, (uint)m, out val);
		//			FreeImage.GetPixelColor(dibTmp, (uint)n, (uint)m, out rgb);
		//			//uint val = rgb.uintValue & 0x00FFFFFF;
		//			if(rgb.rgbReserved <= hideOpacity) {
		//				FreeImage.SetPixelIndex(dibOut, (uint)n, (uint)m, ref lastPaletteIdx);
		//			}
		//			str += val + ",";
		//		}
		//	}

		//	FreeImage.Unload(dibTmp);

		//	//uint bpp = FreeImage.GetBPP(dibOut);

		//	// set transarency table
		//	try {
		//		RGBQUAD[] palette = new Palette(dibOut).AsArray;
		//		byte[] transparency = new byte[palette.Length];
		//		for(int m = 0; m < palette.Length; ++m) {
		//			transparency[m] = 0xFF;
		//			if(m == lastPaletteIdx) {
		//				transparency[m] = 0;
		//			}
		//		}
		//		FreeImage.SetTransparencyTable(dibOut, transparency);
		//	} catch(Exception) { }

		//	return dibOut;
		//}

		private void Window_DragOver(object sender, DragEventArgs e) {
			e.Effects = nowCursor;
			e.Handled = true;
		}

		private void setMiniMode(bool _isMiniMode) {
			isMiniMode = _isMiniMode;
			if(!isMiniMode) {
				Width = normalWidth;
				Height = normalHeight;
				grdMini.Visibility = Visibility.Collapsed;
				grdBox.Visibility = Visibility.Visible;
				return;
			}

			Width = miniWidth;
			Height = miniHeight;
			grdMini.Visibility = Visibility.Visible;
			grdBox.Visibility = Visibility.Collapsed;
		}

		private void Img_MouseUp(object sender, MouseButtonEventArgs e) {

			
		}

		private void ImgBox_OnClickMiniMode(object sender, RoutedEventArgs e) {
			setMiniMode(false);
		}

		private void Window_Closed(object sender, EventArgs e) {
			saveConfig();
		}

		private void BtnOk_Click(object sender, RoutedEventArgs e) {
			setMiniMode(true);
			updateSettingText();

			saveConfig();
		}

		void updateSettingText() {
			if(cbxOutType.SelectedIndex >= 0) {
				lblShowOutType.Content = cbxOutType.Items[cbxOutType.SelectedIndex].ToString();
			}

			if(cbxOperate.SelectedIndex >= 0) {
				lblShowOperate.Content = cbxOperate.Items[cbxOperate.SelectedIndex].ToString();
			}

			stkMergeOutput.Visibility = (chkMergeOutput.IsChecked == true ? Visibility.Visible : Visibility.Collapsed);

			string checkedSize = "48*48 | 32" + Lang.ins.langBpp;
			int checkedCount = 0;
			for(int i = 0; i < lstSize.Length; ++i) {
				for(int j = 0; j < arrCheckBox.Length; ++j) {
					var chk = arrCheckBox[j][i];
					if(chk.IsChecked != true) {
						continue;
					}
					++checkedCount;
					if(checkedCount > 1) {
						break;
					}

					int bpp = getIcoBppByArrayIdx(j);

					checkedSize = lstSize[i] + "*" + lstSize[i] + " | " + bpp + Lang.ins.langBpp;
				}
				if(checkedCount > 1) {
					break;
				}
			}

			if(checkedCount > 1) {
				btnSetting.Content = Lang.ins.langSetting + " | " + Lang.ins.langExportMulti;
				return;
			}

			btnSetting.Content = Lang.ins.langSetting + " | " + checkedSize;
		}

		private void BtnSetting_Click(object sender, RoutedEventArgs e) {
			setMiniMode(false);
		}

		private void BtnMoreSetting_Click(object sender, RoutedEventArgs e) {
			new SettingWin().show(this);
		}

	}
}
