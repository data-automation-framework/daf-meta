// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PropertyTools.Wpf;

namespace Daf.Meta.Editor
{
	public partial class GeneralControl : UserControl
	{
		public GeneralControl()
		{
			InitializeComponent();
		}

		private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (sender is PropertyGrid && !e.Handled)
			{
				e.Handled = false;

				MouseWheelEventArgs eventArg = new(e.MouseDevice, e.Timestamp, e.Delta)
				{
					RoutedEvent = MouseWheelEvent,
					Source = sender
				};

				UIElement parent = (UIElement)((Control)sender).Parent;

				parent.RaiseEvent(eventArg);
			}
		}
	}
}
