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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace toIcon.view {
	/// <summary>
	/// ImageBox.xaml 的交互逻辑
	/// </summary>
	public partial class ImageBox : UserControl {
		public ImageBox() {
			InitializeComponent();
		}

		//SelectedMode
		//public static readonly DependencyProperty SelectedModeProperty = DependencyProperty.Register("SelectedMode", typeof(bool), typeof(ImageBox), new PropertyMetadata(false));
		//public bool SelectedMode {
		//	get { return (bool)GetValue(SelectedModeProperty); }
		//	set { SetCurrentValue(SelectedModeProperty, value); }
		//}

		//IsMiniMode
		public static readonly DependencyProperty IsMiniModeProperty = DependencyProperty.RegisterAttached("IsMiniMode", typeof(bool), typeof(ImageBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMiniModeChanged)));
		public bool IsMiniMode {
			get { return (bool)GetValue(IsMiniModeProperty); }
			set { SetCurrentValue(IsMiniModeProperty, value); }
		}

		private static void OnIsMiniModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var ele = d as ImageBox;
			if(ele == null || e.NewValue == null) {
				return;
			}

			ele.btnMini.Content = ele.IsMiniMode ? "B" : "M";
			ele.btnMini.ToolTip = ele.IsMiniMode ? "Back" : "Mini Mode";
			ele.img.VerticalAlignment = ele.IsMiniMode ? VerticalAlignment.Center : VerticalAlignment.Bottom;
		}

		//ShowCheckbox
		public static readonly DependencyProperty TextCheckboxProperty = DependencyProperty.RegisterAttached("TextCheckbox", typeof(string), typeof(ImageBox), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnShowCheckboxChanged)));
		public string TextCheckbox {
			get { return (string)GetValue(TextCheckboxProperty); }
			set { SetCurrentValue(TextCheckboxProperty, value); }
		}

		private static void OnShowCheckboxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var ele = d as ImageBox;
			if(ele == null || e.NewValue == null) {
				return;
			}

			ele.chkInfo.Visibility = (ele.TextCheckbox != "") ? Visibility.Visible : Visibility.Collapsed;
		}

		//SelectedMode
		//public static readonly DependencyProperty SelectedModeProperty = DependencyProperty.RegisterAttached("IsEnabledDragFile", typeof(bool), typeof(ImageBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSelectedModeChanged)));
		//public bool SelectedMode {
		//	get { return (bool)GetValue(SelectedModeProperty); }
		//	set { SetCurrentValue(SelectedModeProperty, value); }
		//}

		//private static void OnSelectedModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		//	var ele = d as ImageBox;
		//	if(ele == null || e.NewValue == null) {
		//		return;
		//	}

		//	ele.chkSelect.IsChecked = false;
		//	ele.chkSelect.Visibility = ele.SelectedMode ? Visibility.Visible : Visibility.Collapsed;
		//}

		//Source
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageBox), new PropertyMetadata(null));
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetCurrentValue(SourceProperty, value); }
		}

		//ImgSize
		public static readonly DependencyProperty ImgSizeProperty = DependencyProperty.Register("ImgSize", typeof(double), typeof(ImageBox), new PropertyMetadata(0.0));
		public double ImgSize {
			get { return (double)GetValue(ImgSizeProperty); }
			set { SetCurrentValue(ImgSizeProperty, value); }
		}

		private void GrdMain_MouseEnter(object sender, MouseEventArgs e) {
			btnMini.Visibility = Visibility.Visible;
		}

		private void GrdMain_MouseLeave(object sender, MouseEventArgs e) {
			btnMini.Visibility = Visibility.Collapsed;
		}

		private void BtnMini_Click(object sender, RoutedEventArgs e) {
			RoutedEventArgs arg = new RoutedEventArgs(OnClickMiniModeProperty, this);
			RaiseEvent(arg);
		}

		//OnClickMiniMode
		public static readonly RoutedEvent OnClickMiniModeProperty = EventManager.RegisterRoutedEvent("OnClickMiniMode", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ImageBox));
		public event RoutedEventHandler OnClickMiniMode {
			//将路由事件添加路由事件处理程序
			add { AddHandler(OnClickMiniModeProperty, value); }
			//从路由事件处理程序中移除路由事件
			remove { RemoveHandler(OnClickMiniModeProperty, value); }
		}

		//private void GrdMain_MouseUp(object sender, MouseButtonEventArgs e) {
		//	if(!SelectedMode) {
		//		return;
		//	}
		//}

		//private void ChkSelect_Checked(object sender, RoutedEventArgs e) {
		//	updateSelected();
		//}

		//private void ChkSelect_Unchecked(object sender, RoutedEventArgs e) {
		//	updateSelected();
		//}

		//private void updateSelected() {
		//	if(!SelectedMode) {
		//		return;
		//	}

		//	RoutedEventArgs arg = new RoutedEventArgs(SelectedChangedProperty, this);
		//	RaiseEvent(arg);
		//}

	}
}
