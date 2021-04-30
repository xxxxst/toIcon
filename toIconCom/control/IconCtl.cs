using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using toIconCom.util;

namespace toIconCom.control {
	public enum ExportFileType {
		Auto, Ico, Bmp, Jpg, Png
	}

	public enum IcoExportOperate {
		Jump, Rename, Overwrite, Cancel,
	}

	public class DibMd : IDisposable {
		public FIBITMAP dib;

		public DibMd(FIBITMAP _dib) {
			dib = _dib;
		}

		public void Dispose() {
			FreeImage.Unload(dib);
		}
	}

	public class IconCtl {
		//[DllImport("User32.dll")]
		//public static extern int PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, int[] piconid, int nIcons, int flags);
		//[DllImport("User32.dll")]
		//public static extern bool DestroyIcon(IntPtr hIcon);

		//public int[] lstSupportIconSize = new int[] { 256, 128, 96, 72, 64, 48, 32, 24, 16 };
		//public int[] lstSupportBpp = new int[] { 32, 8, 4 };
		HashSet<int> hsIconSize = new HashSet<int>() { 256, 128, 96, 72, 64, 48, 32, 24, 16 };
		HashSet<int> hsBpp = new HashSet<int>() { 32, 8, 4 };

		HashSet<string> hsImageSuffix = new HashSet<string>() { ".ico", ".bmp", ".jpg", ".png" };
		HashSet<string> hsSupportOutType = new HashSet<string>() { "auto", "ico", "png", "jpg", "bmp" };
		HashSet<string> hsSupportOperate = new HashSet<string>() { "jump", "rename", "overwrite" };

		private bool isDefaultIcon(int size, int bpp) {
			return (size == 48 && bpp == 32);
		}

		private bool isDefaultBpp(int r) {
			return r == 0;
		}

		//private void ergDir(string path, ref List<string> lstPath) {
		//	if(!Directory.Exists(path)) {
		//		return;
		//	}
		//	DirectoryInfo info = new DirectoryInfo(path);
		//	foreach(FileInfo NextFile in info.GetFiles()) {
		//		if(NextFile.Name == "0-0-11.grid")
		//			continue;

		//		// 获取文件完整路径
		//		string heatmappath = NextFile.FullName;

		//	}
		//}

