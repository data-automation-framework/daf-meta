// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Daf.Meta.Editor.ViewModels;

namespace Daf.Meta.Editor.Tests
{
	/// <summary>
	/// Sets up services and MetadataPath to be used for all tests. Services need to be configured for the test class
	/// because ordinarily services are injected in App.Xaml.cs which is never run wihle testing.
	/// </summary>
	public class TestFixture : IDisposable
	{
		public TestFixture()
		{
			ConfigureServices();
			// Ensures every new instance of MainViewModel will call Model.Initialize rather than Model.Deserialize at construction.
			//App app = new();
			//app.Run();

			MainViewModel.MetadataPath = null;
		}

		private static void ConfigureServices()
		{
			ServiceCollection services = new();
			services.AddSingleton<IMessageBoxService, MessageBoxService>().BuildServiceProvider();
			services.AddSingleton<IWindowService, WindowService>().BuildServiceProvider();

			ServiceProvider serviceProvider = services.BuildServiceProvider();

			Ioc.Default.ConfigureServices(serviceProvider);
		}

		public void Dispose()
		{
			// clean up test data

			// Not sure yet how to do this. May be important if we have several test-classes.
		}
	}
}
