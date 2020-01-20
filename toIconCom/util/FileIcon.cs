using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace toIconCom.util {


	public class FileIcon {

		/// <summary>
		/// 根据文件扩展名得到系统扩展名的图标
		/// </summary>
		/// <param name="fileName">文件名(如：win.rar;setup.exe;temp.txt)</param>
		/// <param name="largeIcon">图标的大小</param>
		/// <returns></returns>
		public static Bitmap getIcon(string fileName, int size = 48) {
			//bool largeIcon = true
			//SHFILEINFO info = new SHFILEINFO(true);
			//int cbFileInfo = Marshal.SizeOf(info);
			//SHGFI flags;
			//if(largeIcon) {
			//	flags = SHGFI.Icon | SHGFI.LargeIcon;
			//} else {
			//	flags = SHGFI.Icon | SHGFI.SmallIcon;
			//}
			//IntPtr IconIntPtr = SHGetFileInfo(fileName, 0, out info, (uint)cbFileInfo, flags);
			//if(IconIntPtr.Equals(IntPtr.Zero)) {
			//	return null;
			//}
			//return Icon.FromHandle(info.hIcon);

			if(size > 48) {
				size = 256;
			} else {
				size = 48;
			}

			IntPtr hIcon = IntPtr.Zero;

			bool isDir = Directory.Exists(fileName);

			string suffix = Path.GetExtension(fileName).ToLower();
			if(suffix == ".exe" && File.Exists(fileName)) {
				//var iconTotalCount = PrivateExtractIcons(srcPath, 0, 0, 0, null, null, 0, 0);
				//int iconTotalCount = 1;
				//IntPtr[] hIcons = new IntPtr[iconTotalCount];
				//int[] ids = new int[iconTotalCount];
				//var successCount = PrivateExtractIcons(fileName, 0, 48, 48, hIcons, ids, iconTotalCount, 0);

				//if(successCount > 0) {
				//	var ico = Icon.FromHandle(hIcons[0]);
				//	if(ico == null) {
				//		return null;
				//	}
				//	Bitmap img = ico.ToBitmap();
				//	DestroyIcon(hIcons[0]);
				//	return img;
				//}

				hIcon = getExeIcon(fileName, size);
			}

			if(hIcon == IntPtr.Zero) {
				int idx = GetIconIndex(suffix, isDir);
				if(size > 48) {
					hIcon = GetJumboIcon(idx);
				} else {
					hIcon = GetXLIcon(idx);
				}
			}
			if(hIcon == IntPtr.Zero) {
				return null;
			}
			var ico = Icon.FromHandle(hIcon);
			if(ico == null) {
				return null;
			}
			return ico.ToBitmap();
		}

		static IntPtr getExeIcon(string fileName, int size) {
			//var iconTotalCount = PrivateExtractIcons(srcPath, 0, 0, 0, null, null, 0, 0);
			int iconTotalCount = 1;
			IntPtr[] hIcons = new IntPtr[iconTotalCount];
			int[] ids = new int[iconTotalCount];
			var successCount = PrivateExtractIcons(fileName, 0, size, size, hIcons, ids, iconTotalCount, 0);

			if(successCount > 0) {
				//var ico = Icon.FromHandle(hIcons[0]);
				return hIcons[0];
			}
			return IntPtr.Zero;
		}

		// 256*256
		static IntPtr GetJumboIcon(int iImage) {
			IImageList spiml = null;
			Guid guil = new Guid(IID_IImageList2);

			SHGetImageList(SHIL_JUMBO, ref guil, ref spiml);
			IntPtr hIcon = IntPtr.Zero;
			spiml.GetIcon(iImage, ILD_TRANSPARENT | ILD_IMAGE, ref hIcon);

			return hIcon;
		}

		// 48X48
		static IntPtr GetXLIcon(int iImage) {
			IImageList spiml = null;
			Guid guil = new Guid(IID_IImageList);

			SHGetImageList(SHIL_EXTRALARGE, ref guil, ref spiml);
			IntPtr hIcon = IntPtr.Zero;
			spiml.GetIcon(iImage, ILD_TRANSPARENT | ILD_IMAGE, ref hIcon);

			return hIcon;
		}

		static int GetIconIndex(string pszFile, bool isDir = false) {
			SHFILEINFO sfi = new SHFILEINFO();
			SHGFI flag = SHGFI.SysIconIndex | SHGFI.LargeIcon;
			if(!isDir) {
				flag = flag | SHGFI.UseFileAttributes;
			}

			SHGetFileInfo(pszFile, 0, ref sfi, (uint)Marshal.SizeOf(sfi), (uint)(flag));
			return sfi.iIcon;
		}

		//public static Icon getFolderIcon(string path, bool largeIcon = true) {
		//	SHFILEINFO info = new SHFILEINFO();
		//	int cbFileInfo = Marshal.SizeOf(info);
		//	SHGFI flags;
		//	if(largeIcon) {
		//		flags = SHGFI.Icon | SHGFI.LargeIcon;
		//	} else {
		//		flags = SHGFI.Icon | SHGFI.SmallIcon;
		//	}

		//	IntPtr IconIntPtr = SHGetFileInfo(path, 48, out info, (uint)cbFileInfo, flags);
		//	if(IconIntPtr.Equals(IntPtr.Zero)) {
		//		return null;
		//	}
		//	Icon _Icon = Icon.FromHandle(info.hIcon);
		//	return _Icon;
		//}

		
		
		

		[DllImport("User32.dll")]
		public static extern int PrivateExtractIcons(string lpszFile, int nIconIndex, int cxIcon, int cyIcon, IntPtr[] phicon, int[] piconid, int nIcons, int flags);
		[DllImport("User32.dll")]
		public static extern bool DestroyIcon(IntPtr hIcon);

		//[DllImport("Shell32.dll")]
		//private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);

		//[StructLayout(LayoutKind.Sequential)]
		//private struct SHFILEINFO {
		//	public SHFILEINFO(bool b) {
		//		hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
		//	}
		//	public IntPtr hIcon;
		//	public int iIcon;
		//	public uint dwAttributes;
		//	[MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
		//	public string szDisplayName;
		//	[MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
		//	public string szTypeName;
		//}

		//private enum SHGFI {
		//	SmallIcon = 0x00000001,
		//	LargeIcon = 0x00000000,
		//	Icon = 0x00000100,
		//	DisplayName = 0x00000200,
		//	Typename = 0x00000400,
		//	SysIconIndex = 0x00004000,
		//	UseFileAttributes = 0x00000010
		//}

		[DllImport("shell32.dll", EntryPoint = "#727")]
		public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

		[DllImport("Shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[Flags]
		enum SHGFI : uint {
			Icon = 0x000000100,
			DisplayName = 0x000000200,
			TypeName = 0x000000400,
			Attributes = 0x000000800,
			IconLocation = 0x000001000,
			ExeType = 0x000002000,
			SysIconIndex = 0x000004000,
			LinkOverlay = 0x000008000,
			Selected = 0x000010000,
			Attr_Specified = 0x000020000,
			LargeIcon = 0x000000000,
			SmallIcon = 0x000000001,
			OpenIcon = 0x000000002,
			ShellIconSize = 0x000000004,
			PIDL = 0x000000008,
			UseFileAttributes = 0x000000010,
			AddOverlays = 0x000000020,
			OverlayIndex = 0x000000040,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO {
			public const int NAMESIZE = 80;
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};


		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMAGELISTDRAWPARAMS {
			public int cbSize;
			public IntPtr himl;
			public int i;
			public IntPtr hdcDst;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int xBitmap;    // x offest from the upperleft of bitmap
			public int yBitmap;    // y offset from the upperleft of bitmap
			public int rgbBk;
			public int rgbFg;
			public int fStyle;
			public int dwRop;
			public int fState;
			public int Frame;
			public int crEffect;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMAGEINFO {
			public IntPtr hbmImage;
			public IntPtr hbmMask;
			public int Unused1;
			public int Unused2;
			public RECT rcImage;
		}

		[ComImportAttribute()]
		[GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IImageList {
			[PreserveSig]
			int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
			[PreserveSig]
			int ReplaceIcon(int i, IntPtr hicon, ref int pi);
			[PreserveSig]
			int SetOverlayImage(int iImage, int iOverlay);
			[PreserveSig]
			int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
			[PreserveSig]
			int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
			[PreserveSig]
			int Draw(ref IMAGELISTDRAWPARAMS pimldp);
			[PreserveSig]
			int Remove(int i);
			[PreserveSig]
			int GetIcon(int i, int flags, ref IntPtr picon);
			[PreserveSig]
			int GetImageInfo(int i, ref IMAGEINFO pImageInfo);
			[PreserveSig]
			int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
			[PreserveSig]
			int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int Clone(ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int GetImageRect(int i, ref RECT prc);
			[PreserveSig]
			int GetIconSize(ref int cx, ref int cy);
			[PreserveSig]
			int SetIconSize(int cx, int cy);
			[PreserveSig]
			int GetImageCount(ref int pi);
			[PreserveSig]
			int SetImageCount(int uNewCount);
			[PreserveSig]
			int SetBkColor(int clrBk, ref int pclr);
			[PreserveSig]
			int GetBkColor(ref int pclr);
			[PreserveSig]
			int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int EndDrag();
			[PreserveSig]
			int DragEnter(IntPtr hwndLock, int x, int y);
			[PreserveSig]
			int DragLeave(IntPtr hwndLock);
			[PreserveSig]
			int DragMove(int x, int y);
			[PreserveSig]
			int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int DragShowNolock(int fShow);
			[PreserveSig]
			int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int GetItemFlags(int i, ref int dwFlags);
			[PreserveSig]
			int GetOverlayImage(int iOverlay, ref int piIndex);
		};

		const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
		const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";

		public const int SHIL_LARGE = 0x0;
		public const int SHIL_SMALL = 0x1;
		public const int SHIL_EXTRALARGE = 0x2;
		public const int SHIL_SYSSMALL = 0x3;
		public const int SHIL_JUMBO = 0x4;
		public const int SHIL_LAST = 0x4;

		public const int ILD_TRANSPARENT = 0x00000001;
		public const int ILD_IMAGE = 0x00000020;

	}

}
