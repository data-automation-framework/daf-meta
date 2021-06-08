// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Daf.Meta.JsonConverters;

namespace Daf.Meta.Layers
{
	public abstract class DataSource : PropertyChangedBaseClass, IComparable<DataSource>
	{
		internal event EventHandler? StagingColumnAddedRemoved;

		protected DataSource(string name, SourceSystem sourceSystem, Tenant tenant)
		{
			_name = name;
			_sourceSystem = sourceSystem;
			_tenant = tenant;

			// May want to create specific EventHandlers for these but not doing so until we're
			// starting the upcoming re-design of Hub-/LinkRelationship so as to not have to duplicate code.
			HubRelationships.CollectionChanged += HubRelationshipsChanged;
			LinkRelationships.CollectionChanged += LinkRelationshipsChanged;

			// Both lists need to be updated when columns are changed. May need to split into separate events or add EventArgs.
			StagingColumnAddedRemoved += (s, e) => { GetColumnsNotInHubsOrLinks(); };
		}

		private string _name; // This is initialized in the constructor of each derived class.

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;

					NotifyPropertyChanged("Name");
					QualifiedName = string.Empty; // Update QualifiedName's bindings without changing its value.
					TenantName = string.Empty; // Update TenantName's bindings without changing its value.
				}
			}
		}

		[JsonIgnore]
		public string QualifiedName
		{
			get { return $"{SourceSystem.ShortName}_{Tenant.ShortName}_{Name}"; }
			set
			{
				NotifyPropertyChanged("QualifiedName");
			}
		}

		[JsonIgnore]
		public string TenantName
		{
			get { return $"{Tenant.ShortName}_{Name}"; }
			set
			{
				NotifyPropertyChanged("TenantName");
			}
		}

		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<SourceSystem> SourceSystems => Model.Instance.SourceSystems;

		private SourceSystem _sourceSystem; // This is initialized in the constructor of each derived class. Dahomey.Json doesn't support constructors in abstract classes.

		[JsonConverter(typeof(SourceSystemConverter))]
		public SourceSystem SourceSystem
		{
			get
			{
				return _sourceSystem;
			}
			set
			{
				if (_sourceSystem != value)
				{
					_sourceSystem = value;

					NotifyPropertyChanged("SourceSystem");
				}
			}
		}

		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<Tenant> Tenants => Model.Instance.Tenants;

		private Tenant _tenant; // This is initialized in the constructor of each derived class. Dahomey.Json doesn't support constructors in abstract classes.

		[JsonConverter(typeof(TenantConverter))]
		public Tenant Tenant
		{
			get
			{
				return _tenant;
			}
			set
			{
				if (_tenant != value)
				{
					_tenant = value;

					NotifyPropertyChanged("Tenant");
				}
			}
		}

		private DataSourceType _dataSourceType;

		public DataSourceType DataSourceType
		{
			get { return _dataSourceType; }
			set
			{
				if (_dataSourceType != value)
				{
					_dataSourceType = value;
					NotifyPropertyChanged("DataSourceType");
				}
			}
		}

		private DestinationType _destinationType;

		public DestinationType DestinationType
		{
			get { return _destinationType; }
			set
			{
				if (_destinationType != value)
				{
					_destinationType = value;

					if (value == DestinationType.Blob)
					{
						StagingTable?.Columns.Clear();
						LoadTable?.Columns.Clear();
						Satellites.Clear();
						LinkRelationships.Clear();
						HubRelationships.Clear();
						BusinessKey = null;
					}

					NotifyPropertyChanged("DestinationType");
				}
			}
		}

		private LoadWidth _defaultLoadWidth;

		public LoadWidth DefaultLoadWidth
		{
			get { return _defaultLoadWidth; }
			set
			{
				if (_defaultLoadWidth != value)
				{
					_defaultLoadWidth = value;

					NotifyPropertyChanged("DefaultLoadWidth");
				}
			}
		}

		private bool _generateLatestViews;

		public bool GenerateLatestViews
		{
			get { return _generateLatestViews; }
			set
			{
				if (_generateLatestViews != value)
				{
					_generateLatestViews = value;

					NotifyPropertyChanged("GenerateLatestViews");
				}
			}
		}

		private bool? _containsMultiStructuredJson;

		public bool? ContainsMultiStructuredJson
		{
			get { return _containsMultiStructuredJson; }
			set
			{
				if (_containsMultiStructuredJson != value)
				{
					_containsMultiStructuredJson = value;

					NotifyPropertyChanged("ContainsMultiStructuredJson");
				}
			}
		}

		private string? _fileName;

		public string? FileName
		{
			get { return _fileName; }
			set
			{
				if (_fileName != value)
				{
					_fileName = value;

					NotifyPropertyChanged("FileName");
				}
			}
		}

		private string? _incrementalStagingColumn;

		public string? IncrementalStagingColumn
		{
			get { return _incrementalStagingColumn; }
			set
			{
				if (_incrementalStagingColumn != value)
				{
					_incrementalStagingColumn = value;

					NotifyPropertyChanged("IncrementalStagingColumn");
				}
			}
		}

		private string? _incrementalQuery;

		[DataType(DataType.MultilineText)]
		public string? IncrementalQuery
		{
			get { return _incrementalQuery; }
			set
			{
				if (_incrementalQuery != value)
				{
					_incrementalQuery = value;

					NotifyPropertyChanged("IncrementalQuery");
				}
			}
		}

		private string? _businessDateColumn;

		public string? BusinessDateColumn
		{
			get { return _businessDateColumn; }
			set
			{
				if (_businessDateColumn != value)
				{
					_businessDateColumn = value;

					NotifyPropertyChanged("BusinessDateColumn");
				}
			}
		}

		private string? _sqlSelectQuery;

		public string? SqlSelectQuery
		{
			get { return _sqlSelectQuery; }
			set
			{
				if (_sqlSelectQuery != value)
				{
					_sqlSelectQuery = value;

					NotifyPropertyChanged("SqlSelectQuery");
				}
			}
		}

		private string? _azureLinkedServiceReference;

		public string? AzureLinkedServiceReference
		{
			get { return _azureLinkedServiceReference; }
			set
			{
				if (_azureLinkedServiceReference != value)
				{
					_azureLinkedServiceReference = value;

					NotifyPropertyChanged("AzureLinkedServiceReference");
				}
			}
		}

		private Build _build;

		public Build Build
		{
			get { return _build; }
			set
			{
				if (_build != value)
				{
					_build = value;

					NotifyPropertyChanged("Build");
				}
			}
		}

		private string? _errorHandling;

		public string? ErrorHandling
		{
			get { return _errorHandling; }
			set
			{
				if (_errorHandling != value)
				{
					_errorHandling = value;

					NotifyPropertyChanged("ErrorHandling");
				}
			}
		}

		private BusinessKey? _businessKey;

		[JsonConverter(typeof(BusinessKeyConverter))]
		public BusinessKey? BusinessKey
		{
			get { return _businessKey; }
			set
			{
				if (_businessKey != value)
				{
					_businessKey = value;

					NotifyPropertyChanged("BusinessKey");
				}
			}
		}

		protected void OnColumnsChanged()
		{
			StagingColumnAddedRemoved?.Invoke(this, new EventArgs());
		}

		public StagingColumn? GetBusinessKey()
		{
			if (BusinessKey == null)
				throw new InvalidOperationException();

			return Model.Instance.GetBusinessKey(BusinessKey.Name);
		}

		public ObservableCollection<HubRelationship> HubRelationships { get; } = new();
		public ObservableCollection<LinkRelationship> LinkRelationships { get; } = new();
		public ObservableCollection<Satellite> Satellites { get; } = new();
		public LoadTable? LoadTable { get; set; } = new();

		private StagingTable? _stagingTable = new();

		public StagingTable? StagingTable
		{
			get => _stagingTable;
			set
			{
				if (_stagingTable != value)
				{
					_stagingTable = value;

					// Update ColumnsNotInHubsOrLinks when the StagingTable is updated after deserialization.
					// This method is run repeatedly.. Figure out if that's unnecessary.
					GetColumnsNotInHubsOrLinks();
				}
			}
		}

		public abstract DataSource Clone();

		public abstract void GetMetadata();

		/// <summary>
		/// Creates a new Column and adds it to the LoadTable's Columns list.
		/// </summary>
		/// <returns>The added Column.</returns>
		public Column AddLoadColumn()
		{
			Column column = new(name: "New Column");

			column.PropertyChanged += (s, e) =>
			{
				NotifyPropertyChanged("Column");
			};

			if (LoadTable == null)
			{
				LoadTable = new LoadTable();
			}

			LoadTable.Columns.Add(column);

			return column;
		}

		public void RemoveLoadColumn(Column columnToRemove)
		{
			if (columnToRemove == null)
				throw new ArgumentNullException(nameof(columnToRemove));

			if (LoadTable == null)
				throw new InvalidOperationException(nameof(LoadTable));

			columnToRemove.ClearSubscribers();

			LoadTable.Columns.Remove(columnToRemove);
		}

		/// <summary>
		/// Creates a new StagingColumn and adds it to the StagingTable's StagingColumns list.
		/// </summary>
		/// <returns>The added StagingColumn.</returns>
		public StagingColumn AddStagingColumn()
		{
			if (StagingTable == null)
				throw new InvalidOperationException(nameof(StagingTable));

			StagingColumn stagingColumn = new(name: "New Column");

			stagingColumn.PropertyChanged += (s, e) =>
			{
				NotifyPropertyChanged("StagingColumn");
			};

			StagingTable.Columns.Add(stagingColumn);

			OnColumnsChanged();

			return stagingColumn;
		}

		public void RemoveStagingColumn(StagingColumn columnToRemove)
		{
			if (columnToRemove == null)
				throw new ArgumentNullException(nameof(columnToRemove));

			if (StagingTable == null)
				throw new InvalidOperationException(nameof(StagingTable));

			columnToRemove.ClearSubscribers();

			StagingTable.Columns.Remove(columnToRemove);

			OnColumnsChanged();
		}

		public Satellite AddSatellite(string name = "New Satellite")
		{
			Satellite satellite = new(name) { Type = SatelliteType.HashDiff };

			satellite.PropertyChanged += (s, e) =>
			{
				NotifyPropertyChanged("Satellite");
			};

			Satellites.Add(satellite);

			NotifyPropertyChanged("Satellites");

			return satellite;
		}

		public void RemoveSatellite(Satellite satellite)
		{
			if (Satellites.Count == 0 || satellite == null)
				throw new InvalidOperationException();

			satellite.ClearSubscribers();

			Satellites.Remove(satellite);

			NotifyPropertyChanged("Satellites");
		}

		protected static List<Column> GetSqlList(DataTable dt)
		{
			List<Column> convertedList = new(); // = (from rw in dt.AsEnumerable()
												//select new Column()
												//{
												// Name = Convert.ToString(rw["ColumnName"]),
												// DataType = Convert.ToString(rw["DataTypeName"]),
												// Length = Convert.ToInt32(rw["ColumnSize"]),
												// Precision = Convert.ToInt32(rw["NumericPrecision"]),
												// Scale = Convert.ToInt32(rw["NumericScale"]),
												// Nullable = Convert.ToBoolean(rw["AllowDBNull"])
												//}).ToList();

			foreach (DataRow rw in dt.AsEnumerable())
			{
				Column col = new(name: Convert.ToString(rw["ColumnName"], CultureInfo.InvariantCulture)!)
				{
					DataType = (SqlServerDataType)Enum.Parse(typeof(SqlServerDataType), Convert.ToString(rw["DataTypeName"], CultureInfo.InvariantCulture)!, ignoreCase: true),
					Nullable = Convert.ToBoolean(rw["AllowDBNull"], CultureInfo.InvariantCulture)
				};

				if (col.DataType != SqlServerDataType.Decimal && col.DataType != SqlServerDataType.DateTime2 && !col.DataType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("int") && col.DataType != SqlServerDataType.Bit && col.DataType != SqlServerDataType.Char && col.DataType != SqlServerDataType.UniqueIdentifier)
					col.Length = Convert.ToInt32(rw["ColumnSize"], CultureInfo.InvariantCulture);

				if (col.DataType == SqlServerDataType.Decimal)
					col.Precision = Convert.ToInt32(rw["NumericPrecision"], CultureInfo.InvariantCulture);

				if (col.DataType is SqlServerDataType.Decimal or SqlServerDataType.DateTime2)
					col.Scale = Convert.ToInt32(rw["NumericScale"], CultureInfo.InvariantCulture);

				convertedList.Add(col);
			}

			//return new ObservableCollection<Column>(convertedList);
			return new List<Column>(convertedList);
		}

		protected static List<Column> GetOdbcList(DataTable dt)
		{
			List<Column> convertedList = new();

			foreach (DataRow rw in dt.AsEnumerable())
			{
				dynamic dataType = rw["DataType"];

				string dataTypeName = DotNetToSqlTypeConverter(dataType.Name);

				Column col = new(name: Convert.ToString(rw["ColumnName"], CultureInfo.InvariantCulture)!)
				{
					DataType = (SqlServerDataType)Enum.Parse(typeof(SqlServerDataType), dataTypeName, ignoreCase: true),
					Nullable = Convert.ToBoolean(rw["AllowDBNull"], CultureInfo.InvariantCulture)
				};

				if (col.DataType != SqlServerDataType.Decimal && col.DataType != SqlServerDataType.DateTime2 && !col.DataType.ToString().ToLower(CultureInfo.InvariantCulture).Contains("int") && col.DataType != SqlServerDataType.Bit && col.DataType != SqlServerDataType.Char && col.DataType != SqlServerDataType.UniqueIdentifier)
					col.Length = Convert.ToInt32(rw["ColumnSize"], CultureInfo.InvariantCulture);

				if (col.DataType == SqlServerDataType.Decimal)
					col.Precision = Convert.ToInt32(rw["NumericPrecision"], CultureInfo.InvariantCulture);

				if (col.DataType is SqlServerDataType.Decimal or SqlServerDataType.DateTime2)
					col.Scale = Convert.ToInt32(rw["NumericScale"], CultureInfo.InvariantCulture);

				convertedList.Add(col);
			}

			//return new ObservableCollection<Column>(convertedList);
			return new List<Column>(convertedList);
		}

		protected static string DotNetToSqlTypeConverter(string dataTypeName)
		{
			return new Dictionary<string, string>
			{
				{ "Int64", "bigint" },
				{ "Int32", "int" },
				{ "Int16", "smallint" },
				{ "String", "varchar" },
				{ "DateTime", "datetime2" },
				{ "DateTimeOffset", "datetimeoffset" },
				{ "Byte[]", "binary" },
				{ "Boolean", "bit" },
				{ "Double", "float" },
				{ "Decimal", "decimal" },
				{ "Object", "sql_variant" },
				{ "TimeSpan", "time" },
				{ "Byte", "tinyint" },
				{ "Guid", "uniqueidentifier" },
				{ "Xml", "xml" }
			}[dataTypeName];
		}

		internal JsonDocument? TryGetJsonDoc(WebClient client, string? url)
		{
			if (url == null)
			{
				throw new ArgumentNullException($"URL was null in TryGetJsonDoc for data source {Name}");
			}

			string? rawJson;

			try
			{
				rawJson = client.DownloadString(url);
			}
			catch (WebException)
			{
				return null;
			}

			return JsonDocument.Parse(rawJson);
		}

		protected static string FormatColumnName(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			string formattedName = $"{name[0].ToString().ToUpper(CultureInfo.InvariantCulture)}{name.Substring(1)}";

			if (formattedName.EndsWith("id", StringComparison.Ordinal) || formattedName.EndsWith("Id", StringComparison.Ordinal))
			{
				formattedName = formattedName.Substring(0, formattedName.Length - 2) + formattedName[^2].ToString().ToUpper(CultureInfo.InvariantCulture) + formattedName[^1].ToString().ToUpper(CultureInfo.InvariantCulture);
			}

			return formattedName;
		}

		public void UpdateTables(Dictionary<string, Column> suggestedColumnDict)
		{
			if (suggestedColumnDict == null)
				throw new ArgumentNullException(nameof(suggestedColumnDict));

			if (LoadTable == null)
			{
				LoadTable = new LoadTable();
			}

			if (StagingTable == null)
			{
				StagingTable = new StagingTable();
			}

			// Update existing columns
			foreach (Column stagingColumn in StagingTable.Columns!)
			{
				if (!string.IsNullOrEmpty(stagingColumn.Name) && suggestedColumnDict.ContainsKey(stagingColumn.Name!))
				{
					StagingColumn suggestedColumn = (StagingColumn)suggestedColumnDict[stagingColumn.Name!];

					if (stagingColumn.DataType != suggestedColumn.DataType)
					{
						stagingColumn.DataType = suggestedColumn.DataType;
					}

					if (stagingColumn.Nullable != suggestedColumn.Nullable)
					{
						stagingColumn.Nullable = suggestedColumn.Nullable;
					}

					if (stagingColumn.Length != suggestedColumn.Length)
					{
						stagingColumn.Length = suggestedColumn.Length;
					}

					if (stagingColumn.Precision != suggestedColumn.Precision)
					{
						stagingColumn.Precision = suggestedColumn.Precision;
					}

					if (stagingColumn.Scale != suggestedColumn.Scale)
					{
						stagingColumn.Scale = suggestedColumn.Scale;
					}
				}
			}

			// Add new columns
			foreach (Column newColumn in suggestedColumnDict.Values)
			{
				if (!StagingTable.Columns.Any(stagingColumn => stagingColumn.Name == newColumn.Name))
				{
					Column loadColumn = new(name: newColumn.Name)
					{
						DataType = SqlServerDataType.NVarChar,
						Length = newColumn.Length > 100 ? newColumn.Length : 100,
						Nullable = true
					};

					loadColumn.PropertyChanged += (s, e) =>
					{
						NotifyPropertyChanged("Column");
					};

					LoadTable.Columns.Add(loadColumn);

					StagingColumn toAdd = new(name: FormatColumnName(newColumn.Name!))
					{
						DataType = newColumn.DataType,
						Length = newColumn.Length,
						Precision = newColumn.Precision,
						Scale = newColumn.Scale,
						Nullable = newColumn.Nullable,
						LoadColumn = loadColumn
					};

					toAdd.PropertyChanged += (s, e) =>
					{
						NotifyPropertyChanged("StagingColumn");
					};

					StagingTable.Columns.Add(toAdd);
				}
			}
		}

		[JsonIgnore]
		public ObservableCollection<StagingColumn> HubList
		{
			get
			{
				ObservableCollection<StagingColumn> hubList = new(GetHubList());

				return hubList;
			}
		}

		// Returns all staging columns mapped to hubs, I think.
		public List<StagingColumn> GetHubList()
		{
			List<StagingColumn> hubList = new();

			if (StagingTable != null && StagingTable.Columns != null && HubRelationships != null)
			{
				foreach (StagingColumn stgColumn in StagingTable.Columns)
				{
					foreach (HubRelationship x in HubRelationships)
					{
						foreach (HubMapping y in x.Mappings)
						{
							if (y.StagingColumn == null)
								throw new InvalidOperationException(nameof(y.StagingColumn));

							if (stgColumn.Name == y.StagingColumn.Name)
								hubList.Add(stgColumn);
						}
					}
				}
			}

			return hubList;
		}

		/// <summary>
		/// Returns relevant keys for the data source depending on driving key completeness.
		/// </summary>
		/// <param name="prefix">Prefix the keys with this string (dot added inside function e.g. for "stg." pass "stg").</param>
		/// <returns>List containing relevant key names.</returns>
		public List<string> GetRelevantKeys(string prefix = "")
		{
			List<StagingColumn> stgColumnsWithLink = GetLinkList();
			List<StagingColumn> stgColumnsWithHub = GetHubList();

			HashSet<string> relevantKeySet = new();

			if (!string.IsNullOrEmpty(prefix))
			{
				prefix += ".";
			}

			if (stgColumnsWithLink.Count > 0)
			{
				foreach (StagingColumn stgColumn in stgColumnsWithLink)
				{
					Link? link = stgColumn.GetLink(this);

					if (link != null)
					{
						foreach (string relevantKey in link.GetRelevantKeys())
						{
							relevantKeySet.Add(prefix + relevantKey);
						}
					}
				}
			}
			else if (stgColumnsWithHub.Count > 0)
			{
				foreach (StagingColumn stgColumn in stgColumnsWithHub)
				{
					Hub? hub = stgColumn.GetHub(this);

					if (hub != null)
					{
						relevantKeySet.Add(prefix + hub.Name + "NaturalKey");
					}
				}
			}
			else
			{
				throw new InvalidOperationException($"{Name} does not have any links or hubs!");
			}

			if (relevantKeySet.Count == 0)
			{
				throw new InvalidOperationException($"Could not find relevant keys for {Name}!");
			}

			return relevantKeySet.ToList();
		}

		[JsonIgnore]
		public ObservableCollection<StagingColumn>? LinkList
		{
			get
			{
				List<StagingColumn> links = GetLinkList();

				if (links.Count == 0)
					return null;
				else
					return new ObservableCollection<StagingColumn>(links);
			}
		}

		// Returns all staging columns mapped to links, I think.
		public List<StagingColumn> GetLinkList()
		{
			List<StagingColumn> linkList = new();

			if (StagingTable == null || StagingTable.Columns == null || LinkRelationships == null)
			{
				return linkList;
			}

			foreach (StagingColumn stgColumn in StagingTable.Columns)
			{
				foreach (LinkRelationship linkRelationship in LinkRelationships)
				{
					foreach (LinkMapping linkMapping in linkRelationship.Mappings)
					{
						if (linkMapping.StagingColumn == null)
							throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

						if (stgColumn.Name == linkMapping.StagingColumn.Name)
							linkList.Add(stgColumn);
					}
				}
			}

			return linkList;
		}

		public List<StagingColumn> GetSortedLinkList()
		{
			if (StagingTable == null)
				throw new InvalidOperationException(nameof(StagingTable));

			List<StagingColumn> sortedLinkList = new();

			foreach (LinkRelationship linkRelationship in LinkRelationships)
			{
				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					foreach (StagingColumn stgColumn in StagingTable.Columns)
					{
						if (stgColumn == linkMapping.StagingColumn)
						{
							sortedLinkList.Add(stgColumn);

							break;
						}
					}
				}
			}

			return sortedLinkList;
		}

		private ObservableCollection<StagingColumn> _columnsNotInHubsOrLinks = new();

		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
		public ObservableCollection<StagingColumn> ColumnsNotInHubsOrLinks
		{
			get => _columnsNotInHubsOrLinks;
			set
			{
				if (_columnsNotInHubsOrLinks != value)
				{
					_columnsNotInHubsOrLinks = value;

					NotifyPropertyChanged(nameof(ColumnsNotInHubsOrLinks));
				}
			}
		}

		internal void GetColumnsNotInHubsOrLinks()
		{
			ObservableCollection<StagingColumn> columns = new();

			// Neither of these should ever be null.
			if (StagingTable == null || StagingTable.Columns == null)
				throw new InvalidOperationException();

			foreach (StagingColumn stgColumn in StagingTable.Columns)
			{
				bool foundInHubOrLink = false;

				foreach (HubRelationship relationship in HubRelationships)
				{
					foreach (HubMapping mapping in relationship.Mappings)
					{
						if (stgColumn == mapping.StagingColumn)
						{
							foundInHubOrLink = true;
							break;
						}
					}

					if (foundInHubOrLink)
						break;
				}

				if (!foundInHubOrLink)
				{
					foreach (LinkRelationship relationship in LinkRelationships)
					{
						foreach (LinkMapping mapping in relationship.Mappings)
						{
							if (stgColumn == mapping.StagingColumn)
							{
								foundInHubOrLink = true;
								break;
							}
						}

						if (foundInHubOrLink)
							break;
					}
				}

				if (!foundInHubOrLink)
					columns.Add(stgColumn);
			}

			ColumnsNotInHubsOrLinks = columns;
			NotifyPropertyChanged(nameof(ColumnsNotInHubsOrLinks));
		}

		[JsonIgnore]
		public ObservableCollection<StagingColumn> SatelliteList
		{
			get
			{
				ObservableCollection<StagingColumn> satellites = new(GetSatelliteList());

				return satellites;
			}
		}

		public List<StagingColumn> GetSatelliteList()
		{
			List<StagingColumn> satelliteList = new();

			if (StagingTable == null || StagingTable.Columns == null)
			{
				return satelliteList;
			}

			foreach (StagingColumn stgColumn in StagingTable.Columns)
			{
				if (stgColumn.Satellite != null)
				{
					satelliteList.Add(stgColumn);
				}
			}

			return satelliteList;
		}

		// Gets all staging columns not connected to a hub.
		public List<StagingColumn> GetStagingList()
		{
			if (StagingTable == null)
				throw new InvalidOperationException(nameof(StagingTable));

			List<StagingColumn> stagingList = new();

			foreach (StagingColumn stgColumn in StagingTable.Columns)
			{
				StagingColumn? hubColumn = null;

				foreach (HubRelationship x in HubRelationships)
				{
					foreach (HubMapping y in x.Mappings)
					{
						if (y.StagingColumn == null)
							throw new InvalidOperationException(nameof(y.StagingColumn));

						if (y.StagingColumn.Name == stgColumn.Name)
						{
							hubColumn = y.HubColumn;

							break;
						}
					}
				}

				if (hubColumn == null)
				{
					stagingList.Add(stgColumn);
				}
			}

			return stagingList;
		}

		// This one runs several times, probably once for every hub/link-relationship that exists.
		private void HubRelationshipsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			// OldItems contains the HubRelationship(s) that were removed. OldItems is null if no HubRelationship(s) were removed.
			if (e.OldItems != null)
			{
				foreach (HubRelationship oldItem in e.OldItems)
				{
					// Removes each Hub from the list of AssociatedBusinessKeys that no longer participates in a HubRelationship.
					AssociatedBusinessKeys.Remove(oldItem.Hub);
				}
			}

			// For each HubRelationship in HubRelationships, checks if AssociatedBusinessKeys contains the Hub associated with that HubRelationship.
			// If AssociatedBusinessKeys does not already contain that Hub, it adds that Hub to AssociatedBusinessKeys.
			foreach (HubRelationship relationship in HubRelationships)
			{
				if (!AssociatedBusinessKeys.Contains(relationship.Hub))
					AssociatedBusinessKeys.Add(relationship.Hub);
			}

			GetColumnsNotInHubsOrLinks();
		}

		private void LinkRelationshipsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (LinkRelationship oldItem in e.OldItems)
				{
					AssociatedBusinessKeys.Remove(oldItem.Link);
				}
			}

			foreach (LinkRelationship relationship in LinkRelationships)
			{
				if (!AssociatedBusinessKeys.Contains(relationship.Link))
					AssociatedBusinessKeys.Add(relationship.Link);
			}

			GetColumnsNotInHubsOrLinks();
		}

		/// <summary>
		/// A collection of all Hubs and/or Links which participate in a Hub- or LinkRelationship.
		/// </summary>
		[JsonIgnore]
		public ObservableCollection<BusinessKey> AssociatedBusinessKeys { get; } = new();

		public HashSet<string> GetLinkNames()
		{
			HashSet<string> linkList = new();

			if (LinkRelationships != null)
			{
				foreach (LinkRelationship link in LinkRelationships)
				{
					linkList.Add(link.Link.Name);
				}
			}

			return linkList;
		}

		[JsonIgnore]
		public List<string> SatelliteNames
		{
			get
			{
				List<string> satelliteNameList = new();

				if (Satellites.Count != 0)
				{
					foreach (Satellite satellite in Satellites)
					{
						satelliteNameList.Add(satellite.Name);
					}
				}

				return satelliteNameList;
			}
		}

		public HashSet<string> GetSatelliteNames()
		{
			HashSet<string> satelliteList = new();

			foreach (Satellite satellite in Satellites)
			{
				satelliteList.Add(satellite.Name);
			}

			return satelliteList;
		}

		public string GetSatelliteColumnName()
		{
			if (Satellites.Count != 0)
			{
				return Satellites[0].Name;
			}
			else
			{
				throw new InvalidOperationException("Data source does not contain any satellites");
			}
		}

		public List<string> GetIndexColumnNames()
		{
			if (BusinessKey == null)
				throw new InvalidOperationException(nameof(BusinessKey));

			//check if busines key is a link (starts with L)
			List<string> IndexColumns = new();

			if (BusinessKey.Name.Substring(0, 1) == "L")
			{
				foreach (Link link in Model.Instance.Links)
				{
					if (link.Name == BusinessKey.Name)
					{
						foreach (StagingColumn businessKey in link.BusinessKeys)
						{
							IndexColumns.Add(businessKey.Name);
						}

					}
				}

				return IndexColumns;
			}
			else if (BusinessKey.Name.Substring(0, 1) == "H")
			{
				foreach (Hub hub in Model.Instance.Hubs)
				{
					if (hub.Name == BusinessKey.Name)
					{
						foreach (StagingColumn businessKey in hub.BusinessKeys)
						{
							IndexColumns.Add(businessKey.Name);

						}
					}
				}

				return IndexColumns;
			}
			else
			{
				throw new InvalidOperationException($"Business key does not start with H or L for DataSource {Name}");
			}
		}

		public List<StagingColumn> GetOrderedBusinessKeyColumns()
		{
			List<StagingColumn> columns = new();

			if (BusinessKey is Link)
			{
				foreach (LinkRelationship link in LinkRelationships)
				{
					if (link.Link == BusinessKey)
					{
						foreach (LinkMapping linkMapping in link.Mappings)
						{
							if (linkMapping.StagingColumn == null)
								throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

							columns.Add(linkMapping.StagingColumn);
						}
					}
				}

				return columns;
			}
			else if (BusinessKey is Hub)
			{
				foreach (HubRelationship hub in HubRelationships)
				{
					if (hub.Hub == BusinessKey)
					{
						foreach (HubMapping hubMapping in hub.Mappings)
						{
							if (hubMapping.StagingColumn == null)
								throw new InvalidOperationException(nameof(hubMapping.StagingColumn));

							columns.Add(hubMapping.StagingColumn);
						}
					}
				}

				return columns;
			}
			else
			{
				throw new InvalidOperationException($"DataSource {Name} has no business key!");
			}
		}


		public HashSet<string> GetHubNames()
		{
			HashSet<string> hubNameList = new();

			if (HubRelationships != null)
			{
				foreach (HubRelationship hub in HubRelationships)
				{
					hubNameList.Add(hub.Hub.Name);
				}
			}

			return hubNameList;
		}

		public HashSet<string>? GetSatelliteNames(SatelliteType type)
		{
			if (Satellites.Count != 0)
			{
				HashSet<string> satelliteList = new();
				HashSet<string> satelliteNames = GetSatelliteNames();
				foreach (string satelliteName in satelliteNames)
				{
					foreach (Satellite satellite in Satellites)
					{
						if (satellite.Name == satelliteName)
						{
							if (satellite.Type == type)
							{
								satelliteList.Add(satellite.Name);
							}
						}
					}
				}

				if (satelliteList.Count != 0)
				{
					return satelliteList;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		public HashSet<string>? GetSatelliteNames(SatelliteType[] type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			HashSet<string> satelliteList = new();
			foreach (SatelliteType satelliteType in type)
			{
				HashSet<string>? tmpSatelliteList;
				tmpSatelliteList = GetSatelliteNames(satelliteType);

				if (tmpSatelliteList != null)
				{
					satelliteList.UnionWith(tmpSatelliteList);
				}
			}

			return satelliteList;
		}

		[JsonIgnore]
		public Dictionary<string, List<StagingColumn>> HubandHubColumns
		{
			get
			{
				return GetHubandHubColumns();
			}
		}

		public Dictionary<string, List<StagingColumn>> GetHubandHubColumns()
		{
			Dictionary<string, List<StagingColumn>> hubHubColumnPairs = new();

			HashSet<string> hubNames = GetHubNames();
			if (hubNames.Count > 0)
			{
				foreach (string hubName in hubNames)
				{
					List<StagingColumn> hubColumns = new(); //create a list of associated columns for each hub name in the data source.

					if (StagingTable != null)
					{
						foreach (StagingColumn stgColumn in StagingTable.Columns)
						{
							foreach (HubRelationship x in HubRelationships)
							{
								if (x.Hub.Name == hubName)
								{
									foreach (HubMapping y in x.Mappings)
									{
										if (y.StagingColumn == null)
											throw new InvalidOperationException(nameof(y.StagingColumn));

										// Check if the column should be added to list of associated columns to current hub.
										if (y.StagingColumn.Name == stgColumn.Name)
										{
											hubColumns.Add(stgColumn);

											break;
										}
									}
								}
							}
						}
					}

					if (hubColumns.Count != 0)
					{
						hubHubColumnPairs.Add(hubName, hubColumns);
					}
				}

				return hubHubColumnPairs;
			}

			return hubHubColumnPairs;
		}

		public string GetFormattedLinkInsertColumns(int numTabs, bool withLoadDate = false)
		{
			List<string> insertColumns = new();

			if (withLoadDate)
			{
				List<string> relevantKeys = GetRelevantKeys();
				relevantKeys.Insert(1, "LoadDate");
				insertColumns.AddRange(relevantKeys);
			}
			else
			{
				insertColumns.AddRange(GetRelevantKeys());
			}

			foreach (LinkRelationship linkRelationship in LinkRelationships)
			{
				List<string> drivingColumns = new();
				List<string> nonDrivingColumns = new();

				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					if (linkMapping.StagingColumn == null)
						throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

					string? alias = null;

					StagingColumn stagingColumn = linkMapping.StagingColumn;

					if (stagingColumn.GetHub(this) != null)
					{
						alias = Converter.CheckKeyword(stagingColumn.GetHubColumnName(this));
					}
					else if (stagingColumn.Name != null)
					{
						alias = Converter.CheckKeyword(stagingColumn.Name);
					}
					else
					{
						throw new InvalidOperationException($"Staging column in {Name} does not have a name!");
					}

					if (!linkRelationship.Link.AllColumnsAreDriving)
					{
						if (linkMapping.LinkColumn.Driving == true)
						{
							drivingColumns.Add(alias);
						}
						else
						{
							nonDrivingColumns.Add(alias);
						}
					}
					else
					{
						insertColumns.Add(alias);
					}
				}

				if (!linkRelationship.Link.AllColumnsAreDriving)
				{
					insertColumns.InsertRange(2, drivingColumns);
					insertColumns.AddRange(nonDrivingColumns);
				}
			}

			return string.Join($",{Environment.NewLine}{new string('\t', numTabs)}", insertColumns);
		}

		public string GetFormattedLinkSelectColumns(int numTabs, bool withLoadDate = false)
		{
			List<string> selectColumns = new();

			foreach (string relevantKey in GetRelevantKeys())
			{
				selectColumns.Add($"stg.{relevantKey} AS {relevantKey}");
			}

			if (withLoadDate)
			{
				selectColumns.Insert(1, "stg.LoadDate");
			}

			foreach (LinkRelationship linkRelationship in LinkRelationships)
			{
				List<string> drivingColumns = new();
				List<string> nonDrivingColumns = new();

				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					if (linkMapping.StagingColumn == null)
						throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

					string? alias = null;

					StagingColumn stagingColumn = linkMapping.StagingColumn;

					if (stagingColumn.GetHub(this) != null)
					{
						string hubColumnName = Converter.CheckKeyword(stagingColumn.GetHubColumnName(this));
						alias = $"stg.{hubColumnName} AS {hubColumnName}";
					}
					else if (stagingColumn.Name != null)
					{
						alias = $"stg.{Converter.CheckKeyword(stagingColumn.Name)} AS {Converter.CheckKeyword(stagingColumn.Name)}";
					}
					else
					{
						throw new InvalidOperationException($"Staging column in {Name} does not have a name!");
					}

					if (!linkRelationship.Link.AllColumnsAreDriving)
					{
						if (linkMapping.LinkColumn.Driving == true)
						{
							drivingColumns.Add(alias);
						}
						else
						{
							nonDrivingColumns.Add(alias);
						}
					}
					else
					{
						selectColumns.Add(alias);
					}
				}

				if (!linkRelationship.Link.AllColumnsAreDriving)
				{
					selectColumns.InsertRange(2, drivingColumns);
					selectColumns.AddRange(nonDrivingColumns);
				}
			}

			return string.Join($",{Environment.NewLine}{new string('\t', numTabs)}", selectColumns);
		}

		public List<Satellite> GetAssociatedSatellites()
		{
			return Satellites.ToList();
		}

		public SqlServerDataType? GetLoadColumnDatatype(StagingColumn column)
		{
			if (column == null)
				throw new ArgumentNullException(nameof(column));

			if (LoadTable == null)
				throw new InvalidOperationException(nameof(LoadTable));

			SqlServerDataType? dataType = null;

			foreach (Column loadColumn in LoadTable.Columns)
			{
				if (loadColumn == column.LoadColumn)
				{
					dataType = loadColumn.DataType;
					break;
				}
			}

			return dataType;
		}

		public Column? GetLoadColumn(StagingColumn column)
		{
			if (column == null)
				throw new ArgumentNullException(nameof(column));

			if (LoadTable == null)
				throw new InvalidOperationException(nameof(LoadTable));

			if (column.LoadColumn == null)
				throw new InvalidOperationException(nameof(column.LoadColumn));

			foreach (Column loadColumn in LoadTable.Columns)
			{
				if (loadColumn.Name == column.LoadColumn.Name)
				{
					return loadColumn;
				}
			}

			return null;
		}

		public string GetNamedLinkDrivingKeySql(string linkName)
		{
			string linkHashkeySql = "CAST(HASHBYTES('SHA1', ";

			bool firstRow = true;

			string? columnSQL;

			LinkRelationship? linkRelationship = LinkRelationships.SingleOrDefault(linkRel => linkRel.Link.Name == linkName);

			if (linkRelationship == null)
			{
				throw new ArgumentException($"Link {linkName} not found in data source {Name}.");
			}

			foreach (LinkMapping linkMapping in linkRelationship!.Mappings)
			{
				if (linkMapping.StagingColumn == null)
					throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

				StagingColumn linkColumn = linkMapping.StagingColumn;

				if (linkRelationship.Link.Name == linkName && linkMapping.LinkColumn.Driving == true)
				{
					string hashLength = "255";
					if (Model.Instance.DataWarehouse.UseNewHashLogic)
					{
						if (linkColumn.DataTypeStringLength > 4000)
						{
							hashLength = "MAX";
						}
						else
						{
							hashLength = linkColumn.DataTypeStringLength.ToString(CultureInfo.InvariantCulture);
						}
					}

					columnSQL = $"UPPER(CONVERT([nvarchar]({hashLength}), " + Converter.CheckSqlCompatibility(Converter.CheckKeyword(linkColumn.Name));
					if (linkColumn.DataType == SqlServerDataType.DateTime2)
					{
						columnSQL += ", 121))";
					}
					else if (linkColumn.DataType == SqlServerDataType.Date)
					{
						columnSQL += ", 120))";
					}
					else
					{
						columnSQL += "))";
					}

					if (linkColumn.DataType is SqlServerDataType.VarChar or SqlServerDataType.NVarChar)
					{
						columnSQL = Helper.GetTrimSql(columnSQL);
					}

					if (linkColumn.Nullable)
					{
						columnSQL = "COALESCE(" + columnSQL + ", '')";
					}

					if (firstRow == false)
					{
						columnSQL = " + '|' + " + columnSQL;
					}

					firstRow = false;
					linkHashkeySql += columnSQL;
				}
			}

			linkHashkeySql += ") AS BINARY(20)) AS " + linkName + "DrivingKey";

			return linkHashkeySql;
		}

		public string GetNamedLinkHashkeySql(string linkName)
		{
			List<StagingColumn> linkColumns = GetSortedLinkList();

			string linkHashkeySql = "CAST(HASHBYTES('SHA1', ";
			bool firstRow = true;
			string? columnSQL;

			foreach (StagingColumn linkColumn in linkColumns)
			{
				LinkRelationship? relationship = null;

				// Find match LinkRelationship
				foreach (LinkRelationship linkRelationship in LinkRelationships)
				{
					foreach (LinkMapping linkMapping in linkRelationship.Mappings)
					{
						if (linkMapping.StagingColumn == null)
							throw new InvalidOperationException(nameof(linkMapping.StagingColumn));

						if (linkMapping.StagingColumn.Name == linkColumn.Name)
						{
							relationship = linkRelationship;

							break;
						}
					}

					if (relationship != null)
						break;
				}

				if (relationship != null)
				{
					string hashLength = "255";
					if (Model.Instance.DataWarehouse.UseNewHashLogic)
					{
						if (linkColumn.DataTypeStringLength > 4000)
						{
							hashLength = "MAX";
						}
						else
						{
							hashLength = linkColumn.DataTypeStringLength.ToString(CultureInfo.InvariantCulture);
						}
					}

					columnSQL = $"UPPER(CONVERT([nvarchar]({hashLength}), " + Converter.CheckSqlCompatibility(Converter.CheckKeyword(linkColumn.Name));
					if (linkColumn.DataType == SqlServerDataType.DateTime2)
					{
						columnSQL += ", 121))";
					}
					else if (linkColumn.DataType == SqlServerDataType.Date)
					{
						columnSQL += ", 120))";
					}
					else
					{
						columnSQL += "))";
					}

					if (linkColumn.DataType is SqlServerDataType.VarChar or SqlServerDataType.NVarChar)
					{
						columnSQL = Helper.GetTrimSql(columnSQL);
					}

					if (linkColumn.Nullable)
					{
						columnSQL = "COALESCE(" + columnSQL + ", '')";
					}

					if (firstRow == false)
					{
						columnSQL = " + '|' + " + columnSQL;
					}

					firstRow = false;
					linkHashkeySql += columnSQL;
				}
			}

			linkHashkeySql += ") AS BINARY(20)) AS " + linkName + "HashKey";

			return linkHashkeySql;
		}

		public string? GetFormattedLinkHashKeySql(int numTabs)
		{
			List<string> sql = new();
			List<string> relevantKeys = GetRelevantKeys();

			foreach (string relevantKey in relevantKeys)
			{
				if (relevantKey.Contains("HashKey"))
				{
					sql.Add(GetNamedLinkHashkeySql(relevantKey.Replace("HashKey", "")));
				}
				else if (relevantKey.Contains("DrivingKey"))
				{
					sql.Add(GetNamedLinkDrivingKeySql(relevantKey.Replace("DrivingKey", "")));
				}
			}

			if (sql.Count == 0)
			{
				return null;
			}

			return string.Join($",{Environment.NewLine}{new string('\t', numTabs)}", sql);
		}

		public string? GetFormattedHashDiffSql(bool useHubNames = false)
		{
			string satelliteHashkeySql = "CAST(HASHBYTES('SHA1', ";
			bool firstRow = true;
			string? columnSQL;
			List<StagingColumn> stagingColumns = new();

			if (Model.Instance.DataWarehouse.UseNewHashLogic)
			{
				stagingColumns = new List<StagingColumn>(StagingTable!.Columns);
			}
			else
			{
				if (BusinessKey != null)
				{
					if (BusinessKey.Name.StartsWith("H", StringComparison.Ordinal))
					{
						stagingColumns = GetHubandHubColumns()[BusinessKey.Name];

						if (stagingColumns.Count > 0)
						{
							stagingColumns.RemoveAll(column => column.GetHubColumnName(this) != BusinessKey.Name + "NaturalKey");
						}

						stagingColumns = StagingTable!.Columns.Except(stagingColumns).ToList();
					}

					if (BusinessKey.Name.StartsWith("L", StringComparison.Ordinal))
					{
						if (StagingTable != null && StagingTable.Columns != null)
						{
							foreach (StagingColumn column in StagingTable.Columns)
							{
								bool foundInLink = false;
								foreach (LinkRelationship linkRelationship in LinkRelationships)
								{
									foreach (LinkMapping mapping in linkRelationship.Mappings)
									{
										if (mapping.StagingColumn == column)
										{
											foundInLink = true;

											break;
										}
									}

									if (foundInLink)
										break;
								}

								bool foundInHub = false;
								foreach (HubRelationship hubRelationship in HubRelationships)
								{
									foreach (HubMapping mapping in hubRelationship.Mappings)
									{
										if (mapping.StagingColumn == column)
										{
											foundInHub = true;

											break;
										}
									}

									if (foundInHub)
										break;
								}

								if (BusinessKey.Name.StartsWith("L", StringComparison.Ordinal) && !foundInLink && (column.Satellite != null || foundInHub))
								{
									stagingColumns.Add(column);
								}
							}
						}
					}
				}
			}

			if (StagingTable != null && StagingTable.Columns != null)
			{
				foreach (StagingColumn column in stagingColumns)
				{
					columnSQL = Converter.CheckSqlCompatibility(Converter.CheckKeyword(column.Name));

					if (useHubNames)
					{
						List<StagingColumn> hubColumns = GetHubList();

						if (hubColumns.Count > 0)
						{
							foreach (StagingColumn hubColumn in hubColumns)
							{
								if (column.Name == hubColumn.Name)
								{
									// Overwrite columnSQL value with the column's hub column name.
									columnSQL = hubColumn.GetHubColumnName(this);
									break;
								}
							}
						}
					}

					if (column.DataType == SqlServerDataType.Decimal)
					{
						columnSQL = " REPLACE(RTRIM(REPLACE(" + columnSQL + ", '0', ' ')), ' ', '0')";
					}

					string hashLength = "255";
					if (Model.Instance.DataWarehouse.UseNewHashLogic)
					{
						if (column.DataTypeStringLength > 4000)
						{
							hashLength = "MAX";
						}
						else
						{
							hashLength = column.DataTypeStringLength.ToString(CultureInfo.InvariantCulture);
						}
					}

					columnSQL = $"UPPER(CONVERT([nvarchar]({hashLength}), " + columnSQL;
					if (column.DataType == SqlServerDataType.DateTime2)
					{
						columnSQL += ", 121))";
					}
					else if (column.DataType == SqlServerDataType.Date)
					{
						columnSQL += ", 120))";
					}
					else if (column.DataType == SqlServerDataType.Float)
					{
						columnSQL += ", 3))";
					}
					else
					{
						columnSQL += "))";
					}

					columnSQL = "COALESCE(" + columnSQL + ", '')";
					if (column.DataType is SqlServerDataType.VarChar or SqlServerDataType.NVarChar)
					{
						columnSQL = Helper.GetTrimSql(columnSQL);
					}

					if (firstRow == false)
					{
						columnSQL = " + '|' + " + columnSQL;
					}

					firstRow = false;
					satelliteHashkeySql += columnSQL;
				}

				satelliteHashkeySql += ") AS BINARY(20))";

				return satelliteHashkeySql;
			}
			else
			{
				return null;
			}
		}

		public int CompareTo(DataSource? other)
		{
			if (other == null)
				return -1;

			if (Name == other.Name)
				return 0;

			if (string.Compare(Name, other.Name, StringComparison.InvariantCulture) < 0)
				return -1;

			return 1;
		}

		public virtual bool IsValid(out string message)
		{
			message = string.Empty;

			if (string.IsNullOrWhiteSpace(Name))
			{
				message = "Data source does not have a name!";

				return false;
			}

			if (LoadTable != null)
			{
				foreach (Column column in LoadTable.Columns)
				{
					if (!column.IsValid(out string innerMessage))
					{
						message = innerMessage;

						return false;
					}
				}
			}

			if (StagingTable != null)
			{
				foreach (StagingColumn column in StagingTable.Columns)
				{
					if (!column.IsValid(out string innerMessage))
					{
						message = innerMessage;

						return false;
					}
				}
			}

			return true;
		}
	}
}
