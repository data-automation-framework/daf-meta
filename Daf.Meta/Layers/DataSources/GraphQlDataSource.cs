// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;
using Daf.Meta.Interfaces;
using Daf.Meta.JsonConverters;
using Daf.Meta.Layers.Connections;

namespace Daf.Meta.Layers.DataSources
{
	[JsonDiscriminator("GraphQl")]
	public class GraphQlDataSource : DataSource, IConnection
	{
		public GraphQlDataSource(string name, GraphQlConnection connection, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
			_connection = connection;
		}

		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<Connection> Connections { get { return Model.Instance.Connections; } }

		private GraphQlConnection _connection;

		[JsonConverter(typeof(GraphQlConnectionConverter))]
		public GraphQlConnection Connection
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

		private int _numberToFetch;

		public int NumberToFetch
		{
			get { return _numberToFetch; }
			set
			{
				if (_numberToFetch != value)
				{
					_numberToFetch = value;

					NotifyPropertyChanged("NumberToFetch");
				}
			}
		}

		private string _graphQlQuery = "";

		public string GraphQlQuery
		{
			get { return _graphQlQuery; }
			set
			{
				if (_graphQlQuery != value)
				{
					_graphQlQuery = value;

					NotifyPropertyChanged("GraphQlQuery");
				}
			}
		}

		private string? _parent;

		public string? Parent
		{
			get { return _parent; }
			set
			{
				if (_parent != value)
				{
					_parent = value;

					NotifyPropertyChanged("Parent");
				}
			}
		}

		private string? _collectionReference;

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

		private string? _destinationEncoding;

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

		public override DataSource Clone()
		{
			GraphQlDataSource clone = new(string.Empty, Connection, SourceSystem, Tenant)
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
				if (BusinessKey is Link linkBusinessKey)
				{
					Link cloneBusinessKey = new(linkBusinessKey.Name);

					foreach (StagingColumn stagingColumn in linkBusinessKey.BusinessKeys)
					{
						StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable, clone.Satellites);

						cloneBusinessKey.BusinessKeys.Add(cloneColumn);
					}

					clone.BusinessKey = cloneBusinessKey;
				}
				else if (BusinessKey is Hub hubBusinessKey)
				{
					Hub cloneBusinessKey = new(hubBusinessKey.Name);

					foreach (StagingColumn stagingColumn in hubBusinessKey.BusinessKeys)
					{
						StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable, clone.Satellites);

						cloneBusinessKey.BusinessKeys.Add(cloneColumn);
					}

					clone.BusinessKey = cloneBusinessKey;
				}
			}

			foreach (HubRelationship hubRelationship in HubRelationships)
			{
				Hub cloneHub = new(hubRelationship.Hub.Name);

				foreach (StagingColumn stagingColumn in hubRelationship.Hub.BusinessKeys)
				{
					StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable, clone.Satellites);

					cloneHub.BusinessKeys.Add(cloneColumn);
				}

				HubRelationship cloneRelationship = new(cloneHub);

				foreach (HubMapping hubMapping in hubRelationship.Mappings)
				{
					StagingColumn cloneBusinessKeyColumn = clone.BusinessKey!.BusinessKeys.Single(cloneBk => cloneBk.Name == hubMapping.HubColumn.Name);
					StagingColumn cloneStagingColumn = clone.StagingTable.Columns.Single(cloneStgCol => cloneStgCol.Name == hubMapping.StagingColumn!.Name);

					HubMapping cloneMapping = new(cloneBusinessKeyColumn)
					{
						StagingColumn = cloneStagingColumn
					};

					cloneRelationship.Mappings.Add(cloneMapping);
				}

				clone.HubRelationships.Add(cloneRelationship);
			}

			foreach (LinkRelationship linkRelationship in LinkRelationships)
			{
				Link cloneLink = new(linkRelationship.Link.Name);

				foreach (StagingColumn stagingColumn in linkRelationship.Link.BusinessKeys)
				{
					StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable, clone.Satellites);

					cloneLink.BusinessKeys.Add(cloneColumn);
				}

				LinkRelationship cloneRelationship = new(cloneLink);

				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					StagingColumn cloneBusinessKeyColumn = clone.BusinessKey!.BusinessKeys.Single(cloneBk => cloneBk.Name == linkMapping.LinkColumn.Name);
					StagingColumn cloneStagingColumn = clone.StagingTable.Columns.Single(cloneStgCol => cloneStgCol.Name == linkMapping.StagingColumn!.Name);

					LinkMapping cloneMapping = new(cloneBusinessKeyColumn)
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
			clone.DataSourceType = DataSourceType;
			clone.DefaultLoadWidth = DefaultLoadWidth;
			clone.ErrorHandling = ErrorHandling;
			clone.FileName = FileName;
			clone.IncrementalStagingColumn = IncrementalStagingColumn;
			clone.IncrementalQuery = IncrementalQuery;
			clone.MergeToBlob = MergeToBlob;
			clone.DestinationEncoding = DestinationEncoding;
			clone.QualifiedName = QualifiedName;
			clone.GraphQlQuery = GraphQlQuery;
			clone.NumberToFetch = NumberToFetch;
			clone.Parent = Parent;
			clone.RelativeUrl = RelativeUrl;
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
