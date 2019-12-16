using csharpHelp.util;
using FreeImageAPI;
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
using toIcon.util;

namespace toIcon.view {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window {
		int[] lstSize = new int[] { 256, 128, 96, 72, 64, 48, 32, 24, 16 };
		
		CheckBox[][] arrCheckBox = new CheckBox[0][];

		Regex regSupportImage = new Regex("(.png$)|(.bmp$)|(.jpg$)|(.ico$)");
		DragDropEffects nowCursor = DragDropEffects.Copy;
		
		bool isMiniMode = false;

		double normalWidth = 0;
		double normalHeight = 0;
		double miniWidth = 200;
		double miniHeight = 210;

		IniParse parser = null;
		string srcPath = "";

		//string langSetting = "Setting";
		//string langBpp = " Bpp";
		//string langExportMulti = "Export Multi";
		//string langDragImageHere = "Drag Image Here";
		string langSetting = "设置";
		string langBpp = "位";
		string langExportMulti = "导出多个";
		string langDragImageHere = "拖拽图片到这里";

		public MainWindow() {
			InitializeComponent();

			lblLeftTitle32.Content = 32 + langBpp;
			lblLeftTitle8.Content = 8 + langBpp;
			lblLeftTitle4.Content = 4 + langBpp;
			btnSetting.Content = langSetting + "48*48 | 32" + langBpp;
			lblDragImage.Content = langDragImageHere;

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

			loadConfig();
			updateSettingText();
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
					return;
				}

				Left = parser["win"].getInt("x", 100);
				Top = parser["win"].getInt("y", 100);

				string str = parser["config"]["miniMode"];
				if(str != "" && str != "0") {
					setMiniMode(true);
				}

