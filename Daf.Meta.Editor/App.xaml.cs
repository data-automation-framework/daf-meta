// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;

namespace Daf.Meta.Editor
{
	public partial class App : Application
	{
		public App() : base()
		{
			ConfigureServices();

			InitializeComponent();
		}

		private static void ConfigureServices()
		{
			ServiceCollection services = new();
			services.AddSingleton<IMessageBoxService, MessageBoxService>().BuildServiceProvider();
			services.AddSingleton<IWindowService, WindowService>().BuildServiceProvider();

			ServiceProvider serviceProvider = services.BuildServiceProvider();

			Ioc.Default.ConfigureServices(serviceProvider);
		}
	}
}
