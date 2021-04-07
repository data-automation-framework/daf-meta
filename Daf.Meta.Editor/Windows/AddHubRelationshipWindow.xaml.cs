// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows;

namespace Daf.Meta.Editor.Windows
{
	public partial class AddHubRelationshipWindow : Window
	{
		public AddHubRelationshipWindow()
		{
			InitializeComponent();
		}

		private void btnDialogOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
