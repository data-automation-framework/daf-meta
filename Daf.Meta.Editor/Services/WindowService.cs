// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Editor.Windows;
using System;
using System.Reflection;
using System.Windows;

namespace Daf.Meta.Editor
{
	public interface IWindowService
	{
		public void OpenWindow(Type windowType);
		public bool ShowDialog(Type windowType, out object viewModel); // For receiving a view model from the dialog.
		public bool ShowDialog(Type windowType, object viewModel); // For passing a view model to the dialog.
		public void SetMainWindowTitle(string title);
		public System.Windows.Forms.DialogResult OpenFile(out string path);
	}

	public class WindowService : IWindowService
	{
		public void OpenWindow(Type windowType)
		{
			Window window = CreateWindow(windowType);

			window!.Show();
		}

		public bool ShowDialog(Type windowType, out object viewModel)
		{
			// If there is a pre dialog action then invoke that.
			//if (_preOpenDialogAction != null)
			//	_preOpenDialogAction();

			Window window = CreateWindow(windowType);

			// Show the dialog.
			bool result = (bool)window.ShowDialog()!;

			// Invoke the post open dialog action.
			//_postOpenDialogAction(result, window.DataContext);

			viewModel = window.DataContext;
			return result;
		}

		public bool ShowDialog(Type windowType, object viewModel)
		{
			Window window = CreateWindow(windowType);
			window.DataContext = viewModel;

			// Show the dialog.
			bool result = (bool)window.ShowDialog()!;

			return result;
		}

		public void SetMainWindowTitle(string title)
		{
			MainWindow window = (MainWindow)Application.Current.MainWindow;

			window.Title = title;
		}

		public System.Windows.Forms.DialogResult OpenFile(out string path)
		{
			System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new();
			System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

			path = folderBrowserDialog.SelectedPath;

			folderBrowserDialog.Dispose(); // It appears FolderBrowserDialog needs to be disposed of manually.

			return result;
		}

		private static Window CreateWindow(Type windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException("TargetWindowType");

			// Get the type.
			TypeInfo typeInfo = (TypeInfo)windowType;

			if (typeInfo.BaseType != typeof(Window))
				throw new InvalidOperationException("parameter is not a Window type");

			// Create the window.
			Window? window = Activator.CreateInstance(typeInfo) as Window;

			return window!;
		}
	}
}
