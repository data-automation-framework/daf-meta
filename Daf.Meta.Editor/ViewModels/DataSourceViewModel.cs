﻿// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel.DataAnnotations;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public abstract class DataSourceViewModel : ObservableValidator
	{
		protected DataSourceViewModel(DataSource dataSource)
		{
			DataSource = dataSource;
			//HubRelationships.CollectionChanged += HubRelationshipsChanged;
			//LinkRelationships.CollectionChanged += LinkRelationshipsChanged;
		}
		public virtual DataSource DataSource { get; }

		[Category("General")]
		[Description("The name of the data source")]
		public string Name
		{
			get => DataSource.Name;
			set
			{
				SetProperty(DataSource.Name, value, DataSource, (dataSource, name) => dataSource.Name = name, true);

				//QualifiedName = string.Empty; // Update QualifiedName's bindings without changing its value.
				//TenantName = string.Empty; // Update TenantName's bindings without changing its value.
			}
		}

		[Category("General")]
		[ItemsSourceProperty("SourceSystems")]
		[DisplayMemberPath("Name")]
		public SourceSystem SourceSystem
		{
			get => DataSource.SourceSystem;
			set
			{
				SetProperty(DataSource.SourceSystem, value, DataSource, (dataSource, sourceSystem) => dataSource.SourceSystem = sourceSystem, true);
			}
		}

		[Category("General")]
		[ItemsSourceProperty("Tenants")]
		[DisplayMemberPath("Name")]
		public Tenant Tenant
		{
			get => DataSource.Tenant;
			set
			{
				SetProperty(DataSource.Tenant, value, DataSource, (dataSource, tenant) => dataSource.Tenant = tenant, true);
			}
		}

		[Category("General")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		public DestinationType DestinationType
		{
			get => DataSource.DestinationType;
			set
			{
				SetProperty(DataSource.DestinationType, value, DataSource, (dataSource, destinationType) => dataSource.DestinationType = destinationType, true);
			}
		}

		[Category("General")]
		public LoadWidth DefaultLoadWidth
		{
			get => DataSource.DefaultLoadWidth;
			set
			{
				SetProperty(DataSource.DefaultLoadWidth, value, DataSource, (dataSource, defaultLoadWidth) => dataSource.DefaultLoadWidth = defaultLoadWidth, true);
			}
		}

		[Category("General")]
		public bool GenerateLatestViews
		{
			get => DataSource.GenerateLatestViews;
			set
			{
				SetProperty(DataSource.GenerateLatestViews, value, DataSource, (dataSource, generateLatestViews) => dataSource.GenerateLatestViews = generateLatestViews, true);
			}
		}

		[Category("General")]
		public bool? ContainsMultiStructuredJson
		{
			get => DataSource.ContainsMultiStructuredJson;
			set
			{
				SetProperty(DataSource.ContainsMultiStructuredJson, value, DataSource, (dataSource, containsMultiStructuredJson) => dataSource.ContainsMultiStructuredJson = containsMultiStructuredJson, true);
			}
		}

		[Category("General")]
		public string? FileName
		{
			get => DataSource.FileName;
			set
			{
				SetProperty(DataSource.FileName, value, DataSource, (dataSource, fileName) => dataSource.FileName = fileName, true);
			}
		}

		[Category("General")]
		public string? IncrementalStagingColumn
		{
			get => IncrementalStagingColumn;
			set
			{
				SetProperty(DataSource.IncrementalStagingColumn, value, DataSource, (dataSource, incrementalStagingColumn) => dataSource.IncrementalStagingColumn = incrementalStagingColumn, true);
			}
		}

		[DataType(DataType.MultilineText)]
		[Category("General")]
		public string? IncrementalQuery
		{
			get => DataSource.IncrementalQuery;
			set
			{
				SetProperty(DataSource.IncrementalQuery, value, DataSource, (dataSource, incrementalQuery) => dataSource.IncrementalQuery = incrementalQuery, true);
			}
		}

		[Category("General")]
		public string? BusinessDateColumn
		{
			get => DataSource.BusinessDateColumn;
			set
			{
				SetProperty(DataSource.BusinessDateColumn, value, DataSource, (dataSource, businessDateColumn) => dataSource.BusinessDateColumn = businessDateColumn, true);
			}
		}

		[Category("General")]
		[Description("The custom select query that is run against the load table when loading the staging table.")]
		public string? SqlSelectQuery
		{
			get => DataSource.SqlSelectQuery;
			set
			{
				SetProperty(DataSource.SqlSelectQuery, value, DataSource, (dataSource, sqlSelectQuery) => dataSource.SqlSelectQuery = sqlSelectQuery, true);
			}
		}

		[Category("Azure")]
		public string? AzureLinkedServiceReference
		{
			get => DataSource.AzureLinkedServiceReference;
			set
			{
				SetProperty(DataSource.AzureLinkedServiceReference, value, DataSource, (dataSource, azureLinkedServiceReference) => dataSource.AzureLinkedServiceReference = azureLinkedServiceReference, true);
			}
		}

		[Category("General")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		public Build Build
		{
			get => DataSource.Build;
			set
			{
				SetProperty(DataSource.Build, value, DataSource, (dataSource, build) => dataSource.Build = build, true);
			}
		}

		[Category("General")]
		public string? ErrorHandling
		{
			get => DataSource.ErrorHandling;
			set
			{
				SetProperty(DataSource.ErrorHandling, value, DataSource, (dataSource, errorHandling) => dataSource.ErrorHandling = errorHandling, true);
			}
		}

		// TODO: This is necessary but should be a property on SatellitesViewModel instead.
		[Browsable(false)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "<Pending>")]
		public BusinessKey? BusinessKey
		{
			get => DataSource.BusinessKey;
			set
			{
				SetProperty(DataSource.BusinessKey, value, DataSource, (dataSource, businessKey) => dataSource.BusinessKey = businessKey, true);
			}
		}
	}
}
