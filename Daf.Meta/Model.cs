// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dahomey.Json;
using Dahomey.Json.Serialization.Conventions;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;

namespace Daf.Meta
{
	public class Model : PropertyChangedBaseClass
	{
		private static Model? _instance;

		private const string CurrentFormatVersion = "0.0.1";

		// Do not call this directly! This is required for JsonSerializer to work in .NET 5.0. Better solution to be implemented...
		public Model(string formatVersion)
		{
			FormatVersion = formatVersion;
		}

		public static Model Instance
		{
			get
			{
				if (_instance == null)
					throw new InvalidOperationException();

				return _instance;
			}
		}

		public string FormatVersion { get; set; }

		public DataWarehouse DataWarehouse { get; set; } = new DataWarehouse("Azure", new OleDBConnection("", ""), new OleDBConnection("", ""), new OleDBConnection("", ""), new OleDBConnection("", ""), new OleDBConnection("", ""), new OleDBConnection("", ""));

		[JsonIgnore]
		public ObservableCollection<Connection> Connections { get; } = new ObservableCollection<Connection>();

		[JsonIgnore]
		public ObservableCollection<SourceSystem> SourceSystems { get; } = new ObservableCollection<SourceSystem>();

		[JsonIgnore]
		public ObservableCollection<Tenant> Tenants { get; } = new ObservableCollection<Tenant>();

		[JsonIgnore]
		public ObservableCollection<DataSource> DataSources { get; } = new ObservableCollection<DataSource>();

		[JsonIgnore]
		public ObservableCollection<Hub> Hubs { get; } = new ObservableCollection<Hub>();

		[JsonIgnore]
		public ObservableCollection<Link> Links { get; } = new ObservableCollection<Link>();

		public void AddConnection(Connection connection)
		{
			if (connection == null)
				throw new ArgumentNullException($"Can't add a {nameof(Connection)} that is null.");

			connection.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("Connection");
			};

			Connections.Add(connection);
		}

		public void RemoveConnection(Connection connection)
		{
			if (connection == null)
				throw new ArgumentNullException($"Can't remove a {nameof(Connection)} that is null.");

			connection.ClearSubscribers();

			if (Connections.Contains(connection))
				Connections.Remove(connection);
			else
				throw new InvalidOperationException("Attempted to delete connection which does not exist in Model.Connections!");
		}

		public void AddSourceSystem(SourceSystem sourceSystem)
		{
			if (sourceSystem == null)
				throw new ArgumentNullException($"Can't add a {nameof(SourceSystem)} that is null.");

			sourceSystem.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("SourceSystem");
			};

