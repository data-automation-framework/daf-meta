// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Daf.Meta.Editor
{
	public static class NavigationHelper
	{
		/// <summary>
		/// Gets the ListBoxItem ancestor of any Control
		/// </summary>
		/// <param name="obj">Any control (Button, TextBox, etc) that has a ListBoxItem as an ancestor in its visual tree.</param>
		/// <returns>The ListBoxItem ancestor.</returns>
		public static ListBoxItem GetListBoxItem(object obj)
		{
			DependencyObject item = (DependencyObject)obj;

			while (item is not ListBoxItem)
			{
				// Walks the visual tree until ListBoxItem is found.
				item = VisualTreeHelper.GetParent(item);
			}

			return (ListBoxItem)item;
		}

		/// <summary>
		/// Gets TextBox control of a ListBoxItem if it contains one. May not work in all cases, see method definition.
		/// </summary>
		/// <param name="listBoxItem">The ListBoxItem</param>
		/// <returns>The first textbox child that is found inside.</returns>
		public static TextBox GetTextBox(ListBoxItem listBoxItem)
		{
			// This returns the first TextBox control within the ListBoxItem, but does not account for the visual tree forking.
			DependencyObject child = listBoxItem;

			while (child is not TextBox)
			{
				int childCount = VisualTreeHelper.GetChildrenCount(child);

				for (int i = 0; i < childCount; i++)
				{
					DependencyObject childControl = VisualTreeHelper.GetChild(child, i);

					if (childControl is TextBox box)
						return box;
				}
				child = VisualTreeHelper.GetChild(child, 0);
			}

			throw new InvalidOperationException("ListBoxItem does not contain TextBox item.");
		}
	}
}
