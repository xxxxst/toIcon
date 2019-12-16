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

namespace toIcon.view {
	/// <summary>
	/// Popwin.xaml 的交互逻辑
	/// </summary>
	public partial class Popwin : Window {
		public enum SelecType { Replace, ReplaceAll, Jump, Cancel };
		public SelecType type = SelecType.Cancel;

		public Popwin() {
			InitializeComponent();
		}

		public void show(Window parent, string fileName) {
			type = SelecType.Cancel;
			lblFileName.Content = fileName;

			Owner = parent;
			ShowDialog();
		}

		private void BtnReplace_Click(object sender, RoutedEventArgs e) {
			type = SelecType.Replace;
			Hide();
		}

		private void BtnReplaceAll_Click(object sender, RoutedEventArgs e) {
			type = SelecType.ReplaceAll;
			Hide();
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e) {
			type = SelecType.Cancel;
			Hide();
		}

		private void BtnJump_Click(object sender, RoutedEventArgs e) {
			type = SelecType.Jump;
			Hide();
		}
	}
}
