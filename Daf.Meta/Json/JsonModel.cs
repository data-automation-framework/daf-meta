// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Meta.Json
{
	public class Model
	{
		public Model
		(
			string formatVersion, DataWarehouse dataWarehouse, Layers.SourceSystem[] sourceSystems, Layers.Tenant[] tenants,
			Connection[] connections, Layers.Hub[] hubs, Layers.Link[] links, DataSource[] dataSources
		)
		{
			FormatVersion = formatVersion;
			DataWarehouse = dataWarehouse;
			SourceSystems = sourceSystems;
			Tenants = tenants;
			Connections = connections;
			Hubs = hubs;
			Links = links;
			DataSources = dataSources;
		}

		public string FormatVersion { get; set; }
		public DataWarehouse DataWarehouse { get; set; }
		public Layers.SourceSystem[] SourceSystems { get; set; }
		public Layers.Tenant[] Tenants { get; set; }
		public Connection[] Connections { get; set; }
		public Layers.Hub[] Hubs { get; set; }
		public Layers.Link[] Links { get; set; }
		public DataSource[] DataSources { get; set; }
	}

	//public class DataWarehouse
	//{
	//	public string TargetPlatform { get; set; }
	//	public string TargetPlatformVersion { get; set; }
	//	public bool SingleDatabase { get; set; }
	//	public bool ColumnStore { get; set; }
	//	public bool UseNewHashLogic { get; set; }
	//	public Connection ProdStagingDatabase { get; set; }
	//	public Connection ProdDataVaultDatabase { get; set; }
	//	public Connection ProdMartDatabase { get; set; }
	//	public Connection DevStagingDatabase { get; set; }
	//	public Connection DevDataVaultDatabase { get; set; }
	//	public Connection DevMartDatabase { get; set; }
	//}

	//public class SourceSystem
	//{
	//	public string Name { get; set; }
	//	public string ShortName { get; set; }
	//}

	public class Connection
	{
		public Connection(string derivedType, string name)
		{
			DerivedType = derivedType;
			Name = name;
		}

		public string DerivedType { get; set; }
		public string Name { get; set; }
		public ConnectionType ConnectionType { get; set; }
		public string? ConnectionString { get; set; }
		public string? BaseUrl { get; set; }
		public string? EncryptedCredential { get; set; }
		public HttpAuthorization? Authorization { get; set; }
		public string? RestUser { get; set; }
		public string? Password { get; set; }
		public string? AuthorizationToken { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public List<Layers.Connections.KeyValue>? TokenBody { get; init; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public List<Layers.Connections.KeyValue>? TokenParameters { get; init; }
		public string? TokenAbsoluteUrl { get; set; }
		public string? TokenJsonIdentifier { get; set; }
		public string? ClientID { get; set; }
		public string? ClientSecret { get; set; }
		public GrantType? GrantType { get; set; }
	}

	//public class Hub
	//{
	//	public string Name { get; set; }
	//	public StagingColumn[] BusinessKeys { get; set; }
	//}

	//public class Link
	//{
	//	public string Name { get; set; }
	//	public StagingColumn[] BusinessKeys { get; set; }
	//}

	//public class StagingColumn : Column
	//{
	//	public bool? Driving { get; set; }
	//}

	public class DataSource
	{
		public DataSource
		(
			string derivedType, string name, string sourceSystem, string tenant,
			HubRelationship[] hubRelationships, LinkRelationship[] linkRelationships, Layers.Satellite[] satellites
		)
		{
			DerivedType = derivedType;
			Name = name;
			SourceSystem = sourceSystem;
			Tenant = tenant;
			HubRelationships = hubRelationships;
			LinkRelationships = linkRelationships;
			Satellites = satellites;
		}

		public string DerivedType { get; set; }
		public string Name { get; set; }
		public string SourceSystem { get; set; }
		public string Tenant { get; set; }
		public DataSourceType DataSourceType { get; set; }
		public LoadWidth DefaultLoadWidth { get; set; }
		public bool GenerateLatestViews { get; set; }
		public string? FileName { get; set; }
		public string? Format { get; set; }
		public string? RowDelimiter { get; set; }
		public string? ColumnDelimiter { get; set; }
		public bool? TextQualified { get; set; }
		public bool? HeadersInFirstRow { get; set; }
		public uint BusinessDateOffset { get; set; }
		public string? SqlSelectQuery { get; set; }
		public string? BusinessKey { get; set; }
		public Build Build { get; set; }
		public DestinationType DestinationType { get; set; }
		public string? ErrorHandling { get; set; }
		public string? FileDateRegex { get; set; }
		public HubRelationship[] HubRelationships { get; set; }
		public LinkRelationship[] LinkRelationships { get; set; }
		public Layers.Satellite[] Satellites { get; set; }
		public Layers.LoadTable? LoadTable { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public List<Layers.Connections.KeyValue>? Parameters { get; init; }
		public StagingTable? StagingTable { get; set; }
		public string? Connection { get; set; }
		public string? SqlStatement { get; set; }
		public string? SqlReadyCondition { get; set; }
		public uint ConnectionRetryAttempts { get; set; }
		public uint ConnectionRetryMinutes { get; set; }
		public uint? CodePage { get; set; }
		public string? IncrementalStagingColumn { get; set; }
		public string? IncrementalQuery { get; set; }
		public string? IncrementalExpression { get; set; }
		public bool? MergeToBlob { get; set; }
		public string? DestinationEncoding { get; set; }
		public string? TextQualifier { get; set; }
		public string? BusinessDateColumn { get; set; }
		public string? RelativeUrl { get; set; }
		public string? CollectionReference { get; set; }
		public string? PaginationNextLink { get; set; }
		public bool? PaginationLinkIsRelative { get; set; }
		public string? AzureLinkedServiceReference { get; set; }
		public string? TableName { get; set; }
		public string? Parent { get; set; }
		public int? NumberToFetch { get; set; }
		public string? GraphQlQuery { get; set; }
		public bool? SaveCookie { get; set; }
		public string? CustomScriptPath { get; set; }
	}

	//public class Loadtable
	//{
	//	public Column[] Columns { get; set; }
	//}

	//public class Column
	//{
	//	public string Name { get; set; }
	//	public string DataType { get; set; }
	//	public bool Nullable { get; set; }
	//	public int Length { get; set; }
	//	public int Scale { get; set; }
	//	public int Precision { get; set; }
	//}

	public class StagingTable
	{
		public StagingTable(StagingColumn[] columns)
		{
			Columns = columns;
		}

		public StagingColumn[] Columns { get; set; }
	}

	public class StagingColumn
	{
		public StagingColumn(string name)
		{
			Name = name;
		}

		public string? LoadColumn { get; set; }
		public string Name { get; set; }
		public SqlServerDataType DataType { get; set; }
		public bool Nullable { get; set; }
		public int? Length { get; set; }
		public string? Satellite { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public Dictionary<string, string>? ExtendedProperties { get; init; }
		public int? Scale { get; set; }
		public string? Logic { get; set; }
		public int? Precision { get; set; }
	}

	public class HubRelationship
	{
		public HubRelationship(string hub, HubMapping[] mappings)
		{
			Hub = hub;
			Mappings = mappings;
		}

		public string Hub { get; set; }
		public HubMapping[] Mappings { get; set; }
	}

	public class HubMapping
	{
		public HubMapping(string hubColumn)
		{
			HubColumn = hubColumn;
		}

		public string? StagingColumn { get; set; }
		public string HubColumn { get; set; }
	}

	public class LinkRelationship
	{
		public LinkRelationship(string link, LinkMapping[] mappings)
		{
			Link = link;
			Mappings = mappings;
		}

		public string Link { get; set; }
		public LinkMapping[] Mappings { get; set; }
	}

	public class LinkMapping
	{
		public LinkMapping(string linkColumn)
		{
			LinkColumn = linkColumn;
		}

		public string? StagingColumn { get; set; }
		public string LinkColumn { get; set; }
	}

	//public class Satellite
	//{
	//	public string Name { get; set; }
	//	public Layers.SatelliteType Type { get; set; }
	//}
}
