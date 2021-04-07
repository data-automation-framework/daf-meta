// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Jot;
using Daf.Meta.Editor.ViewModels;
using Daf.Meta.Editor.Windows;
using System.Windows;

namespace Daf.Meta.Editor
{
	internal static class Settings
	{
		// Expose the tracker instance.
		internal static Tracker Tracker = new();

		static Settings()
		{
			// Tell Jot how to track MainWindow objects.
			Tracker.Configure<MainWindow>()
				.Id(w => w.Name)
				.Properties(w => new { MainViewModel.MetadataPath })
				.PersistOn(nameof(Window.Closing))
				.StopTrackingOn(nameof(Window.Closing));
		}
	}
}