		public string convert(string[] srcMultiPath, string dstDir, string multiSizeBpp, string outType = "auto", string operate = "rename", bool mergeOutput = false) {
			string rstInfo = "Success";

			//HashSet<int> hsIconSize = new HashSet<int>();
			//for(int i = 0; i < lstSupportIconSize.Length; ++i) {
			//	hsIconSize.Add(lstSupportIconSize[i]);
			//}

			//HashSet<int> hsBpp = new HashSet<int>();
			//for(int i = 0; i < lstSupportBpp.Length; ++i) {
			//	hsBpp.Add(lstSupportBpp[i]);
			//}

			List<int> lstIcoSize = new List<int>();
			List<int> lstIcoBpp = new List<int>();

			// check param : outType
			if(!hsSupportOutType.Contains(outType)) {
				return getErrorInfo(outType);
			}

			// check param : operate
			if(!hsSupportOperate.Contains(operate)) {
				return getErrorInfo(operate);
			}

			// format param : size & bpp;
			string[] arr = multiSizeBpp.Split(new string[] { ";", "；" }, StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < arr.Length; ++i) {
				string[] arr2 = arr[i].Split(new string[] { ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
				if(arr2.Length <= 0) {
					continue;
				}
				int size;
				int bpp = 32;
				bool isOk = int.TryParse(arr2[0], out size);
				if(!isOk || !hsIconSize.Contains(size)) {
					return getErrorInfo(multiSizeBpp);
				}
				if(arr2.Length >= 2) {
					isOk = int.TryParse(arr2[1], out bpp);
					if(!isOk) {
						return getErrorInfo(multiSizeBpp);
					}
				}
				if(!hsBpp.Contains(bpp)) {
					return getErrorInfo(multiSizeBpp);
				}
				lstIcoSize.Add(size);
				lstIcoBpp.Add(bpp);
			}

			if(lstIcoSize.Count == 0) {
				lstIcoSize.Add(48);
				lstIcoBpp.Add(32);
			}

			//List<string> lstSrcPath = new List<string>();
			//for(int i = 0; i < srcMultiPath.Length; ++i) {
			//	if(Directory.Exists(path)) {
			//		continue;
			//	}
			//}

			for(int j = 0; j < srcMultiPath.Length; ++j) {
				List<DibMd> lstData = new List<DibMd>();
				string path = srcMultiPath[j];
				if(!File.Exists(path) && !Directory.Exists(path)) {
					continue;
				}
				string suffix = Path.GetExtension(path).ToLower();
				string dir = Path.GetDirectoryName(path) + "/";
				if(dstDir != "") {
					dir = dstDir + "/";
					Directory.CreateDirectory(dir);

				}


				string outSuffix = getOutFileSuffix(suffix, outType);

				// 只有输出格式ico的才能合并输出
				bool realMergeOutput = mergeOutput;
				if(outSuffix != ".ico") {
					realMergeOutput = false;
				}

				string mergeDstPath = dir + Path.GetFileNameWithoutExtension(path) + outSuffix;
				if(realMergeOutput) {
					if(File.Exists(mergeDstPath)) {
						switch(operate) {
							case "jump": continue;
							case "overwrite": break;
							case "cancel": return "Cancel";
							case "rename": default: mergeDstPath = renameDstFileName(mergeDstPath); break;
						}
					}
				}

				for(int i = 0; i < lstIcoSize.Count; ++i) {
					//if(Directory.Exists(path)) {
					//	continue;
					//}

					string fname = Path.GetFileNameWithoutExtension(path);
					if(!realMergeOutput && !isDefaultIcon(lstIcoSize[i], lstIcoBpp[i])) {
						fname = $"{fname}_{lstIcoSize[i]}_{lstIcoBpp[i]}";
					}

					string dstPath = dir + fname + outSuffix;
					if(File.Exists(dstPath)) {
						switch(operate) {
							case "jump": continue;
							case "overwrite": break;
							case "cancel": return "Cancel";
							case "rename": default: dstPath = renameDstFileName(dstPath); break;
						}
					}
					//convert(path, dstPath, lstIcoSize[i], lstIcoBpp[i]);

					try {
						DibMd md = loadFile(path, lstIcoSize[i]);
						DibMd outMd = formatOutput(md, lstIcoSize[i], lstIcoBpp[i]);

						md?.Dispose();
						if(outMd == null) {
							continue;
						}

						if(realMergeOutput) {
							lstData.Add(outMd);
						} else {
							save(outMd, dstPath);
						}
					} catch(Exception) { }
				}

				// 合并输出
				if(realMergeOutput && lstData.Count > 0) {
					try {
						FIMULTIBITMAP fmb = FreeImage.OpenMultiBitmap(FREE_IMAGE_FORMAT.FIF_ICO, mergeDstPath, true, false, false, FREE_IMAGE_LOAD_FLAGS.ICO_MAKEALPHA);

						for(var i = 0; i < lstData.Count; ++i) {
							FreeImage.AppendPage(fmb, lstData[i].dib);
							lstData[i].Dispose();
						}
						lstData.Clear();
						FreeImage.CloseMultiBitmapEx(ref fmb, FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE);
					} catch(Exception) { }
				}
			}

			return rstInfo;
		}

		private string renameDstFileName(string dstPath) {
			string dir = Path.GetDirectoryName(dstPath);
			string suffix = Path.GetExtension(dstPath);
			string fname = Path.GetFileNameWithoutExtension(dstPath);
			int idx = 0;
			do {
				string path = dir + "/" + fname + "." + idx + suffix;
				if(!File.Exists(path)) {
					return path;
				}
				++idx;
			} while(true);
		}

		private string getOutFileSuffix(string srcFileSuffix, string outType) {
			srcFileSuffix = srcFileSuffix.ToLower();
			//string rst = "";
			switch(outType) {
				case "ico":
				case "bmp":
				case "jpg":
				case "png": {
					return "." + outType;
				}
				case "auto":
				default: {
					//return ".png";
					if(srcFileSuffix == ".ico") {
						return ".png";
					}
					if(hsImageSuffix.Contains(srcFileSuffix)) {
						return ".ico";
					}
					return ".png";
					//return ".ico";
				}
			}
		}

		private string getErrorInfo(string param) {
			string rst = "Failed\r\n";
			rst += "Unsupport params: " + param;


			return rst;
		}

		//private string getSuffix(ExportFileType type) {

		//}

		private DibMd loadFile(string srcPath, int icoSize) {
			string srcSuffix = Path.GetExtension(srcPath).ToLower();

			FIBITMAP dib;
			try {
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
					case ".png": {
						dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
						break;
					}
					default: {
						Bitmap img = FileIcon.getIcon(srcPath, icoSize);
						if(img == null) {
							return null;
						}
						dib = FreeImage.CreateFromBitmap(img);
						break;
						//return;
					}
				}
				return new DibMd(dib);
			} catch(Exception) { }

			return null;
		}

		private DibMd formatOutput(DibMd dibMd, int icoSize, int bpp) {
			if(dibMd == null) {
				return null;
			}

			uint width = FreeImage.GetWidth(dibMd.dib);
			uint height = FreeImage.GetHeight(dibMd.dib);

			try {
				FIBITMAP dibOut = formatImage(dibMd.dib, icoSize, bpp);

				return new DibMd(dibOut);
			} catch(Exception) { }

			return null;
		}

		private void save(DibMd dibMd, string dstPath) {
			if(dibMd == null) {
				return;
			}

			string dstSuffix = Path.GetExtension(dstPath).ToLower();
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_ICO;
			FREE_IMAGE_SAVE_FLAGS flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE;
			switch(dstSuffix) {
				case ".png": format = FREE_IMAGE_FORMAT.FIF_PNG; flag = FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION; break;
				case ".jpg": format = FREE_IMAGE_FORMAT.FIF_JPEG; flag = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD; break;
				case ".bmp": format = FREE_IMAGE_FORMAT.FIF_BMP; flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE; break;
				case ".ico": default: break;
			}
			FreeImage.Save(format, dibMd.dib, dstPath, flag);
			//bool isOk = FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, dibOut, dstPath + ".png", FREE_IMAGE_SAVE_FLAGS.PNG_INTERLACED);
			//FreeImage.Unload(dibMd.dib);
			//FreeImage.Unload(dib);
		}

		private void saveToStream(DibMd dibMd, string dstSuffix, Stream stream) {
			if(dibMd == null) {
				return;
			}

			//string dstSuffix = Path.GetExtension(dstPath).ToLower();
			FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_ICO;
			FREE_IMAGE_SAVE_FLAGS flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE;
			switch(dstSuffix) {
				case ".png": format = FREE_IMAGE_FORMAT.FIF_PNG; flag = FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION; break;
				case ".jpg": format = FREE_IMAGE_FORMAT.FIF_JPEG; flag = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD; break;
				case ".bmp": format = FREE_IMAGE_FORMAT.FIF_BMP; flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE; break;
				case ".ico": default: break;
			}

			FreeImage.SaveToStream(dibMd.dib, stream, format, flag);
		}

		private void convert(string srcPath, string dstPath, int icoSize, int bpp) {
			string srcSuffix = Path.GetExtension(srcPath).ToLower();
			string dstSuffix = Path.GetExtension(dstPath).ToLower();
			//bool isDir = Directory.Exists(srcPath);

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
					case ".png": {
						dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
						break;
					}
					default: {
						Bitmap img = FileIcon.getIcon(srcPath, icoSize);
						if(img == null) {
							return;
						}
						dib = FreeImage.CreateFromBitmap(img);
						break;
						//return;
					}
				}
				//FIBITMAP dib = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, srcPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
				uint width = FreeImage.GetWidth(dib);
				uint height = FreeImage.GetHeight(dib);
				//FIBITMAP dibTmp = FreeImage.Rescale(dib, icoSize, icoSize, FREE_IMAGE_FILTER.FILTER_BICUBIC);

				FIBITMAP dibOut = formatImage(dib, icoSize, bpp);

				FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_ICO;
				FREE_IMAGE_SAVE_FLAGS flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE;
				switch(dstSuffix) {
					case ".png": format = FREE_IMAGE_FORMAT.FIF_PNG; flag = FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION; break;
					case ".jpg": format = FREE_IMAGE_FORMAT.FIF_JPEG; flag = FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYGOOD; break;
					case ".bmp": format = FREE_IMAGE_FORMAT.FIF_BMP; flag = FREE_IMAGE_SAVE_FLAGS.BMP_SAVE_RLE; break;
					case ".ico": default: break;
				}
				FreeImage.Save(format, dibOut, dstPath, flag);
				//bool isOk = FreeImage.Save(FREE_IMAGE_FORMAT.FIF_PNG, dibOut, dstPath + ".png", FREE_IMAGE_SAVE_FLAGS.PNG_INTERLACED);
				FreeImage.Unload(dibOut);
				FreeImage.Unload(dib);
			} catch(Exception ex) { Debug.WriteLine(ex.ToString()); }
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

	}
}
