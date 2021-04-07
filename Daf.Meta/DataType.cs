// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Daf.Meta
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "It's an enum of data types.")]
	public enum SqlServerDataType
	{
		BigInt,
		Binary,
		Bit,
		Char,
		Date,
		DateTime,
		DateTime2,
		DateTimeOffset,
		Decimal, // Todo: Numeric data type needs to convert to decimal.
		Float,
		Geography,
		Geometry,
		HierarchyId,
		Image,
		Int,
		Money,
		NChar,
		NText,
		NVarChar, // Special handling for max length when converting to SSIS data type.
		Real,
		RowVersion,
		SmallDateTime,
		SmallInt,
		SmallMoney,
		Sql_Variant,
		Text,
		Time,
		Timestamp,
		TinyInt,
		UniqueIdentifier,
		VarBinary, // Special handling for max length when converting to SSIS data type.
		VarChar, // Special handling for max length when converting to SSIS data type.
		Xml
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "It's an enum of data types.")]
	public enum DafDataType
	{
		AnsiString,
		AnsiStringFixedLength,
		Binary,
		Boolean,
		Byte,
		Currency,
		Date,
		DateTime,
		DateTime2,
		DateTimeOffset,
		Decimal,
		Double,
		Geography,
		Geometry,
		Guid,
		HierarchyId,
		Int16,
		Int32,
		Int64,
		Object,
		SByte,
		Single,
		SmallMoney,
		String,
		StringFixedLength,
		Time,
		UInt16,
		UInt32,
		UInt64,
		VarNumeric,
		Xml
	}

	public static class Converter
	{
		#region Reserved Keywords
		private static readonly HashSet<string> ReservedKeyWords = new()
		{
			"ADD",
			"ALL",
			"ALTER",
			"AND",
			"ANY",
			"AS",
			"ASC",
			"AUTHORIZATION",
			"BACKUP",
			"BEGIN",
			"BETWEEN",
			"BREAK",
			"BROWSE",
			"BULK",
			"BY",
			"CASCADE",
			"CASE",
			"CHECK",
			"CHECKPOINT",
			"CLOSE",
			"CLUSTERED",
			"COALESCE",
			"COLLATE",
			"COLUMN",
			"COMMIT",
			"COMPUTE",
			"CONSTRAINT",
			"CONTAINS",
			"CONTAINSTABLE",
			"CONTINUE",
			"CONVERT",
			"CREATE",
			"CROSS",
			"CURRENT",
			"CURRENT_DATE",
			"CURRENT_TIME",
			"CURRENT_TIMESTAMP",
			"CURRENT_USER",
			"CURSOR",
			"DATABASE",
			"DBCC",
			"DEALLOCATE",
			"DECLARE",
			"DEFAULT",
			"DELETE",
			"DENY",
			"DESC",
			"DISK",
			"DISTINCT",
			"DISTRIBUTED",
			"DOUBLE",
			"DROP",
			"DUMP",
			"ELSE",
			"END",
			"ERRLVL",
			"ESCAPE",
			"EXCEPT",
			"EXEC",
			"EXECUTE",
			"EXISTS",
			"EXIT",
			"EXTERNAL",
			"FETCH",
			"FILE",
			"FILLFACTOR",
			"FOR",
			"FOREIGN",
			"FREETEXT",
			"FREETEXTTABLE",
			"FROM",
			"FULL",
			"FUNCTION",
			"GOTO",
			"GRANT",
			"GROUP",
			"HAVING",
			"HOLDLOCK",
			"IDENTITY",
			"IDENTITYCOL",
			"IDENTITY_INSERT",
			"IF",
			"IN",
			"INDEX",
			"INNER",
			"INSERT",
			"INTERSECT",
			"INTO",
			"IS",
			"JOIN",
			"KEY",
			"KILL",
			"LEFT",
			"LIKE",
			"LINENO",
			"LOAD",
			"MERGE",
			"NATIONAL",
			"NOCHECK",
			"NONCLUSTERED",
			"NOT",
			"NULL",
			"NULLIF",
			"OF",
			"OFF",
			"OFFSETS",
			"ON",
			"OPEN",
			"OPENDATASOURCE",
			"OPENQUERY",
			"OPENROWSET",
			"OPENXML",
			"OPTION",
			"OR",
			"ORDER",
			"OUTER",
			"OVER",
			"OWNER",
			"PERCENT",
			"PIVOT",
			"PLAN",
			"PRECISION",
			"PRIMARY",
			"PRINT",
			"PROC",
			"PROCEDURE",
			"PUBLIC",
			"RAISERROR",
			"READ",
			"READTEXT",
			"RECONFIGURE",
			"REFERENCES",
			"REPLICATION",
			"RESTORE",
			"RESTRICT",
			"RETURN",
			"REVERT",
			"REVOKE",
			"RIGHT",
			"ROLLBACK",
			"ROWCOUNT",
			"ROWGUIDCOL",
			"RULE",
			"SAVE",
			"SCHEMA",
			"SECURITYAUDIT",
			"SELECT",
			"SEMANTICKEYPHRASETABLE",
			"SEMANTICSIMILARITYDETAILSTABLE",
			"SEMANTICSIMILARITYTABLE",
			"SESSION_USER",
			"SET",
			"SETUSER",
			"SHUTDOWN",
			"SOME",
			"STATISTICS",
			"STATUS",
			"SYSTEM_USER",
			"TABLE",
			"TABLESAMPLE",
			"TEXTSIZE",
			"THEN",
			"TO",
			"TOP",
			"TRAN",
			"TRANSACTION",
			"TRIGGER",
			"TRUNCATE",
			"TRY_CONVERT",
			"TSEQUAL",
			"UNION",
			"UNIQUE",
			"UNPIVOT",
			"UPDATE",
			"UPDATETEXT",
			"USE",
			"USER",
			"VALUES",
			"VARYING",
			"VIEW",
			"WAITFOR",
			"WHEN",
			"WHERE",
			"WHILE",
			"WITH",
			"WITHIN GROUP",
			"WRITETEXT",
		};
		#endregion

		public static string GetDafDataTypeName(SqlServerDataType dataType)
		{
			switch (dataType)
			{
				case SqlServerDataType.VarChar:
					return DafDataType.AnsiString.ToString();
				case SqlServerDataType.NVarChar:
					return DafDataType.String.ToString();
				case SqlServerDataType.Int:
					return DafDataType.Int32.ToString();
				case SqlServerDataType.BigInt:
					return DafDataType.Int64.ToString();
				case SqlServerDataType.Date:
					return DafDataType.Date.ToString();
				case SqlServerDataType.Bit:
					return DafDataType.Boolean.ToString();
				case SqlServerDataType.DateTime:
					return DafDataType.DateTime.ToString();
				case SqlServerDataType.DateTime2:
					return DafDataType.DateTime2.ToString();
				case SqlServerDataType.Time:
					return DafDataType.Time.ToString();
				case SqlServerDataType.Decimal:
					return DafDataType.Decimal.ToString();
				case SqlServerDataType.TinyInt:
					return DafDataType.Byte.ToString();
				case SqlServerDataType.Float:
					return DafDataType.Double.ToString();
				case SqlServerDataType.UniqueIdentifier:
					return DafDataType.Guid.ToString();
				case SqlServerDataType.SmallInt:
					return DafDataType.Int16.ToString();
				case SqlServerDataType.Binary:
					return DafDataType.Binary.ToString();
				case SqlServerDataType.DateTimeOffset:
					return DafDataType.DateTimeOffset.ToString();
			}

			throw new NotImplementedException($"Data type {dataType} is not supported by Daf.Meta");
		}

		public static string CheckKeyword(string name)
		{
			if (name == null)
				throw new ArgumentNullException($"Can't check a keyword {nameof(name)} that is null.");

			if (ReservedKeyWords.Contains(name.ToUpper(CultureInfo.InvariantCulture)))
			{
				return $"[{name}]";
			}
			else
			{
				return name;
			}
		}

		public static string CheckSqlCompatibility(string name)
		{
			//Checks that the field name starts with a letter or is contained in brackets.
			string regexPattern = @"^[a-zA-Z]+[a-zA-Z_\d]*$|^\[[\s\S]+\]$";
			Regex regexCompare = new(regexPattern);

			if (regexCompare.IsMatch(name))
			{
				return name;
			}
			else
			{
				return $"[{name}]";
			}
		}
	}
}
