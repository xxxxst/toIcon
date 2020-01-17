using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toIcon.control {
	public enum ExportFileType {
		Auto, Ico, Bmp, Jpg, Png
	}

	public enum IcoExportOperate {
		Jump,
		Rename,
		Overwrite,
		Cancel,
	}

	public class IconCtl {
		public int[] lstSupportIconSize = new int[] { 256, 128, 96, 72, 64, 48, 32, 24, 16 };

		HashSet<string> hsImageSuffix = new HashSet<string>() { ".ico", ".bmp", ".jpg", ".png" };
		HashSet<string> hsSupportOutType = new HashSet<string>() { "auto", "ico", "bmp", "jpg", "png" };
		HashSet<string> hsSupportOperate = new HashSet<string>() { "jump", "rename", "overwrite" };

		public string convert(string[] srcMultiPath, string dstDir, string multiSizeBpp, string outType = "auto", string operate = "rename") {
			string rstInfo = "Success";

			HashSet<int> hsIconSize = new HashSet<int>();
			for(int i = 0; i < lstSupportIconSize.Length; ++i) {
				hsIconSize.Add(lstSupportIconSize[i]);
			}

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
				lstIcoSize.Add(size);
				lstIcoBpp.Add(bpp);
			}

			for(int i = 0; i < lstIcoSize.Count; ++i) {
				for(int j = 0; j < srcMultiPath.Length; ++j) {
					string path = srcMultiPath[j];
					if(Directory.Exists(path)) {
						continue;
					}
					if(!File.Exists(path)) {
						continue;
					}
					string suffix = Path.GetExtension(path).ToLower();
					string dir = Path.GetDirectoryName(path);
					string fname = Path.GetFileNameWithoutExtension(path);
					string outSuffix = getOutFileSuffix(suffix, outType);
					string dstPath = dir + fname + outSuffix;
					if(File.Exists(dstPath)) {
						switch(operate) {
							case "jump": continue;
							case "overwrite": break;
							case "cancel": return "Cancel";
							case "rename": default: dstPath = renameDstFileName(dstPath); break;
						}
					}
					convert(path, dstPath, lstIcoSize[i], lstIcoBpp[i]);
					//if(hsImageSuffix.Contains(suffix)) {
						
					//}
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
				string path = dir + fname + "." + idx + suffix;
				if(!File.Exists(path)) {
					return path;
				}
				++idx;
			} while(true);

			//return "";
		}

		private string getOutFileSuffix(string srcFileSuffix, string outType) {
			string rst = "";
			switch(outType) {
				case "ico":
				case "bmp":
				case "jpg":
				case "png": {
					return "." + outType;
				}
				case "auto":
				default: {
					if(srcFileSuffix.ToLower() == ".ico") {
						return ".png";
					}
					return ".ico";
					//if(hsImageSuffix.Contains(srcFileSuffix)) {
					//	return ".ico";
					//}
					//break;
				}
			}

			//return "";
		}

		private string getErrorInfo(string param) {
			string rst = "Failed\r\n";
			rst += "Unsupport params: " + param;


			return rst;
		}

		//private string getSuffix(ExportFileType type) {

		//}

		public void convert(string srcPath, string dstPath, int icoSize, int bpp) {
			string srcSuffix = Path.GetExtension(srcPath).ToLower();

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
					case ".png":
					default: {
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
