// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Daf.Meta.Editor.ViewModels;
using PropertyTools.Wpf;

namespace Daf.Meta.Editor
{
	public partial class LoadControl : UserControl
	{
		public LoadControl()
		{
			InitializeComponent();
		}

		private void LoadColumnsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LoadViewModel viewModel = (LoadViewModel)DataContext;

			List<ColumnViewModel>? columns = columnsListNew.SelectedItems.Cast<ColumnViewModel>().ToList();

			viewModel.SelectedColumns = columns;
			viewModel.SelectedColumn = columns.FirstOrDefault();
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

		private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			columnsListNew.SelectedItems.Clear();

			ListBoxItem listBoxItem = NavigationHelper.GetListBoxItem(sender);
			listBoxItem.IsSelected = true;
		}

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key is Key.Down or Key.Up)
			{
				// Get the index of the currently selected Textbox, from the ListBoxItem.
				ListBoxItem listBoxItem = NavigationHelper.GetListBoxItem(sender);
				int itemIndex = columnsListNew.ItemContainerGenerator.IndexFromContainer(listBoxItem);

				if (e.Key == Key.Down)
				{
					if (itemIndex == columnsListNew.Items.Count - 1)
					{
						e.Handled = true;
						return;
					}

					// Index incremented for Key.Down to correspond with the ListBox row that is further down.
					itemIndex++;
				}
				else // If Key == Key.Up
				{
					if (itemIndex == 0)
					{
						e.Handled = true;
						return;
					}

					// Index decremented for Key.Up to correspond with the ListBox row that is higher up.
					itemIndex--;
				}

				// Generate new ListBoxItem corresponding to the new index.
				ListBoxItem listBoxItem2 = (ListBoxItem)columnsListNew.ItemContainerGenerator.ContainerFromIndex(itemIndex);

				// Select the new ListBoxItem as the current selected item and set the properties of its child TextBox.
				listBoxItem2.IsSelected = true;
				TextBox textBox = NavigationHelper.GetTextBox(listBoxItem2);
				textBox.Focus();
				textBox.SelectAll();
				e.Handled = true;
			}
		}
	}
}
