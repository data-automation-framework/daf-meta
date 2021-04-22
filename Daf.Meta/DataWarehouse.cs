// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using Daf.Meta.Layers.Connections;

namespace Daf.Meta
{
	public class DataWarehouse : PropertyChangedBaseClass
	{
		public DataWarehouse(string targetPlatform, OleDBConnection prodStagingDatabase, OleDBConnection prodDataVaultDatabase, OleDBConnection prodMartDatabase, OleDBConnection devStagingDatabase, OleDBConnection devDataVaultDatabase, OleDBConnection devMartDatabase)
		{
			_targetPlatform = targetPlatform;

			ProdStagingDatabase = prodStagingDatabase;
			ProdDataVaultDatabase = prodDataVaultDatabase;
			ProdMartDatabase = prodMartDatabase;
			DevStagingDatabase = devStagingDatabase;
			DevDataVaultDatabase = devDataVaultDatabase;
			DevMartDatabase = devMartDatabase;
		}

		private string _targetPlatform; // Azure, Snowflake, SqlServer

		public string TargetPlatform
		{
			get { return _targetPlatform; }
			set
			{
				if (_targetPlatform != value)
				{
					_targetPlatform = value;

					NotifyPropertyChanged("TargetPlatform");
				}
			}
		}

		private string? _targetPlatformVersion; // 2016, 2017, 2019, empty string (for platforms other than SqlServer)

		public string? TargetPlatformVersion
		{
			get { return _targetPlatformVersion; }
			set
			{
				if (_targetPlatformVersion != value)
				{
					_targetPlatformVersion = value;

					NotifyPropertyChanged("TargetPlatformVersion");
				}
			}
		}

		public string TargetPlatformAssemblyVersion
		{
			get
			{
				if (TargetPlatform == "SqlServer")
				{
					int assemblyVersion = (int)Enum.Parse(typeof(SqlServerVersion), TargetPlatform + TargetPlatformVersion);

					return assemblyVersion.ToString(CultureInfo.InvariantCulture);
				}

				return "";
			}
		}

		private bool _singleDatabase;

		public bool SingleDatabase
		{
			get { return _singleDatabase; }
			set
			{
				if (_singleDatabase != value)
				{
					_singleDatabase = value;

					NotifyPropertyChanged("SingleDatabase");
				}
			}
		}

		private bool _columnStore;

		public bool ColumnStore
		{
			get { return _columnStore; }
			set
			{
				if (_columnStore != value)
				{
					_columnStore = value;

					NotifyPropertyChanged("ColumnStore");
				}
			}
		}

		private bool _useNewHashLogic;

		public bool UseNewHashLogic
		{
			get { return _useNewHashLogic; }
			set
			{
				if (_useNewHashLogic != value)
				{
					_useNewHashLogic = value;

					NotifyPropertyChanged("UseNewHashLogic");
				}
			}
		}

		//[JsonIgnore]
		//public List<OleDBConnection> Connections
		//{
		//	get
		//	{
		//		List<OleDBConnection> connections = new List<OleDBConnection>
		//		{
		//			ProdStagingDatabase,
		//			ProdDataVaultDatabase,
		//			ProdMartDatabase,
		//			DevStagingDatabase,
		//			DevDataVaultDatabase,
		//			DevMartDatabase
		//		};

		//		return connections;
		//	}
		//}

		public OleDBConnection ProdStagingDatabase { get; set; }

		public OleDBConnection ProdDataVaultDatabase { get; set; }

		public OleDBConnection ProdMartDatabase { get; set; }

		public OleDBConnection DevStagingDatabase { get; set; }

		public OleDBConnection DevDataVaultDatabase { get; set; }

		public OleDBConnection DevMartDatabase { get; set; }

		private string? _azureStorageAccount;

		public string? AzureStorageAccount
		{
			get { return _azureStorageAccount; }
			set
			{
				if (_azureStorageAccount != value)
				{
					_azureStorageAccount = value;

					NotifyPropertyChanged("AzureStorageAccount");
				}
			}
		}

		private string? _azureStorageAccountFriendlyName;

		public string? AzureStorageAccountFriendlyName
		{
			get { return _azureStorageAccountFriendlyName; }
			set
			{
				if (_azureStorageAccountFriendlyName != value)
				{
					_azureStorageAccountFriendlyName = value;

					NotifyPropertyChanged("AzureStorageAccountFriendlyName");
				}
			}
		}

		private string? _azureStorageAccountKey;

		public string? AzureStorageAccountKey
		{
			get { return _azureStorageAccountKey; }
			set
			{
				if (_azureStorageAccountKey != value)
				{
					_azureStorageAccountKey = value;

					NotifyPropertyChanged("AzureStorageAccountKey");
				}
			}
		}

		private string? _azureBlobContainer;

		public string? AzureBlobContainer
		{
			get { return _azureBlobContainer; }
			set
			{
				if (_azureBlobContainer != value)
				{
					_azureBlobContainer = value;

					NotifyPropertyChanged("AzureBlobContainer");
				}
			}
		}

		private string? _azureResourceGroup;

		public string? AzureResourceGroup
		{
			get { return _azureResourceGroup; }
			set
			{
				if (_azureResourceGroup != value)
				{
					_azureResourceGroup = value;

					NotifyPropertyChanged("AzureResourceGroup");
				}
			}
		}

		private string? _azureDataFactoryName;

		public string? AzureDataFactoryName
		{
			get { return _azureDataFactoryName; }
			set
			{
				if (_azureDataFactoryName != value)
				{
					_azureDataFactoryName = value;

					NotifyPropertyChanged("AzureDataFactoryName");
				}
			}
		}

		private string? _azureTenantId;

		public string? AzureTenantId
		{
			get { return _azureTenantId; }
			set
			{
				if (_azureTenantId != value)
				{
					_azureTenantId = value;

					NotifyPropertyChanged("AzureTenantId");
				}
			}
		}

		private string? _azureApplicationId;

		public string? AzureApplicationId
		{
			get { return _azureApplicationId; }
			set
			{
				if (_azureApplicationId != value)
				{
					_azureApplicationId = value;

					NotifyPropertyChanged("AzureApplicationId");
				}
			}
		}

		private string? _azureAuthenticationKey;

		public string? AzureAuthenticationKey
		{
			get { return _azureAuthenticationKey; }
			set
			{
				if (_azureAuthenticationKey != value)
				{
					_azureAuthenticationKey = value;

					NotifyPropertyChanged("AzureAuthenticationKey");
				}
			}
		}

		private string? _azureSubscriptionId;

		public string? AzureSubscriptionId
		{
			get { return _azureSubscriptionId; }
			set
			{
				if (_azureSubscriptionId != value)
				{
					_azureSubscriptionId = value;

					NotifyPropertyChanged("AzureSubscriptionId");
				}
			}
		}

		private string? _azureMainRunnerCode;

		public string? AzureMainRunnerCode
		{
			get { return _azureMainRunnerCode; }
			set
			{
				if (_azureMainRunnerCode != value)
				{
					_azureMainRunnerCode = value;

					NotifyPropertyChanged("AzureMainRunnerCode");
				}
			}
		}
	}
}
