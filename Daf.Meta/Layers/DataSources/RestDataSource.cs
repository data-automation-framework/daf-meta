// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;
using Daf.Meta.Interfaces;
using Daf.Meta.JsonConverters;
using Daf.Meta.Layers.Connections;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Layers.DataSources
{
	[JsonDiscriminator("Rest")]
	public class RestDataSource : DataSource, IConnection
	{
		public RestDataSource(string name, RestConnection connection, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
			_connection = connection;
		}

		[Browsable(false)]
		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<Connection> Connections { get { return Model.Instance.Connections; } }

		private RestConnection _connection;

		[Category("REST")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[ItemsSourceProperty("Connections")]
		[DisplayMemberPath("Name")]
		[SortIndex(100)]
		[JsonConverter(typeof(RestConnectionConverter))]
		public RestConnection Connection
		{
			get { return _connection; }
			set
			{
				if (_connection != value)
				{
					_connection = value;

					NotifyPropertyChanged("Connection");
				}
			}
		}

		private uint _connectionRetryAttempts;

		[Category("REST")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryAttempts
		{
			get { return _connectionRetryAttempts; }
			set
			{
				if (_connectionRetryAttempts != value)
				{
					_connectionRetryAttempts = value;

					NotifyPropertyChanged("ConnectionRetryAttempts");
				}
			}
		}

		private uint _connectionRetryMinutes;

		[Category("REST")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryMinutes
		{
			get { return _connectionRetryMinutes; }
			set
			{
				if (_connectionRetryMinutes != value)
				{
					_connectionRetryMinutes = value;

					NotifyPropertyChanged("ConnectionRetryMinutes");
				}
			}
		}

		private string? _relativeUrl;

		[Category("REST")]
		[SortIndex(100)]
		public string? RelativeUrl
		{
			get { return _relativeUrl; }
			set
			{
				if (_relativeUrl != value)
				{
					_relativeUrl = value;

					NotifyPropertyChanged("RelativeUrl");
				}
			}
		}

		private bool _saveCookie;

		[Category("REST")]
		[SortIndex(100)]
		public bool SaveCookie
		{
			get { return _saveCookie; }
			set
			{
				if (_saveCookie != value)
				{
					_saveCookie = value;

					NotifyPropertyChanged("SaveCookie");
				}
			}
		}

		private string? _customScriptPath;

		[Category("REST")]
		[SortIndex(100)]
		public string? CustomScriptPath
		{
			get { return _customScriptPath; }
			set
			{
				if (_customScriptPath != value)
				{
					_customScriptPath = value;

					NotifyPropertyChanged("CustomScriptPath");
				}
			}
		}

		[Category("REST")]
		[VisibleBy("Authorization", HttpAuthorization.Token)]
		[SortIndex(100)]
		public List<KeyValue> Parameters { get; } = new();

		private string? _destinationEncoding;

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public string? DestinationEncoding
		{
			get { return _destinationEncoding; }

			set
			{
				if (_destinationEncoding != value)
				{
					_destinationEncoding = value;

					NotifyPropertyChanged("DestinationEncoding");
				}
			}
		}

		private bool _mergeToBlob;

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public bool MergeToBlob
		{
			get { return _mergeToBlob; }
			set
			{
				if (_mergeToBlob != value)
				{
					_mergeToBlob = value;

					NotifyPropertyChanged("MergeToBlob");
				}
			}
		}

		private string? _incrementalExpression;

		[Category("REST")]
		[Description(
			"Azure Data Factory expression that should evaluate to HTTP parameters relevant to incremental filtering." +
			"The text '{incremental}' will be replaced with the output column name of the IncrementalQuery SQL statement." +
			"The text '{load}' will be replaced with the load column corresponding to IncrementalStagingColumn.")]
		public string? IncrementalExpression
		{
			get { return _incrementalExpression; }
			set
			{
				if (_incrementalExpression != value)
				{
					_incrementalExpression = value;

					NotifyPropertyChanged("IncrementalExpression");
				}
			}
		}

		private string? _collectionReference;

		[Category("REST")]
		[SortIndex(100)]
		public string? CollectionReference
		{
			get { return _collectionReference; }
			set
			{
				if (_collectionReference != value)
				{
					_collectionReference = value;

					NotifyPropertyChanged("CollectionReference");
				}
			}
		}

		private string? _paginationNextLink;

		[Category("REST")]
		[SortIndex(100)]
		public string? PaginationNextLink
		{
			get { return _paginationNextLink; }
			set
			{
				if (_paginationNextLink != value)
				{
					_paginationNextLink = value;

					NotifyPropertyChanged("PaginationNextLink");
				}
			}
		}

		private bool _paginationLinkIsRelative;

		[Category("REST")]
		[SortIndex(100)]
		public bool PaginationLinkIsRelative
		{
			get { return _paginationLinkIsRelative; }
			set
			{
				if (_paginationLinkIsRelative != value)
				{
					_paginationLinkIsRelative = value;

					NotifyPropertyChanged("PaginationLinkIsRelative");
				}
			}
		}

		public override DataSource Clone()
		{
			RestDataSource clone = new(string.Empty, Connection, SourceSystem, Tenant)
			{
				LoadTable = new LoadTable()
			};

			if (LoadTable != null)
			{
				foreach (Column column in LoadTable.Columns)
				{
					Column cloneColumn = new(column.Name)
					{
						AddedOnBusinessDate = column.AddedOnBusinessDate,
						DataType = column.DataType,
						Length = column.Length,
						Precision = column.Precision,
						Scale = column.Scale,
						Nullable = column.Nullable
					};

					cloneColumn.PropertyChanged += (s, e) =>
					{
						NotifyPropertyChanged("Column");
					};

					clone.LoadTable.Columns.Add(cloneColumn);
				}
			}

			foreach (Satellite sat in Satellites)
			{
				Satellite cloneSat = new(sat.Name)
				{
					Type = sat.Type
				};

				clone.Satellites.Add(cloneSat);
			}

			clone.StagingTable = new StagingTable();

			if (StagingTable != null)
			{
				foreach (StagingColumn stagingColumn in StagingTable.Columns)
				{
					StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable, clone.Satellites);

					stagingColumn.PropertyChanged += (s, e) =>
					{
						NotifyPropertyChanged("StagingColumn");
					};

					clone.StagingTable.Columns.Add(cloneColumn);
				}
			}

			if (BusinessKey != null)
			{
				if (BusinessKey is Link link)
				{
					clone.BusinessKey = link;
				}
				else if (BusinessKey is Hub hub)
				{
					clone.BusinessKey = hub;
				}
			}

			if (Parameters != null)
			{
				foreach (KeyValue keyVal in Parameters)
				{
					KeyValue cloneKeyVal = new()
					{
						Key = keyVal.Key,
						Value = keyVal.Value
					};

					clone.Parameters.Add(cloneKeyVal);
				}
			}

			foreach (HubRelationship hubRelationship in HubRelationships)
			{
				Hub hub = hubRelationship.Hub;

				HubRelationship cloneRelationship = new(hub);

				foreach (HubMapping hubMapping in hubRelationship.Mappings)
				{
					StagingColumn cloneStagingColumn = clone.StagingTable.Columns.Single(cloneStgCol => cloneStgCol.Name == hubMapping.StagingColumn!.Name);

					HubMapping cloneMapping = new(hubMapping.HubColumn)
					{
						StagingColumn = cloneStagingColumn
					};

					cloneRelationship.Mappings.Add(cloneMapping);
				}

				clone.HubRelationships.Add(cloneRelationship);
			}

			foreach (LinkRelationship linkRelationship in LinkRelationships)
			{
				Link cloneLink = linkRelationship.Link;

				LinkRelationship cloneRelationship = new(cloneLink);

				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					StagingColumn cloneStagingColumn = clone.StagingTable.Columns.Single(cloneStgCol => cloneStgCol.Name == linkMapping.StagingColumn!.Name);

					LinkMapping cloneMapping = new(linkMapping.LinkColumn)
					{
						StagingColumn = cloneStagingColumn
					};

					cloneRelationship.Mappings.Add(cloneMapping);
				}

				clone.LinkRelationships.Add(cloneRelationship);
			}

			clone.AzureLinkedServiceReference = AzureLinkedServiceReference;
			clone.Build = Build;
			clone.BusinessDateColumn = BusinessDateColumn;
			clone.CollectionReference = CollectionReference;
			clone.ContainsMultiStructuredJson = ContainsMultiStructuredJson;
			clone.CustomScriptPath = CustomScriptPath;
			clone.DataSourceType = DataSourceType;
			clone.DefaultLoadWidth = DefaultLoadWidth;
			clone.ErrorHandling = ErrorHandling;
			clone.FileName = FileName;
			clone.IncrementalStagingColumn = IncrementalStagingColumn;
			clone.IncrementalQuery = IncrementalQuery;
			clone.IncrementalExpression = IncrementalExpression;
			clone.MergeToBlob = MergeToBlob;
			clone.DestinationEncoding = DestinationEncoding;
			clone.PaginationLinkIsRelative = PaginationLinkIsRelative;
			clone.PaginationNextLink = PaginationNextLink;
			clone.QualifiedName = QualifiedName;
			clone.RelativeUrl = RelativeUrl;
			clone.SaveCookie = SaveCookie;
			clone.SqlSelectQuery = SqlSelectQuery;
			clone.GenerateLatestViews = GenerateLatestViews;

			return clone;
		}

		public override void GetMetadata()
		{
			DataTypeAnalyzer analyzer = new(this);
			Dictionary<string, Column> dict = analyzer.AnalyzeDataTypes();
			UpdateTables(dict);
			analyzer.ColumnTypeDecided.Clear();
			dict.Clear();
		}
	}
}
