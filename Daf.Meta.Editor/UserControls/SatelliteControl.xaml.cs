// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor
{
	public partial class SatelliteControl : UserControl
	{
		public SatelliteControl()
		{
			InitializeComponent();

			WeakReferenceMessenger.Default.Register<SatelliteControl, StagingColumnsChanged>(this, (r, m) => RefreshColumnsNotInHubsOrLinks());

		}

		/// <summary>
		/// Refreshes the binding for the list of StagingColumns not associated with a Hub- or LinkRelationship.
		/// </summary>
		public void RefreshColumnsNotInHubsOrLinks()
		{
			SatelliteConnectionList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
		}
	}
}
