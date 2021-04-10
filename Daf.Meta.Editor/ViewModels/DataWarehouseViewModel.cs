// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class DataWarehouseViewModel : ObservableValidator
	{
		public DataWarehouseViewModel()
		{
			DataWarehouse = null!;
		}

		public DataWarehouseViewModel(DataWarehouse dataWarehouse)
		{
			DataWarehouse = dataWarehouse;
		}

		private DataWarehouse DataWarehouse { get; }

		// Azure, Snowflake, SqlServer
		[Category("General")]
		public string TargetPlatform
		{
			get { return DataWarehouse.TargetPlatform; }
			set
			{
				SetProperty(DataWarehouse.TargetPlatform, value, DataWarehouse, (dataWarehouse, targetPlatform) => DataWarehouse.TargetPlatform = targetPlatform, true);
			}
		}

		// 2016, 2017, 2019, empty string (for platforms other than SqlServer)
		public string? TargetPlatformVersion
		{
			get { return DataWarehouse.TargetPlatformVersion; }
			set
			{
				SetProperty(DataWarehouse.TargetPlatformVersion, value, DataWarehouse, (dataWarehouse, targetPlatformVersion) => dataWarehouse.TargetPlatformVersion = targetPlatformVersion, true);
			}
		}

		public bool SingleDatabase
		{
			get { return DataWarehouse.SingleDatabase; }
			set
			{
				SetProperty(DataWarehouse.SingleDatabase, value, DataWarehouse, (dataWarehouse, singleDatabase) => dataWarehouse.SingleDatabase = singleDatabase, true);
			}
		}

		public bool ColumnStore
		{
			get { return DataWarehouse.ColumnStore; }
			set
			{
				SetProperty(DataWarehouse.ColumnStore, value, DataWarehouse, (dataWarehouse, columnStore) => dataWarehouse.ColumnStore = columnStore, true);
			}
		}

		public bool UseNewHashLogic
		{
			get { return DataWarehouse.UseNewHashLogic; }
			set
			{
				SetProperty(DataWarehouse.UseNewHashLogic, value, DataWarehouse, (dataWarehouse, useNewHashLogic) => dataWarehouse.UseNewHashLogic = useNewHashLogic, true);
			}
		}

		[Category("Staging Database (Production)")]
		[DisplayName("Name")]
		public string ProdStagingDatabaseName
		{
			get { return DataWarehouse.ProdStagingDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.ProdStagingDatabase.Name, value, DataWarehouse, (dataWarehouse, prodStagingDatabaseName) => dataWarehouse.ProdStagingDatabase.Name = prodStagingDatabaseName, true);
			}
		}

		[Category("Staging Database (Production)")]
		[DisplayName("Connection string")]
		public string ProdStagingDatabaseConnectionString
		{
			get { return DataWarehouse.ProdStagingDatabase.ConnectionString; }
			set
			{
				SetProperty(DataWarehouse.ProdStagingDatabase.ConnectionString, value, DataWarehouse, (dataWarehouse, prodStagingDatabaseConnectionString) => dataWarehouse.ProdStagingDatabase.ConnectionString = prodStagingDatabaseConnectionString, true);
			}
		}

		[Category("Data Vault Database (Production)")]
		[DisplayName("Name")]
		public string ProdDataVaultDatabaseName
		{
			get { return DataWarehouse.ProdDataVaultDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.ProdDataVaultDatabase.Name, value, DataWarehouse, (dataWarehouse, prodDataVaultDatabaseName) => dataWarehouse.ProdDataVaultDatabase.Name = prodDataVaultDatabaseName, true);
			}
		}

		[Category("Data Vault Database (Production)")]
		[DisplayName("Connection string")]
		public string ProdDataVaultDatabaseConnectionString
		{
			get { return DataWarehouse.ProdDataVaultDatabase.ConnectionString; }
			set
			{
				SetProperty(DataWarehouse.ProdDataVaultDatabase.ConnectionString, value, DataWarehouse, (dataWarehouse, prodDataVaultDatabaseConnectionString) => dataWarehouse.ProdDataVaultDatabase.ConnectionString = prodDataVaultDatabaseConnectionString, true);
			}
		}

		[Category("Mart Database (Production)")]
		[DisplayName("Name")]
		public string ProdMartDatabaseName
		{
			get { return DataWarehouse.ProdMartDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.ProdMartDatabase.Name, value, DataWarehouse, (dataWarehouse, prodMartDatabaseName) => dataWarehouse.ProdMartDatabase.Name = prodMartDatabaseName, true);
			}
		}

		[Category("Mart Database (Production)")]
		[DisplayName("Connection string")]
		public string ProdMartDatabaseConnectionString
		{
			get { return DataWarehouse.ProdMartDatabase.ConnectionString; }
			set
			{
				SetProperty(DataWarehouse.ProdMartDatabase.ConnectionString, value, DataWarehouse, (dataWarehouse, prodMartDatabaseConnectionString) => dataWarehouse.ProdMartDatabase.ConnectionString = prodMartDatabaseConnectionString, true);
			}
		}

		[Category("Staging Database (Development)")]
		[DisplayName("Name")]
		public string DevStagingDatabaseName
		{
			get { return DataWarehouse.DevStagingDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.DevStagingDatabase.Name, value, DataWarehouse, (dataWarehouse, devStagingDatabaseName) => dataWarehouse.DevStagingDatabase.Name = devStagingDatabaseName, true);
			}
		}

		[Category("Staging Database (Development)")]
		[DisplayName("Connection string")]
		public string DevStagingDatabaseConnectionString
		{
			get { return DataWarehouse.DevStagingDatabase.ConnectionString; }
			set
			{
				SetProperty(DataWarehouse.DevStagingDatabase.ConnectionString, value, DataWarehouse, (dataWarehouse, devStagingDatabaseConnectionString) => dataWarehouse.DevStagingDatabase.ConnectionString = devStagingDatabaseConnectionString, true);
			}
		}

		[Category("Data Vault Database (Development)")]
		[DisplayName("Name")]
		public string DevDataVaultDatabaseName
		{
			get { return DataWarehouse.DevDataVaultDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.DevDataVaultDatabase.Name, value, DataWarehouse, (dataWarehouse, devDataVaultDatabaseName) => dataWarehouse.DevDataVaultDatabase.Name = devDataVaultDatabaseName, true);
			}
		}

		[Category("Data Vault Database (Development)")]
		[DisplayName("Connection string")]
		public string DevDataVaultDatabaseConnectionString
		{
			get { return DataWarehouse.DevDataVaultDatabase.ConnectionString; }
			set
			{
				SetProperty(DataWarehouse.DevDataVaultDatabase.ConnectionString, value, DataWarehouse, (dataWarehouse, devDataVaultDatabaseConnectionString) => dataWarehouse.DevDataVaultDatabase.ConnectionString = devDataVaultDatabaseConnectionString, true);
			}
		}

		[Category("Mart Database (Development)")]
		[DisplayName("Name")]
		public string DevMartDatabaseName
		{
			get { return DataWarehouse.DevMartDatabase.Name; }
			set
			{
				SetProperty(DataWarehouse.DevMartDatabase.Name, value, DataWarehouse, (dataWarehouse, devMartDatabaseName) => dataWarehouse.DevMartDatabase.Name = devMartDatabaseName, true);
			}
		}

		[Category("Azure")]
		public string? AzureStorageAccount
		{
			get { return DataWarehouse.AzureStorageAccount; }
			set
			{
				SetProperty(DataWarehouse.AzureStorageAccount, value, DataWarehouse, (dataWarehouse, azureStorageAccount) => dataWarehouse.AzureStorageAccount = azureStorageAccount, true);
			}
		}

		[Category("Azure")]
		public string? AzureStorageAccountFriendlyName
		{
			get { return DataWarehouse.AzureStorageAccountFriendlyName; }
			set
			{
				SetProperty(DataWarehouse.AzureStorageAccountFriendlyName, value, DataWarehouse, (dataWarehouse, azureStorageAccountFriendlyName) => dataWarehouse.AzureStorageAccountFriendlyName = azureStorageAccountFriendlyName, true);
			}
		}

		[Category("Azure")]
		public string? AzureStorageAccountKey
		{
			get { return DataWarehouse.AzureStorageAccountKey; }
			set
			{
				SetProperty(DataWarehouse.AzureStorageAccountKey, value, DataWarehouse, (dataWarehouse, azureStorageAccountKey) => dataWarehouse.AzureStorageAccountKey = azureStorageAccountKey, true);
			}
		}

		public string? AzureBlobContainer
		{
			get { return DataWarehouse.AzureBlobContainer; }
			set
			{
				SetProperty(DataWarehouse.AzureBlobContainer, value, DataWarehouse, (dataWarehouse, azureBlobContainer) => dataWarehouse.AzureBlobContainer = azureBlobContainer, true);
			}
		}

		public string? AzureResourceGroup
		{
			get { return DataWarehouse.AzureResourceGroup; }
			set
			{
				SetProperty(DataWarehouse.AzureResourceGroup, value, DataWarehouse, (dataWarehouse, azureResourceGroup) => dataWarehouse.AzureResourceGroup = azureResourceGroup, true);
			}
		}

		public string? AzureDataFactoryName
		{
			get { return DataWarehouse.AzureDataFactoryName; }
			set
			{
				SetProperty(DataWarehouse.AzureDataFactoryName, value, DataWarehouse, (dataWarehouse, azureDataFactoryName) => dataWarehouse.AzureDataFactoryName = azureDataFactoryName, true);
			}
		}

		public string? AzureTenantId
		{
			get { return DataWarehouse.AzureTenantId; }
			set
			{
				SetProperty(DataWarehouse.AzureTenantId, value, DataWarehouse, (dataWarehouse, azureTenantId) => dataWarehouse.AzureTenantId = azureTenantId, true);
			}
		}

		public string? AzureApplicationId
		{
			get { return DataWarehouse.AzureApplicationId; }
			set
			{
				SetProperty(DataWarehouse.AzureApplicationId, value, DataWarehouse, (dataWarehouse, azureApplicationId) => dataWarehouse.AzureApplicationId = azureApplicationId, true);
			}
		}

		public string? AzureAuthenticationKey
		{
			get { return DataWarehouse.AzureAuthenticationKey; }
			set
			{
				SetProperty(DataWarehouse.AzureAuthenticationKey, value, DataWarehouse, (dataWarehouse, azureAuthenticationKey) => dataWarehouse.AzureAuthenticationKey = azureAuthenticationKey, true);
			}
		}

		public string? AzureSubscriptionId
		{
			get { return DataWarehouse.AzureSubscriptionId; }
			set
			{
				SetProperty(DataWarehouse.AzureSubscriptionId, value, DataWarehouse, (dataWarehouse, azureSubscriptionId) => dataWarehouse.AzureSubscriptionId = azureSubscriptionId, true);
			}
		}

		public string? AzureMainRunnerCode
		{
			get { return DataWarehouse.AzureMainRunnerCode; }
			set
			{
				SetProperty(DataWarehouse.AzureMainRunnerCode, value, DataWarehouse, (dataWarehouse, azureMainRunnerCode) => dataWarehouse.AzureMainRunnerCode = azureMainRunnerCode, true);
			}
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
