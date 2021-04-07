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
}
