﻿using System;
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

namespace csharpHelp.ui {
	/// <summary>Button</summary>
	public class MiniButton : Button {
		static MiniButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MiniButton), new FrameworkPropertyMetadata(typeof(MiniButton)));
		}

		public MiniButton() {
			
		}

		//Radius
		public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(CornerRadius), typeof(MiniButton), new PropertyMetadata(new CornerRadius(0)));
		public CornerRadius Radius {
			get { return (CornerRadius)GetValue(RadiusProperty); }
			set { SetCurrentValue(RadiusProperty, value); }
		}

		//Content color
		public static readonly DependencyProperty OverColorProperty = DependencyProperty.Register("OverColor", typeof(Brush), typeof(MiniButton), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b8b8b8"))));
		public Brush OverColor {
			get { return (Brush)GetValue(OverColorProperty); }
			set { SetCurrentValue(OverColorProperty, value); }
		}

		//IsSelect
		public static readonly DependencyProperty IsSelectProperty = DependencyProperty.Register("IsSelect", typeof(bool), typeof(MiniButton), new PropertyMetadata(false));
		public bool IsSelect {
			get { return (bool)GetValue(IsSelectProperty); }
			set { SetCurrentValue(IsSelectProperty, value); }
		}

		//Select color
		public static readonly DependencyProperty SelectColorProperty = DependencyProperty.Register("SelectColor", typeof(Brush), typeof(MiniButton), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b8b8b8"))));
		public Brush SelectColor {
			get { return (Brush)GetValue(SelectColorProperty); }
			set { SetCurrentValue(SelectColorProperty, value); }
		}
	}
}
