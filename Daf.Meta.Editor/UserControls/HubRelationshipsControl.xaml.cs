// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor
{
	public partial class HubRelationshipControl : UserControl
	{
		public HubRelationshipControl()
		{
			InitializeComponent();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			WeakReferenceMessenger.Default.Send(new StagingColumnsChanged());
		}
	}
}
