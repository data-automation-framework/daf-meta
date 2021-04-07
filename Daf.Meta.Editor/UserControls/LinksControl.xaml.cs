// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Daf.Meta.Editor.ViewModels;

namespace Daf.Meta.Editor
{
	public partial class LinksControl : UserControl
	{
		public LinksControl()
		{
			InitializeComponent();
		}

		private void LinkColumnsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LinksViewModel viewModel = (LinksViewModel)DataContext;

			List<BusinessKeyViewModel>? columns = columnsListNew.SelectedItems.Cast<BusinessKeyViewModel>().ToList();

			viewModel.SelectedLinkColumns = columns;
			viewModel.SelectedLinkColumn = columns.FirstOrDefault();
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

		// The following function is necessary because setting IsTabStop=false in the ListView disables arrowkey navigation as well.
		// This provides a manual workaround.
		private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key is Key.Down or Key.Up)
			{
				// Get the index of the currently selected ListViewItem.
				int itemIndex = linkList.SelectedIndex;

				if (e.Key == Key.Down)
				{
					if (itemIndex == linkList.Items.Count - 1)
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

				linkList.SelectedIndex = itemIndex;
				e.Handled = true;
			}
		}
	}
}
