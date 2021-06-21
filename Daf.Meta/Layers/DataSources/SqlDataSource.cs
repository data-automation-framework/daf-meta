// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;
using Microsoft.Data.SqlClient;
using Daf.Meta.Interfaces;
using Daf.Meta.JsonConverters;
using Daf.Meta.Layers.Connections;

namespace Daf.Meta.Layers.DataSources
{
	[JsonDiscriminator("Sql")]
	public class SqlDataSource : DataSource, IConnection
	{
		public SqlDataSource(string name, Connection connection, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
			_connection = connection;
		}

		[JsonIgnore]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<Connection> Connections { get { return Model.Instance.Connections; } }

		private Connection _connection;

		[JsonConverter(typeof(ConnectionConverter))]
		public Connection Connection
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

		private string? _tableName;

		public string? TableName
		{
			get { return _tableName; }
			set
			{
				if (_tableName != value)
				{
					_tableName = value;

					NotifyPropertyChanged("TableName");
				}
			}
		}

		private string? _sqlStatement;

		public string? SqlStatement
		{
			get { return _sqlStatement; }
			set
			{
				if (_sqlStatement != value)
				{
					_sqlStatement = value;

					NotifyPropertyChanged("SqlStatement");
				}
			}
		}

		private string? _sqlReadyCondition;

		[DataType(DataType.MultilineText)]
		public string? SqlReadyCondition
		{
			get { return _sqlReadyCondition; }
			set
			{
				if (_sqlReadyCondition != value)
				{
					if (string.IsNullOrEmpty(value))
						_sqlReadyCondition = null;
					else
						_sqlReadyCondition = value;

					NotifyPropertyChanged("SqlReadyCondition");
				}
			}
		}

		public override DataSource Clone()
		{
			SqlDataSource clone = new(string.Empty, Connection, SourceSystem, Tenant)
			{
				LoadTable = new LoadTable()
			};

			if (LoadTable != null)
			{
				foreach (Column column in LoadTable.Columns)
				{
					Column cloneColumn = new(column.Name)
					{
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

					clone.LoadTable!.Columns.Add(cloneColumn);
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
					StagingColumn cloneColumn = stagingColumn.Clone(clone.LoadTable!, clone.Satellites);

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
			clone.ContainsMultiStructuredJson = ContainsMultiStructuredJson;
			clone.DataSourceType = DataSourceType;
			clone.DefaultLoadWidth = DefaultLoadWidth;
			clone.ErrorHandling = ErrorHandling;
			clone.FileName = FileName;
			clone.IncrementalStagingColumn = IncrementalStagingColumn;
			clone.IncrementalQuery = IncrementalQuery;
			clone.QualifiedName = QualifiedName;
			clone.SqlSelectQuery = SqlSelectQuery;
			clone.SqlStatement = SqlStatement;
			clone.ConnectionRetryAttempts = ConnectionRetryAttempts;
			clone.ConnectionRetryMinutes = ConnectionRetryMinutes;
			clone.GenerateLatestViews = GenerateLatestViews;

			return clone;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The user needs to be able to run any type of query.")]
		public override void GetMetadata()
		{
			if (SqlStatement == null)
				throw new InvalidOperationException();

			if (Connection is OleDBConnection oledbConnection)
			{
				// HACK. How should this be handled instead? Maybe we should rebuild the connection string from scratch,
				// with confirmed required parameters.
				string connectionString = oledbConnection.ConnectionString.Replace("Provider=MSOLEDBSQL.1;", "").Replace("Auto Translate=True", "");

				using (SqlConnection connection = new(connectionString))
				{
					connection.Open();

					string sql = SqlStatement;

					DataTable schema;

					using (SqlCommand command = new(sql, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
						{
							schema = reader.GetSchemaTable();
						}
					}

					List<Column> suggestedColumns = GetSqlList(schema);

					Dictionary<string, Column> suggestedColumnDict = new();

					foreach (Column column in suggestedColumns)
						suggestedColumnDict.Add(column.Name, column);

					UpdateTables(suggestedColumnDict);
				}
			}
			else if (Connection is Connections.OdbcConnection odbcConnection)
			{
				string connectionString = odbcConnection.ConnectionString;

				using (System.Data.Odbc.OdbcConnection connection = new(connectionString))
				{
					connection.Open();

					string sql = SqlStatement;

					DataTable? schema;

					using (OdbcCommand command = new(sql, connection))
					using (OdbcDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
					{
						schema = reader.GetSchemaTable();
					}

					if (schema != null)
					{
						List<Column> suggestedColumns = GetOdbcList(schema);

						Dictionary<string, Column> suggestedColumnDict = new();

						foreach (Column column in suggestedColumns)
							suggestedColumnDict.Add(column.Name, column);

						UpdateTables(suggestedColumnDict);
					}
					else
					{
						throw new ArgumentNullException($"Failed to retrieve schema from connection {odbcConnection.Name}.");
					}
				}
			}
		}

		public new void UpdateTables(Dictionary<string, Column> suggestedColumnDict)
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
						DataType = newColumn.DataType,
						Length = newColumn.Length,
						Precision = newColumn.Precision,
						Scale = newColumn.Scale,
						Nullable = newColumn.Nullable
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
	}
}
