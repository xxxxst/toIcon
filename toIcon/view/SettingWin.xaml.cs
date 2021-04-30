using csharpHelp.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using toIcon.util;
using toIconCom.model;

namespace toIcon.view {
	/// <summary>
	/// SettingWin.xaml 的交互逻辑
	/// </summary>
	public partial class SettingWin : Window {
		string strRegFile = @"HKEY_CLASSES_ROOT\*\shell\toIcon\";
		//string strRegDir = @"HKEY_CLASSES_ROOT\Directory\Background\shell\toIcon\";

		RegistryCtl regCtl = new RegistryCtl();

		bool isRegFile = false;
		//bool isRegDir = false;
		List<string> lstLang = new List<string>() {
			"自动", "",
			"English", "en-US",
			"简体中文", "zh-CN",
		};

		public SettingWin() {
			InitializeComponent();

			Title = Lang.ins.langMoreSetting;
			lstLang[0] = Lang.ins.langAuto;
			cbxLang.Content = Lang.ins.langLanguage;

			for (int i = 0; i < lstLang.Count; i += 2) {
				cbxLang.Items.Add(lstLang[i]);
				if (lstLang[i + 1] == Lang.ins.nowLang) {
					cbxLang.SelectedIndex = i / 2;
				}
			}
			if (cbxLang.SelectedIndex < 0) {
				cbxLang.SelectedIndex = 0;
			}

			isRegFile = regCtl.exist(strRegFile);
			//isRegDir = regCtl.exist(strRegDir);
			updataeRegBtnDesc();
		}

		public void show(Window parentWin) {
			Owner = parentWin;
			ShowDialog();
		}

		private void updataeRegBtnDesc() {
			btnRegFile.Content = isRegFile ? Lang.ins.langUnRegFile : Lang.ins.langRegFile;
			//btnRegDir.Content = isRegDir ? Lang.ins.langUnRegDir : Lang.ins.langRegDir;
		}

		private void BtnRegFile_Click(object sender, RoutedEventArgs e) {
			if (isRegFile) {
				regCtl.remove(strRegFile);
			} else {
				regCtl.setValue(strRegFile, Lang.ins.langAppName);
				regCtl.setValue(strRegFile + "Icon", SysConst.exePath());
				regCtl.setValue(strRegFile + "command\\", "\"" + SysConst.exePath() + "\" -s \"%V\"");
			}

			isRegFile = !isRegFile;
			updataeRegBtnDesc();
		}

		//private void BtnRegDir_Click(object sender, RoutedEventArgs e) {
		//	if (isRegDir) {
		//		regCtl.remove(strRegDir);
		//	} else {
		//		regCtl.setValue(strRegDir, Lang.ins.langAppName);
		//		regCtl.setValue(strRegDir + "Icon", SysConst.exePath());
		//		regCtl.setValue(strRegDir + "command\\", "\"" + SysConst.exePath() + "\" -s \"%V\"");
		//	}

		//	isRegDir = !isRegDir;
		//	updataeRegBtnDesc();
		//}

		private void BtnOk_Click(object sender, RoutedEventArgs e) {
			string lang = Lang.ins.lstLang[cbxLang.SelectedIndex * 2 + 1];
			MainWindow.ins.updateLang(lang);
			Close();
		}
	}
}
