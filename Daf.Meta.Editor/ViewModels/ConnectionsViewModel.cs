// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Daf.Meta.Layers;
using Daf.Meta.Layers.DataSources;

namespace Daf.Meta.Editor.ViewModels
{
	public class ConnectionsViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddConnectionCommand { get; }
		public RelayCommand DeleteConnectionCommand { get; }

		public ConnectionsViewModel()
		{
			Connections = null!;

			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AddConnectionCommand = new RelayCommand<Type?>(AddConnection);
			DeleteConnectionCommand = new RelayCommand(DeleteConnection);
		}

		public ConnectionsViewModel(ObservableCollection<Connection> connections)
		{
			Connections = connections;

			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AddConnectionCommand = new RelayCommand<Type?>(AddConnection);
			DeleteConnectionCommand = new RelayCommand(DeleteConnection);

			SelectedConnection = Connections.FirstOrDefault();
		}

		public ObservableCollection<Connection> Connections { get; }

		private Connection? _selectedConnection;
		public Connection? SelectedConnection
		{
			get { return _selectedConnection; }
			set
			{
				SetProperty(ref _selectedConnection, value);
			}
		}

		private void AddConnection(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			bool dialogResult = _windowService.ShowDialog(windowType, out object dataContext);

			if (dialogResult)
			{
				string name = ((AddConnectionViewModel)dataContext!).Name;

				// TODO: Discuss, is this preferable to the Activator pattern?
				//Connection connection;

				//switch (inputDialog.ConnectionType)
				//{
				//	case ConnectionType.OleDB:
				//		connection = new OleDBConnection(name: inputDialog.ConnectionName, connectionString: "ThisIsAConnectionstring") { ConnectionType = inputDialog.ConnectionType };
				//		break;
				//	case ConnectionType.MySql:
				//		connection = new MySqlConnection(name: inputDialog.ConnectionName, connectionString: "ThisIsAConnectionstring") { ConnectionType = inputDialog.ConnectionType };
				//		break;
				//	case ConnectionType.Rest:
				//		connection = new RestConnection(name: inputDialog.ConnectionName) { ConnectionType = inputDialog.ConnectionType };
				//		break;
				//	case ConnectionType.GraphQl:
				//		connection = new GraphQlConnection(name: inputDialog.ConnectionName) { ConnectionType = inputDialog.ConnectionType };
				//		break;
				//	default:
				//		throw new NotImplementedException($"Support for connection type {inputDialog.ConnectionType} is not implemented!");
				//}

				ConnectionType connectionType = ((AddConnectionViewModel)dataContext!).SelectedConnectionType;

				Assembly assembly = typeof(ConnectionType).Assembly;
				Type objectType = assembly.GetType($"Daf.Meta.Layers.Connections.{connectionType}Connection")!;

				bool hasConnectionString = objectType.GetProperty("ConnectionString") != null;

				Connection connection;

				if (hasConnectionString)
				{
					connection = (Connection)Activator.CreateInstance(objectType, name, "ThisIsAConnectionstring")!;
				}
				else
				{
					connection = (Connection)Activator.CreateInstance(objectType, name)!;
				}

				connection.ConnectionType = connectionType;

				// Add to view model
				Connections.AddSorted(connection);

				// Also add to base model?
				//MainWindow.Model.AddHub(hub);
			}
		}

		private void DeleteConnection()
		{
			if (SelectedConnection != null)
			{
				foreach (DataSource dataSource in Model.Instance.DataSources)
				{
					// TODO: This is bad, fix it. "ConnectionDataSource" interface?
					if (dataSource is SqlDataSource sqlDataSource)
					{
						if (sqlDataSource.Connection == SelectedConnection)
						{
							string msg = "At least one data source is using the selected connection. Remove all data sources using the connection and try again.";
							_mbService.Show(msg, "Connection in use", MessageBoxButton.OK, MessageBoxImage.Information);

							return;
						}
					}
					else if (dataSource is RestDataSource restDataSource)
					{
						if (restDataSource.Connection == SelectedConnection)
						{
							string msg = "At least one data source is using the selected connection. Remove all data sources using the connection and try again.";
							_mbService.Show(msg, "Connection in use", MessageBoxButton.OK, MessageBoxImage.Information);

							return;
						}
					}
					else if (dataSource is GraphQlDataSource graphQlDataSource)
					{
						if (graphQlDataSource.Connection == SelectedConnection)
						{
							string msg = "At least one data source is using the selected connection. Remove all data sources using the connection and try again.";
							_mbService.Show(msg, "Connection in use", MessageBoxButton.OK, MessageBoxImage.Information);

							return;
						}
					}
				}

				//MainWindow.Model.RemoveConnection(selectedConnection);
				Connections.Remove(SelectedConnection);
			}
		}
	}
}
