// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Windows;

namespace Daf.Meta.Editor.Windows
{
	public partial class AddConnectionWindow : Window
	{
		public AddConnectionWindow()
		{
			InitializeComponent();
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			nameText.SelectAll();
			nameText.Focus();
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
