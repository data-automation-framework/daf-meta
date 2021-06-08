// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor
{
	public partial class LinkRelationshipControl : UserControl
	{
		public LinkRelationshipControl()
		{
			InitializeComponent();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// This event is called both when a StagingColumn is added/removed from StagingControl AND
			// when the user changes the selection in the User Control. It causes binding errors
			// when StagingColumns are removed, but works as intended when the user manually changes
			// the ComboBox.SelectedItem.

			if (e.AddedItems.Count == 0) // Case when the SelectedItem switches to null, as the previously bound object is removed from the collection.
				return;
			else
				// This should update the list of AvailableColumns in SatelliteControl only.
				// The list of all StagingColumns does not need to be updated.
				WeakReferenceMessenger.Default.Send(new HubLinkRelationshipChanged());
		}
	}
}
