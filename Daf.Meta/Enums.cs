// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Meta
{
	public enum ConnectionType
	{
		MySql,
		OleDB,
		Rest,
		GraphQl,
		Odbc
	}

	public enum DataSourceType
	{
		FlatFile,
		Rest,
		Script,
		Sql,
		GraphQl
	}

	public enum DestinationType
	{
		Sql,
		Blob
	}

	public enum Build
	{
		All,
		None,
		NoETL
	}

	public enum LoadWidth
	{
		Full,
		Partial
	}

	public enum HttpAuthorization
	{
		None,
		Basic,
		OAuth2,
		Token
	}

	public enum GrantType
	{
		Password
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Doesn't apply here.")]
	public enum SqlServerVersion
	{
		SqlServer2016 = 13,
		SqlServer2017 = 14,
		SqlServer2019 = 15
	}
}
