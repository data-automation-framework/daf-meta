// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows;

namespace Daf.Meta.Editor.Windows
{
	public partial class ConnectionsWindow : Window
	{
		public ConnectionsWindow()
		{
			InitializeComponent();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
