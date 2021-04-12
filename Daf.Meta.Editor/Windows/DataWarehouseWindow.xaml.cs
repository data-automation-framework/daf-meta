// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Daf.Meta.Editor.Windows
{
	public partial class DataWarehouseWindow : Window
	{
		public DataWarehouseWindow()
		{
			InitializeComponent();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}

	public class DictionaryValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((KeyValuePair<string, Connection>)value).Value;
		}
	}
}
