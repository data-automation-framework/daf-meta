// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Editor.ViewModels;
using Daf.Meta.Editor.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Daf.Meta.Editor
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			IServiceProvider serviceProvider = CreateServiceProvider();

			Window window = new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<MainViewModel>()
			};

			window.Show();

			base.OnStartup(e);
		}

		private static IServiceProvider CreateServiceProvider()
		{
			ServiceCollection services = new();
			services.AddSingleton<IMessageBoxService, MessageBoxService>();
			services.AddSingleton<IWindowService, WindowService>();

			services.AddScoped<MainViewModel>();
			services.AddScoped<HubsViewModel>();
			services.AddScoped<LinksViewModel>();
			services.AddScoped<GeneralViewModel>();
			services.AddScoped<LoadViewModel>();
			services.AddScoped<StagingViewModel>();
			services.AddScoped<HubRelationshipViewModel>();
			services.AddScoped<LinkRelationshipViewModel>();
			services.AddScoped<SatelliteViewModel>();
			services.AddScoped<TenantsViewModel>();
			services.AddScoped<SourceSystemsViewModel>();
			services.AddScoped<ConnectionsViewModel>();

			return services.BuildServiceProvider();
		}
	}
}