			SourceSystems.Add(sourceSystem);
		}

		public void RemoveSourceSystem(SourceSystem sourceSystem)
		{
			if (sourceSystem == null)
				throw new ArgumentNullException($"Can't remove a {nameof(SourceSystem)} that is null.");

			sourceSystem.ClearSubscribers();

			SourceSystems.Remove(sourceSystem);
		}

		public void AddTenant(Tenant tenant)
		{
			if (tenant == null)
				throw new ArgumentNullException($"Can't add a {nameof(Tenant)} that is null.");

			tenant.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("Tenant");
			};

			Tenants.Add(tenant);
		}

		public void RemoveTenant(Tenant tenant)
		{
			if (tenant == null)
				throw new ArgumentNullException($"Can't remove a {nameof(Tenant)} that is null.");

			tenant.ClearSubscribers();

			Tenants.Remove(tenant);
		}

		public void AddDataSource(DataSource dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException($"Can't add a {nameof(DataSource)} that is null.");

			dataSource.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("DataSource");
			};

			DataSources.AddSorted(dataSource);
		}

		public static DataSource CopyDataSource(DataSource dataSource, string name, SourceSystem sourceSystem, Tenant tenant, Connection connection)
		{
			if (dataSource == null)
				throw new InvalidOperationException();

			DataSource clonedDataSource = dataSource.Clone();

			clonedDataSource.Name = name;
			clonedDataSource.SourceSystem = sourceSystem;
			clonedDataSource.Tenant = tenant;

			// Replace names with new ones
			clonedDataSource.FileName = $"{clonedDataSource.Tenant.ShortName}_{clonedDataSource.Name}";
			clonedDataSource.QualifiedName = $"{clonedDataSource.SourceSystem.ShortName}_{clonedDataSource.FileName}";

			foreach (Satellite cloneSatellite in clonedDataSource.Satellites)
			{
				cloneSatellite.Name = cloneSatellite.Name.Replace(dataSource.SourceSystem.ShortName, clonedDataSource.SourceSystem.ShortName);
				cloneSatellite.Name = cloneSatellite.Name.Replace(dataSource.FileName!, clonedDataSource.FileName);
			}

			// Can this be made into a switch? Seems not. Not all DataSources have Connections?
			if (clonedDataSource is RestDataSource restSource)
			{
				restSource.Connection = (RestConnection)connection!;
			}
			else if (clonedDataSource is GraphQlDataSource graphQlSource)
			{
				graphQlSource.Connection = (GraphQlConnection)connection!;
			}
			else if (clonedDataSource is SqlDataSource sqlSource)
			{
				sqlSource.Connection = (OleDBConnection)connection!;
			}

			return clonedDataSource;
		}

		public void RemoveDataSource(DataSource dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException($"Can't remove a {nameof(DataSource)} that is null.");

			dataSource.ClearSubscribers();

			DataSources.Remove(dataSource);
		}

		[JsonIgnore]
		public List<string> HubNames
		{
			get
			{
				List<string> hubNameList = new();

				if (Hubs != null)
				{
					foreach (Hub hub in Hubs)
					{
						hubNameList.Add(hub.Name);
					}
				}

				return hubNameList;
			}
		}

		/// <summary>
		/// Creates a new Hub and adds it at the 0th index in Model.Hubs.
		/// </summary>
		/// <param name="name">(Optional) Name of the new Hub. Default: "New Hub".</param>
		/// <returns>The Hub object that was created.</returns>
		public Hub AddHub(string name = "New Hub")
		{
			Hub hub = new(name);

			hub.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("Hub");
			};

			hub.BusinessKeys.CollectionChanged += (s, e) =>
			{
				hub.NotifyPropertyChanged("BusinessKeys");
			};

			Hubs.Insert(0, hub);

			return hub;
		}

		public void RemoveHub(Hub hub)
		{
			if (hub == null)
				throw new ArgumentNullException($"Can't remove a {nameof(Hub)} that is null.");

			hub.ClearSubscribers();

			Hubs.Remove(hub);
		}

		public static StagingColumn AddBusinessKeyToHub(Hub hub)
		{
			if (hub == null)
				throw new InvalidOperationException("Hub was null!");

			StagingColumn businessKey = hub.AddBusinessKeyColumn();

			return businessKey;
		}

		public static HubRelationship AddHubRelationship(Hub hub, DataSource dataSource)
		{
			if (hub == null || dataSource == null)
				throw new InvalidOperationException("Hub or DataSource was null!");

			HubRelationship hubRelationship = new(hub);

			foreach (StagingColumn bk in hub.BusinessKeys)
			{
				HubMapping hubMapping = new(bk);

				hubMapping.PropertyChanged += (s, e) =>
				{
					hubRelationship.NotifyPropertyChanged("HubMapping");
				};

				hubMapping.ChangedStagingColumn += (s, e) =>
				{
					// I dislike having this method be internal instead of private. Need advice on how to do it better.
					// TODO: Look closer into this and whether these should be separate events.
					dataSource.GetColumnsNotInHubsOrLinks();
				};

				hubRelationship.Mappings.Add(hubMapping);
			}

			hubRelationship.PropertyChanged += (s, e) =>
			{
				dataSource.NotifyPropertyChanged("HubRelationship");
			};

			dataSource.HubRelationships.Add(hubRelationship);

			return hubRelationship;
		}

		public static void RemoveHubRelationship(HubRelationship hubRelationship, DataSource dataSource)
		{
			if (hubRelationship == null || dataSource == null)
				throw new InvalidOperationException("Hub or DataSource was null!");
			else
			{
				foreach (HubMapping hubMapping in hubRelationship.Mappings)
				{
					hubMapping.ClearSubscribers();
				}

				hubRelationship.ClearSubscribers();

				dataSource.HubRelationships.Remove(hubRelationship);

				hubRelationship.Unsubscribe();

				// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
				//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
			}
		}

		public static LinkRelationship AddLinkRelationship(Link link, DataSource dataSource)
		{
			if (link == null || dataSource == null)
				throw new InvalidOperationException("Link or DataSource was null!");

			LinkRelationship linkRelationship = new(link);

			foreach (StagingColumn bk in link.BusinessKeys)
			{
				LinkMapping linkMapping = new(bk);

				linkMapping.PropertyChanged += (s, e) =>
				{
					linkRelationship.NotifyPropertyChanged("LinkMapping");
				};

				linkMapping.ChangedStagingColumn += (s, e) =>
				{
					// I dislike having this method be internal instead of private. Need advice on how to do it better.
					dataSource.GetColumnsNotInHubsOrLinks();
				};

				linkRelationship.Mappings.Add(linkMapping);
			}

			linkRelationship.PropertyChanged += (s, e) =>
			{
				dataSource.NotifyPropertyChanged("LinkRelationship");
			};

			dataSource.LinkRelationships.Add(linkRelationship);

			return linkRelationship;
		}

		public static void RemoveLinkRelationship(LinkRelationship linkRelationship, DataSource dataSource)
		{
			if (linkRelationship == null || dataSource == null)
				throw new InvalidOperationException("Link or DataSource was null!");
			else
			{
				foreach (LinkMapping linkMapping in linkRelationship.Mappings)
				{
					linkMapping.ClearSubscribers();
				}

				linkRelationship.ClearSubscribers();

				dataSource.LinkRelationships.Remove(linkRelationship);

				linkRelationship.Unsubscribe();

				// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
				//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
			}
		}

		[JsonIgnore]
		public List<string> LinkNames
		{
			get
			{
				List<string> linkNameList = new();

				if (Links != null)
				{
					foreach (Link link in Links)
					{
						linkNameList.Add(link.Name);
					}
				}

				return linkNameList;
			}
		}

		/// <summary>
		/// Creates a new Link and adds it at the 0th index in Model.Links.
		/// </summary>
		/// <param name="name">(Optional) Name of the new Link. Default: "New Link".</param>
		/// <returns>The Link object that was created.</returns>
		public Link AddLink(string name = "New Link")
		{
			Link link = new(name);

			link.PropertyChanged += (s, e) =>
			{
				Instance.NotifyPropertyChanged("Link");
			};

			link.BusinessKeys.CollectionChanged += (s, e) =>
			{
				link.NotifyPropertyChanged("BusinessKeys");
			};

			Links.Insert(0, link);

			return link;
		}

		public void RemoveLink(Link link)
		{
			if (link == null)
				throw new ArgumentNullException($"Can't remove a {nameof(Link)} that is null.");

			link.ClearSubscribers();

			Links.Remove(link);
		}

		public Connection? GetConnection(string? connectionName)
		{
			if (Connections == null)
				throw new InvalidOperationException();

			foreach (Connection c in Connections)
			{
				if (c.Name == connectionName)
				{
					return c;
				}
			}

			return null;
		}

		public Hub? GetHub(string? hubName)
		{
			if (Hubs == null)
				throw new InvalidOperationException();

			foreach (Hub h in Hubs)
			{
				if (h.Name == hubName)
				{
					return h;
				}
			}

			return null;
		}

		public Link? GetLink(string linkName)
		{
			if (Links == null)
				throw new InvalidOperationException();

			foreach (Link l in Links)
			{
				if (l.Name == linkName)
				{
					return l;
				}
			}

			return null;
		}

		public StagingColumn? GetBusinessKey(string businessKeyName)
		{
			if (Hubs == null || Links == null)
				throw new InvalidOperationException();

			foreach (Link l in Links)
			{
				if (l.Name == businessKeyName)
				{
					return new StagingColumn(name: l.Name + "HashKey") { DataType = SqlServerDataType.Binary, Length = 20, Precision = null, Scale = null, Nullable = false };
				}
			}

			foreach (Hub h in Hubs)
			{
				if (h.Name == businessKeyName)
				{
					return h.BusinessKeys[0];
				}
			}

			return null;
		}

		public static void Serialize(string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			string json = JsonSerializer.Serialize(Instance, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "Model.json"), normalizedJson);

			SerializeSourceSystems(Instance, folder);
			SerializeTenants(Instance, folder);
			SerializeConnections(Instance, folder);
			SerializeHubs(Instance, folder);
			SerializeLinks(Instance, folder);
			SerializeDataSources(Instance, folder);
		}

		public static void Deserialize(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "Model.json"));

			Json.Model jsonModel = JsonSerializer.Deserialize<Json.Model>(json, options)!;
			jsonModel.SourceSystems = DeserializeSourceSystems(folder);
			jsonModel.Tenants = DeserializeTenants(folder);
			jsonModel.Connections = DeserializeConnections(folder);
			jsonModel.Hubs = DeserializeHubs(folder);
			jsonModel.Links = DeserializeLinks(folder);
			jsonModel.DataSources = DeserializeDataSources(folder);

			// Begin loops here
			_instance = new Model(jsonModel.FormatVersion)
			{
				DataWarehouse = jsonModel.DataWarehouse
			};

			foreach (SourceSystem sourceSystem in jsonModel.SourceSystems)
				_instance.SourceSystems.Add(sourceSystem);

			foreach (Tenant tenant in jsonModel.Tenants)
				_instance.Tenants.Add(tenant);

			foreach (Hub hub in jsonModel.Hubs)
				_instance.Hubs.Add(hub);

			foreach (Link link in jsonModel.Links)
				_instance.Links.Add(link);

			foreach (Hub hub in _instance.Hubs)
			{
				hub.BusinessKeys.CollectionChanged += (s, e) =>
				{
					hub.NotifyPropertyChanged("BusinessKeys");
				};

				foreach (StagingColumn stagingColumn in hub.BusinessKeys)
				{
					stagingColumn.PropertyChanged += (s, e) =>
					{
						hub.NotifyPropertyChanged("StagingColumn");
					};
				}
			}

			foreach (Link link in _instance.Links)
			{
				link.BusinessKeys.CollectionChanged += (s, e) =>
				{
					link.NotifyPropertyChanged("BusinessKeys");
				};

				foreach (StagingColumn stagingColumn in link.BusinessKeys)
				{
					stagingColumn.PropertyChanged += (s, e) =>
					{
						link.NotifyPropertyChanged("StagingColumn");
					};
				}
			}

			_instance.DataWarehouse.PropertyChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("DataWarehouse");
			};

			_instance.SourceSystems.CollectionChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("SourceSystems");
			};

			foreach (SourceSystem sourceSystem in _instance.SourceSystems)
			{
				sourceSystem.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("SourceSystem");
				};
			}

			_instance.Tenants.CollectionChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("Tenants");
			};

			foreach (Tenant tenant in _instance.Tenants)
			{
				tenant.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("Tenant");
				};
			}

			_instance.Hubs.CollectionChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("Hubs");
			};

			foreach (Hub hub in _instance.Hubs)
			{
				hub.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("Hub");
				};
			}

			_instance.Links.CollectionChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("Links");
			};

			foreach (Link link in _instance.Links)
			{
				link.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("Link");
				};
			}

			_instance.DataSources.CollectionChanged += (s, e) =>
			{
				_instance.NotifyPropertyChanged("DataSources");
			};

			foreach (Json.Connection jsonConnection in jsonModel.Connections)
			{
				Connection connection;

				switch (jsonConnection.DerivedType)
				{
					case "OleDB":
						connection = new OleDBConnection(name: jsonConnection.Name, connectionString: jsonConnection.ConnectionString!)
						{
							ConnectionType = jsonConnection.ConnectionType
						};
						break;
					case "MySql":
						connection = new MySqlConnection(name: jsonConnection.Name, connectionString: jsonConnection.ConnectionString!)
						{
							ConnectionType = jsonConnection.ConnectionType
						};
						break;
					case "Rest":
						connection = new RestConnection(name: jsonConnection.Name)
						{
							ConnectionType = jsonConnection.ConnectionType,
							BaseUrl = jsonConnection.BaseUrl,
							EncryptedCredential = jsonConnection.EncryptedCredential,
							Authorization = (HttpAuthorization)jsonConnection.Authorization!,
							RestUser = jsonConnection.RestUser,
							Password = jsonConnection.Password,
							TokenAbsoluteUrl = jsonConnection.TokenAbsoluteUrl,
							TokenJsonIdentifier = jsonConnection.TokenJsonIdentifier,
							ClientID = jsonConnection.ClientID,
							ClientSecret = jsonConnection.ClientSecret,
							GrantType = jsonConnection.GrantType ?? GrantType.Password
						};

						if (jsonConnection.TokenBody != null)
							((RestConnection)connection).TokenBody.AddRange(jsonConnection.TokenBody);

						if (jsonConnection.TokenParameters != null)
							((RestConnection)connection).TokenParameters.AddRange(jsonConnection.TokenParameters);

						break;
					case "GraphQl":
						connection = new GraphQlConnection(name: jsonConnection.Name)
						{
							ConnectionType = jsonConnection.ConnectionType,
							BaseUrl = jsonConnection.BaseUrl,
							EncryptedCredential = jsonConnection.EncryptedCredential,
							AuthorizationToken = jsonConnection.AuthorizationToken
						};
						break;
					case "Odbc":
						connection = new OdbcConnection(name: jsonConnection.Name, connectionString: jsonConnection.ConnectionString!)
						{
							ConnectionType = jsonConnection.ConnectionType
						};
						break;
					default:
						throw new InvalidOperationException("Invalid connection in metadata.");
				}

				connection.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("Connection");
				};

				_instance.Connections.Add(connection);
			}

			foreach (Json.DataSource jsonDataSource in jsonModel.DataSources)
			{
				DataSource dataSource;

				Connection? connection = null;
				SourceSystem? sourceSystem = null;
				Tenant? tenant = null;

				foreach (Connection con in _instance.Connections)
				{
					if (con.Name == jsonDataSource.Connection)
					{
						connection = con;
						break;
					}
				}

				foreach (SourceSystem srcSystem in _instance.SourceSystems)
				{
					if (srcSystem.Name == jsonDataSource.SourceSystem)
					{
						sourceSystem = srcSystem;
						break;
					}
				}

				foreach (Tenant ten in _instance.Tenants)
				{
					if (ten.Name == jsonDataSource.Tenant)
					{
						tenant = ten;
						break;
					}
				}

				switch (jsonDataSource.DerivedType)
				{
					case "FlatFile":
						dataSource = new FlatFileDataSource(jsonDataSource.Name, sourceSystem!, tenant!)
						{
							DataSourceType = jsonDataSource.DataSourceType,
							DefaultLoadWidth = jsonDataSource.DefaultLoadWidth,
							GenerateLatestViews = jsonDataSource.GenerateLatestViews,
							FileName = jsonDataSource.FileName,
							Format = jsonDataSource.Format,
							RowDelimiter = jsonDataSource.RowDelimiter,
							ColumnDelimiter = jsonDataSource.ColumnDelimiter,
							TextQualified = jsonDataSource.TextQualified,
							TextQualifier = jsonDataSource.TextQualifier,
							HeadersInFirstRow = jsonDataSource.HeadersInFirstRow,
							BusinessDateOffset = jsonDataSource.BusinessDateOffset,
							SqlSelectQuery = jsonDataSource.SqlSelectQuery,
							Build = jsonDataSource.Build,
							ErrorHandling = jsonDataSource.ErrorHandling,
							FileDateRegex = jsonDataSource.FileDateRegex,
							CodePage = jsonDataSource.CodePage,
							IncrementalStagingColumn = jsonDataSource.IncrementalStagingColumn,
							IncrementalQuery = jsonDataSource.IncrementalQuery,
							BusinessDateColumn = jsonDataSource.BusinessDateColumn,
							DestinationType = jsonDataSource.DestinationType
						};
						break;
					case "Sql":
						dataSource = new SqlDataSource(jsonDataSource.Name, connection!, sourceSystem!, tenant!)
						{
							DataSourceType = jsonDataSource.DataSourceType,
							DefaultLoadWidth = jsonDataSource.DefaultLoadWidth,
							GenerateLatestViews = jsonDataSource.GenerateLatestViews,
							FileName = jsonDataSource.FileName,
							SqlStatement = jsonDataSource.SqlStatement,
							SqlReadyCondition = jsonDataSource.SqlReadyCondition,
							SqlSelectQuery = jsonDataSource.SqlSelectQuery,
							Build = jsonDataSource.Build,
							ErrorHandling = jsonDataSource.ErrorHandling,
							IncrementalStagingColumn = jsonDataSource.IncrementalStagingColumn,
							IncrementalQuery = jsonDataSource.IncrementalQuery,
							BusinessDateColumn = jsonDataSource.BusinessDateColumn,
							ConnectionRetryAttempts = jsonDataSource.ConnectionRetryAttempts,
							ConnectionRetryMinutes = jsonDataSource.ConnectionRetryMinutes,
							TableName = jsonDataSource.TableName,
							DestinationType = jsonDataSource.DestinationType,
							AzureLinkedServiceReference = jsonDataSource.AzureLinkedServiceReference
						};
						break;
					case "Rest":
						dataSource = new RestDataSource(jsonDataSource.Name, (RestConnection)connection!, sourceSystem!, tenant!)
						{
							DataSourceType = jsonDataSource.DataSourceType,
							DefaultLoadWidth = jsonDataSource.DefaultLoadWidth,
							GenerateLatestViews = jsonDataSource.GenerateLatestViews,
							FileName = jsonDataSource.FileName,
							SqlSelectQuery = jsonDataSource.SqlSelectQuery,
							Build = jsonDataSource.Build,
							ErrorHandling = jsonDataSource.ErrorHandling,
							IncrementalStagingColumn = jsonDataSource.IncrementalStagingColumn,
							IncrementalQuery = jsonDataSource.IncrementalQuery,
							IncrementalExpression = jsonDataSource.IncrementalExpression,
							MergeToBlob = jsonDataSource.MergeToBlob ?? false,
							DestinationEncoding = jsonDataSource.DestinationEncoding,
							BusinessDateColumn = jsonDataSource.BusinessDateColumn,
							ConnectionRetryAttempts = jsonDataSource.ConnectionRetryAttempts,
							ConnectionRetryMinutes = jsonDataSource.ConnectionRetryMinutes,
							RelativeUrl = jsonDataSource.RelativeUrl,
							SaveCookie = jsonDataSource.SaveCookie ?? false,
							CustomScriptPath = jsonDataSource.CustomScriptPath,
							CollectionReference = jsonDataSource.CollectionReference,
							PaginationNextLink = jsonDataSource.PaginationNextLink,
							PaginationLinkIsRelative = jsonDataSource.PaginationLinkIsRelative ?? false,
							DestinationType = jsonDataSource.DestinationType
						};

						if (jsonDataSource.Parameters != null)
							((RestDataSource)dataSource).Parameters.AddRange(jsonDataSource.Parameters);

						break;
					case "Script":
						dataSource = new ScriptDataSource(jsonDataSource.Name, sourceSystem!, tenant!)
						{
							DataSourceType = jsonDataSource.DataSourceType,
							DefaultLoadWidth = jsonDataSource.DefaultLoadWidth,
							GenerateLatestViews = jsonDataSource.GenerateLatestViews,
							FileName = jsonDataSource.FileName,
							SqlSelectQuery = jsonDataSource.SqlSelectQuery,
							Build = jsonDataSource.Build,
							ErrorHandling = jsonDataSource.ErrorHandling,
							IncrementalStagingColumn = jsonDataSource.IncrementalStagingColumn,
							IncrementalQuery = jsonDataSource.IncrementalQuery,
							BusinessDateColumn = jsonDataSource.BusinessDateColumn,
							DestinationType = jsonDataSource.DestinationType
						};
						break;
					case "GraphQl":
						dataSource = new GraphQlDataSource(jsonDataSource.Name, (GraphQlConnection)connection!, sourceSystem!, tenant!)
						{
							DataSourceType = jsonDataSource.DataSourceType,
							DefaultLoadWidth = jsonDataSource.DefaultLoadWidth,
							GenerateLatestViews = jsonDataSource.GenerateLatestViews,
							FileName = jsonDataSource.FileName,
							SqlSelectQuery = jsonDataSource.SqlSelectQuery,
							ErrorHandling = jsonDataSource.ErrorHandling,
							IncrementalStagingColumn = jsonDataSource.IncrementalStagingColumn,
							IncrementalQuery = jsonDataSource.IncrementalQuery,
							MergeToBlob = jsonDataSource.MergeToBlob ?? false,
							DestinationEncoding = jsonDataSource.DestinationEncoding,
							BusinessDateColumn = jsonDataSource.BusinessDateColumn,
							ConnectionRetryAttempts = jsonDataSource.ConnectionRetryAttempts,
							ConnectionRetryMinutes = jsonDataSource.ConnectionRetryMinutes,
							RelativeUrl = jsonDataSource.RelativeUrl,
							CollectionReference = jsonDataSource.CollectionReference,
							Parent = jsonDataSource.Parent,
							NumberToFetch = (int)jsonDataSource.NumberToFetch!,
							GraphQlQuery = jsonDataSource.GraphQlQuery!,
							DestinationType = jsonDataSource.DestinationType
						};
						break;
					default:
						throw new InvalidOperationException("Invalid data source in metadata.");
				};

				if (jsonDataSource.BusinessKey != null && jsonDataSource.BusinessKey.StartsWith("H_", StringComparison.Ordinal))
				{
					dataSource.BusinessKey = _instance.GetHub(jsonDataSource.BusinessKey);
				}
				else if (jsonDataSource.BusinessKey != null && jsonDataSource.BusinessKey.StartsWith("L_", StringComparison.Ordinal))
				{
					dataSource.BusinessKey = _instance.GetLink(jsonDataSource.BusinessKey);
				}

				foreach (Satellite satellite in jsonDataSource.Satellites)
				{
					dataSource.Satellites.Add(satellite);
				}

				foreach (Satellite satellite in dataSource.Satellites)
				{
					satellite.PropertyChanged += (s, e) =>
					{
						dataSource.NotifyPropertyChanged("Satellite");
					};
				}

				dataSource.Satellites.CollectionChanged += (s, e) =>
				{
					dataSource.NotifyPropertyChanged("Satellites");
				};

				dataSource.LoadTable = jsonDataSource.LoadTable;

				if (dataSource.LoadTable != null)
				{
					foreach (Column column in dataSource.LoadTable.Columns)
					{
						column.PropertyChanged += (s, e) =>
						{
							dataSource.NotifyPropertyChanged("LoadColumn");
						};
					}
				}

				StagingTable stagingTable = new();

				if (jsonDataSource.StagingTable != null)
				{
					foreach (Json.StagingColumn jsonColumn in jsonDataSource.StagingTable.Columns)
					{
						StagingColumn stagingColumn = new(name: jsonColumn.Name)
						{
							DataType = jsonColumn.DataType,
							Length = jsonColumn.Length,
							Logic = jsonColumn.Logic,
							Nullable = jsonColumn.Nullable,
							Precision = jsonColumn.Precision,
							Scale = jsonColumn.Scale
						};

						if (jsonColumn.ExtendedProperties != null)
						{
							stagingColumn.ExtendedProperties = new Dictionary<string, string>();

							foreach (KeyValuePair<string, string> extendedProperty in jsonColumn.ExtendedProperties)
							{
								stagingColumn.ExtendedProperties.Add(extendedProperty.Key, extendedProperty.Value);
							}
						}

						foreach (Satellite satellite in dataSource.Satellites)
						{
							if (satellite.Name == jsonColumn.Satellite)
							{
								stagingColumn.Satellite = satellite;
								break;
							}
						}

						// Find the load column that the staging column is dependent on.
						foreach (Column loadColumn in dataSource.LoadTable!.Columns)
						{
							if (loadColumn.Name == jsonColumn.LoadColumn)
								stagingColumn.LoadColumn = loadColumn;
						}

						stagingColumn.PropertyChanged += (s, e) =>
						{
							dataSource.NotifyPropertyChanged("StagingColumn");
						};

						stagingTable.Columns.Add(stagingColumn);
					}
				}

				// Need this to trigger the setter and event AFTER the StagingTable has been populated.
				dataSource.StagingTable = stagingTable;

				foreach (Json.HubRelationship jsonHubRelationship in jsonDataSource.HubRelationships)
				{
					foreach (Hub hub in _instance.Hubs)
					{
						if (hub.Name == jsonHubRelationship.Hub)
						{
							HubRelationship hubRelationship = new(hub);

							foreach (StagingColumn bkColumn in hub.BusinessKeys)
							{
								foreach (Json.HubMapping jsonHubMapping in jsonHubRelationship.Mappings)
								{
									if (bkColumn.Name == jsonHubMapping.HubColumn)
									{
										HubMapping hubMapping = new(bkColumn);

										// Find the staging column that the mapping is dependent on.
										foreach (StagingColumn stagingColumn in dataSource.StagingTable.Columns)
										{
											if (stagingColumn.Name == jsonHubMapping.StagingColumn)
											{
												hubMapping.StagingColumn = stagingColumn;
												break;
											}
										}

										hubMapping.PropertyChanged += (s, e) =>
										{
											hubRelationship.NotifyPropertyChanged("HubMapping");
										};

										hubMapping.ChangedStagingColumn += (s, e) =>
										{
											dataSource.GetColumnsNotInHubsOrLinks();
										};

										hubRelationship.Mappings.Add(hubMapping);

										break;
									}
								}
							}

							hubRelationship.PropertyChanged += (s, e) =>
							{
								dataSource.NotifyPropertyChanged("HubRelationship");
							};

							dataSource.HubRelationships.Add(hubRelationship);

							break;
						}
					}
				}

				dataSource.HubRelationships.CollectionChanged += (s, e) =>
				{
					dataSource.NotifyPropertyChanged("HubRelationships");
				};

				foreach (Json.LinkRelationship jsonLinkRelationship in jsonDataSource.LinkRelationships)
				{
					foreach (Link link in _instance.Links)
					{
						if (link.Name == jsonLinkRelationship.Link)
						{
							LinkRelationship linkRelationship = new(link);

							foreach (StagingColumn bkColumn in link.BusinessKeys)
							{
								foreach (Json.LinkMapping jsonLinkMapping in jsonLinkRelationship.Mappings)
								{
									if (bkColumn.Name == jsonLinkMapping.LinkColumn)
									{
										LinkMapping linkMapping = new(bkColumn);

										// Find the staging column that the mapping is dependent on.
										foreach (StagingColumn stagingColumn in dataSource.StagingTable.Columns)
										{
											if (stagingColumn.Name == jsonLinkMapping.StagingColumn)
											{
												linkMapping.StagingColumn = stagingColumn;
												break;
											}
										}

										linkMapping.PropertyChanged += (s, e) =>
										{
											linkRelationship.NotifyPropertyChanged("LinkMapping");
										};

										linkMapping.ChangedStagingColumn += (s, e) =>
										{
											dataSource.GetColumnsNotInHubsOrLinks();
										};

										linkRelationship.Mappings.Add(linkMapping);

										break;
									}
								}
							}

							linkRelationship.PropertyChanged += (s, e) =>
							{
								dataSource.NotifyPropertyChanged("LinkRelationship");
							};

							dataSource.LinkRelationships.Add(linkRelationship);

							break;
						}
					}
				}

				dataSource.LinkRelationships.CollectionChanged += (s, e) =>
				{
					dataSource.NotifyPropertyChanged("LinkRelationships");
				};

				dataSource.PropertyChanged += (s, e) =>
				{
					_instance.NotifyPropertyChanged("DataSource");
				};

				_instance.DataSources.Add(dataSource);
			}
		}

		private static void SerializeSourceSystems(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			List<SourceSystem> sortedList = new(value.SourceSystems);
			sortedList.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.InvariantCulture));

			string json = JsonSerializer.Serialize(sortedList, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "SourceSystems.json"), normalizedJson);
		}

		private static SourceSystem[] DeserializeSourceSystems(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "SourceSystems.json"));

			return JsonSerializer.Deserialize<SourceSystem[]>(json, options)!;
		}

		private static void SerializeTenants(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			List<Tenant> sortedList = new(value.Tenants);
			sortedList.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.InvariantCulture));

			string json = JsonSerializer.Serialize(sortedList, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "Tenants.json"), normalizedJson);
		}

		private static Tenant[] DeserializeTenants(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "Tenants.json"));

			return JsonSerializer.Deserialize<Tenant[]>(json, options)!;
		}

		private static void SerializeConnections(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
			registry.ClearConventions();
			registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "DerivedType"));
			registry.RegisterType<RestConnection>();
			registry.RegisterType<MySqlConnection>();
			registry.RegisterType<OleDBConnection>();
			registry.RegisterType<GraphQlConnection>();
			registry.RegisterType<OdbcConnection>();

			options.Converters.Add(new JsonStringEnumConverter());

			List<Connection> sortedList = new(value.Connections);
			sortedList.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.InvariantCulture));

			string json = JsonSerializer.Serialize(sortedList, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "Connections.json"), normalizedJson);
		}

		private static Json.Connection[] DeserializeConnections(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
			registry.ClearConventions();
			registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "DerivedType"));
			registry.RegisterType<RestConnection>();
			registry.RegisterType<MySqlConnection>();
			registry.RegisterType<OleDBConnection>();
			registry.RegisterType<GraphQlConnection>();

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "Connections.json"));

			return JsonSerializer.Deserialize<Json.Connection[]>(json, options)!;
		}

		private static void SerializeDataSources(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.SetupExtensions();

			DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
			registry.ClearConventions();
			registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "DerivedType"));
			registry.RegisterType<FlatFileDataSource>();
			registry.RegisterType<SqlDataSource>();
			registry.RegisterType<RestDataSource>();
			registry.RegisterType<ScriptDataSource>();
			registry.RegisterType<GraphQlDataSource>();

			options.Converters.Add(new JsonStringEnumConverter());

			foreach (DataSource ds in value.DataSources)
			{
				Directory.CreateDirectory(Path.Combine(folder, "DataSources", ds.SourceSystem.ShortName, ds.Tenant.ShortName));

				string json = JsonSerializer.Serialize(ds, options);
				string normalizedJson = JsonNormalizer.Normalize(json);
				File.WriteAllText(Path.Combine(folder, "DataSources", ds.SourceSystem.ShortName, ds.Tenant.ShortName, $"{ds.Name}.json"), normalizedJson);
			}
		}

		private static Json.DataSource[] DeserializeDataSources(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			//options.SetupExtensions();

			//DiscriminatorConventionRegistry registry = options.GetDiscriminatorConventionRegistry();
			//registry.ClearConventions();
			//registry.RegisterConvention(new DefaultDiscriminatorConvention<string>(options, "DerivedType"));
			//registry.RegisterType<FlatFileDataSource>();
			//registry.RegisterType<SqlDataSource>();
			//registry.RegisterType<RestDataSource>();
			//registry.RegisterType<ScriptDataSource>();

			options.Converters.Add(new JsonStringEnumConverter());

			List<Json.DataSource> dataSources = new();

			foreach (string file in Directory.GetFiles(Path.Combine(folder, "DataSources"), "*.json", SearchOption.AllDirectories))
			{
				string json = File.ReadAllText(file);
				dataSources.Add(JsonSerializer.Deserialize<Json.DataSource>(json, options)!);
			}

			return dataSources.ToArray();
		}

		private static void SerializeHubs(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.Converters.Add(new JsonStringEnumConverter());

			List<Hub> sortedList = new(value.Hubs);
			sortedList.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.InvariantCulture));

			string json = JsonSerializer.Serialize(sortedList, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "Hubs.json"), normalizedJson);
		}

		private static Hub[] DeserializeHubs(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "Hubs.json"));

			return JsonSerializer.Deserialize<Hub[]>(json, options)!;
		}

		private static void SerializeLinks(Model value, string folder)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true
			};

			options.Converters.Add(new JsonStringEnumConverter());

			List<Link> sortedList = new(value.Links);
			sortedList.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.InvariantCulture));

			string json = JsonSerializer.Serialize(sortedList, options);
			string normalizedJson = JsonNormalizer.Normalize(json);
			File.WriteAllText(Path.Combine(folder, "Links.json"), normalizedJson);
		}

		private static Link[] DeserializeLinks(string folder)
		{
			var options = new JsonSerializerOptions
			{
				AllowTrailingCommas = true,
				IgnoreNullValues = true
			};

			options.Converters.Add(new JsonStringEnumConverter());

			string json = File.ReadAllText(Path.Combine(folder, "Links.json"));

			return JsonSerializer.Deserialize<Link[]>(json, options)!;
		}

		// TODO: Remove this. Only used temporarily for CsvConverter.
		public static void Initialize()
		{
			_instance = new Model(CurrentFormatVersion);
		}
	}
}