				str = parser["config"]["checked"];
				string[] arr = str.Split(';');
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
				//string[] arr = str.Split(',');
				//if(arr.Length == 2) {
				//	int row = getInt(arr[0]);
				//	int col = getInt(arr[1]);
				//	if(row >= 0 && row < 3 && col >= 0 && col < lstSize.Length) {
				//		setMiniMode(true, row, col);
				//	}
				//}
			} catch(Exception ex) { Debug.WriteLine(ex); }
		}

		private void saveConfig() {
			try {
				parser["win"].setInt("x", (int)Left);
				parser["win"].setInt("y", (int)Top);

				if(isMiniMode) {
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
						if(str != "") { str += ";"; }
						str += i + "," + j;
					}
				}
				parser["config"]["checked"] = str;

				parser.save();
			} catch(Exception ex) { Debug.WriteLine(ex); }
		}

		//private Label createLabel(int size, int row, int col) {
		//	Label lbl = new Label();
		//	lbl.VerticalAlignment = VerticalAlignment.Center;
		//	lbl.HorizontalAlignment = HorizontalAlignment.Center;
		//	lbl.Content = size + "*" + size;

		//	grdBox.Children.Add(lbl);
		//	Grid.SetColumn(lbl, col);
		//	Grid.SetRow(lbl, row);
			
		//	return lbl;
		//}

		//private ImageBox createImage(List<ImageBox> lstImg, int size, int row, int col) {
		//	int showSize = Math.Min(size, 128);

		//	ImageBox img = new ImageBox();
		//	img.ImgSize = showSize;
		//	img.MouseEnter += Grid_MouseEnter;
		//	img.MouseLeave += Grid_MouseLeave;
		//	img.MouseUp += Img_MouseUp;
		//	img.OnClickMiniMode += Img_OnClickMiniMode;
		//	if(size == 256) {
		//		img.TextCheckbox = "png";
		//	}
		//	Grid.SetColumn(img, col);
		//	Grid.SetRow(img, row);
		//	grdBox.Children.Add(img);

		//	lstImg.Add(img);
		//	return img;
		//}

		//private void Img_OnClickMiniMode(object sender, RoutedEventArgs e) {
		//	ImageBox ele = sender as ImageBox;
		//	if(ele == null) {
		//		return;
		//	}

		//	int row = Grid.GetRow(ele) - 1;
		//	int col = Grid.GetColumn(ele) - 1;
			
		//	setMiniMode(true, row, col);
		//}

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
			
			if(!regSupportImage.IsMatch(docPath[0])) {
				return;
			}
			
			nowCursor = DragDropEffects.Copy;
		}

		private void Window_Drop(object sender, DragEventArgs e) {
			//拖拽文件
			try {
				string[] docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
				if(docPath.Length <= 0) {
					return;
				}

				if(!regSupportImage.IsMatch(docPath[0])) {
					return;
				}

				//Debug.WriteLine(docPath[0]);

				srcPath = docPath[0];
				//updateImage(docPath[0]);

				if(isMiniMode) {
					export();
				}

			} catch(Exception ex) {
				Debug.WriteLine(ex.ToString());
			}
		}

		//private void updateImage(string path) {
		//	Uri uri = new Uri(path, UriKind.Absolute);

		//	BitmapImage imgData = new BitmapImage(uri);

		//	if(isMiniMode) {
		//		imgBox.Source = imgData;
		//	} else {
		//		eachImageBox((ele, row, col) => ele.Source = imgData);
		//	}
		//}

		private bool isDefaultIcon(int r, int c) {
			return (r==0 && c==5);
		}

		private void export() {
			if(!File.Exists(srcPath)) {
				return;
			}

			string dir = System.IO.Path.GetDirectoryName(srcPath);
			string srcSuffix = System.IO.Path.GetExtension(srcPath).ToLower();

			Popwin win = null;

			const int defIconSize = 48;
			const int defIconBpp = 32;
			List<string> lstFile = new List<string>();
			List<int> lstIcoSize = new List<int>();
			List<int> lstIcoBpp = new List<int>();

			// find all select icon to output
			for(int i = 0; i < lstSize.Length; ++i) {
				for(int j = 0; j < arrCheckBox.Length; ++j) {
					var chk = arrCheckBox[j][i];
					if(chk.IsChecked != true) {
						continue;
					}

					// file name
					string fileName = System.IO.Path.GetFileNameWithoutExtension(srcPath);
					if(!isDefaultIcon(j, i)) {
						fileName += "" + lstSize[i];
					}
					int bpp = getIcoBppByArrayIdx(j);
					if(j > 0) {
						fileName += "-" + bpp;
					}
					fileName += ".ico";

					lstFile.Add(fileName);
					lstIcoSize.Add(lstSize[i]);
					lstIcoBpp.Add(bpp);
				}
			}

			// output one file, use default file name
			if(lstFile.Count == 1) {
				lstFile[0] = System.IO.Path.GetFileNameWithoutExtension(srcPath) + ".ico";
			}

			// add default icon(48*48)
			if(lstFile.Count == 0) {
				string fileName = System.IO.Path.GetFileNameWithoutExtension(srcPath);
				lstFile.Add(fileName + ".ico");
				lstIcoSize.Add(defIconSize);
				lstIcoBpp.Add(defIconBpp);
			}
			
			bool allReplace = false;
			for(int i = 0; i < lstFile.Count; ++i) {
				// file name
				string fileName = lstFile[i];
				string dstPath = dir + "\\" + fileName;

				// pop win
				if(!allReplace && File.Exists(dstPath)) {
					if(win == null) {
						win = new Popwin();
					}
					win.show(this, fileName);
					switch(win.type) {
						case Popwin.SelecType.Cancel: win.Close(); return;
						case Popwin.SelecType.Jump: continue;
						case Popwin.SelecType.ReplaceAll: allReplace = true; break;
					}
				}

				int icoSize = lstIcoSize[i];
				//int type = lstIcoType[i];
				int bpp = lstIcoBpp[i];

				// save
				try {
					FIBITMAP dib;
					switch(srcSuffix) {
						case ".ico": {
							dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_ICO, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
							break;
						}
						case ".bmp": {
							dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_BMP, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
							break;
						}
						case ".jpg": {
							dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_JPEG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
							break;
						}
						case ".png": default: {
							dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
							break;
						}
					}
					//FIBITMAP dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
					uint width = FreeImage.GetWidth(dib);
					uint height = FreeImage.GetHeight(dib);
					//FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_BICUBIC);
					
					FIBITMAP dibOut = formatImage(dib, icoSize, bpp);
					
					FreeImage.Save(FREE_IMAGE_FORMAT.FIF_ICO, dibOut, dstPath, FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE);
					//bool isOk = FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, dibOut, dstPath + ".png", FREE_IMAGE_SAVE_FLAGS.PNG_INTERLACED);
					FreeImage.Unload(dibOut);
					FreeImage.Unload(dib);
				} catch(Exception ex) { Debug.WriteLine(ex.ToString()); }
			}

			if(win != null) {
				win.Close();
			}

		}

		private int getIcoBppByArrayIdx(int idx) {
			if(idx == 1) {
				return 8;
			} else if(idx == 2) {
				return 4;
			}

			return 32;
		}

		private FIBITMAP formatImage(FIBITMAP dib, int icoSize, int dstBpp) {
			const int hideOpacity = 128;
			
			//FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_BICUBIC);
			FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_LANCZOS3);

			if(dstBpp >= 32) {
				return dibTmp;
			}

			int paletteSize = (int)Math.Pow(2, dstBpp);
			byte lastPaletteIdx = (byte)(paletteSize - 1);

			// quantize ico
			// reserve last one palette, to set transparent
			//FIBITMAP dibTmp2 = FreeImage.ConvertColorDepth(dibTmp, FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP);
			FIBITMAP dibOut = FreeImage.ColorQuantizeEx(dibTmp, FREE_IMAGE_QUANTIZE.FIQ_WUQUANT, paletteSize - 1, null, dstBpp);

			// set transparent color index to last one palette
			RGBQUAD rgb = new RGBQUAD();
			byte val = 0;
			for(int m = icoSize - 1; m >= 0; --m) {
				string str = "";
				for(int n = icoSize - 1; n >= 0; --n) {
					FreeImage.GetPixelIndex(dibOut, (uint)n, (uint)m, out val);
					FreeImage.GetPixelColor(dibTmp, (uint)n, (uint)m, out rgb);
					//uint val = rgb.uintValue & 0x00FFFFFF;
					if(rgb.rgbReserved <= hideOpacity) {
						FreeImage.SetPixelIndex(dibOut, (uint)n, (uint)m, ref lastPaletteIdx);
					}
					str += val + ",";
				}
			}

			FreeImage.Unload(dibTmp);

			//uint bpp = FreeImage.GetBPP(dibOut);

			// set transarency table
			try {
				RGBQUAD[] palette = new Palette(dibOut).AsArray;
				byte[] transparency = new byte[palette.Length];
				for(int m = 0; m < palette.Length; ++m) {
					transparency[m] = 0xFF;
					if(m == lastPaletteIdx) {
						transparency[m] = 0;
					}
				}
				FreeImage.SetTransparencyTable(dibOut, transparency);
			} catch(Exception) { }

			return dibOut;
		}

		private void Window_DragOver(object sender, DragEventArgs e) {
			e.Effects = nowCursor;
			e.Handled = true;
		}

		//private void updateBackground() {
		//	if(overRowIdx < 0) {
		//		grdBackRow.Background = brushEmpty;
		//	} else {
		//		grdBackRow.Background = brushOver;
		//		Grid.SetRow(grdBackRow, overRowIdx);
		//	}
		//	if(overColIdx < 0) {
		//		grdBackCol.Background = brushEmpty;
		//	} else {
		//		grdBackCol.Background = brushOver;
		//		Grid.SetColumn(grdBackCol, overColIdx);
		//	}
		//}

		//private void Grid_MouseEnter(object sender, MouseEventArgs e) {
		//	overImage = sender as ImageBox;
		//	overRowIdx = Grid.GetRow(overImage);
		//	overColIdx = Grid.GetColumn(overImage);
		//	updateBackground();
		//}

		//private void Grid_MouseLeave(object sender, MouseEventArgs e) {
		//	if(overImage != sender) {
		//		return;
		//	}
		//	overRowIdx = -1;
		//	overColIdx = -1;
		//	updateBackground();
		//}

		//private void eachImageBox(Action<ImageBox, int, int> fun) {
		//	for(int i = 0; i < lstImg32.Count; ++i) {
		//		fun(lstImg32[i], 0, i);
		//	}
		//	for(int i = 0; i < lstImg8.Count; ++i) {
		//		fun(lstImg8[i], 1, i);
		//	}
		//	for(int i = 0; i < lstImg4.Count; ++i) {
		//		fun(lstImg4[i], 2, i);
		//	}
		//}

		//private void clearImage() {
		//	eachImageBox((tmp, rowTmp, colTmp) => tmp.Source = null);
		//	imgBox.Source = null;
		//}

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

		//private void setMiniMode(bool _isMiniMode, int row = 0, int col = 0) {
		//	clearImage();

		//	miniSelectRow = row;
		//	miniSelectCol = col;

		//	isMiniMode = _isMiniMode;

		//	if(!isMiniMode) {
		//		Width = normalWidth;
		//		Height = normalHeight;
		//		grdBox.Visibility = Visibility.Visible;
		//		grdMini.Visibility = Visibility.Collapsed;
		//		return;
		//	}

		//	int size = lstSize[col];
		//	int viewSize = Math.Min(size, 128);

		//	Width = miniWidth;
		//	Height = miniHeight;
		//	grdBox.Visibility = Visibility.Collapsed;
		//	grdMini.Visibility = Visibility.Visible;
		//	lblMiniDesc.Content = getMiniModeDesc();
		//	//imgBox.Width = size;
		//	//imgBox.Height = size;
		//	imgBox.ImgSize = size;
		//	if(size == 256) {
		//		imgBox.TextCheckbox = "png";
		//	}
		//}

		//private string getMiniModeDesc() {
		//	int row = miniSelectRow;
		//	int col = miniSelectCol;

		//	int size = lstSize[col];
		//	string desc = "";
		//	switch(row) {
		//		case 0: desc += "32位 | "; break;
		//		case 1: desc += "8位 | "; break;
		//		case 2: desc += "4位 | "; break;
		//	}
		//	desc += size + "*" + size;

		//	return desc;
		//}

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
			string checkedSize = "48*48 | 32" + langBpp;
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

					checkedSize = lstSize[i] + "*" + lstSize[i] + " | " + bpp + langBpp;
				}
				if(checkedCount > 1) {
					break;
				}
			}

			if(checkedCount > 1) {
				btnSetting.Content = langSetting + " | " + langExportMulti;
				return;
			}

			btnSetting.Content = langSetting + " | " + checkedSize;
		}

		private void BtnSetting_Click(object sender, RoutedEventArgs e) {
			setMiniMode(false);
		}
	}
}
