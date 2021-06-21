// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel.DataAnnotations;
using Daf.Meta.Layers;
using Daf.Meta.Layers.DataSources;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class SqlDataSourceViewModel : DataSourceViewModel
	{
		private readonly SqlDataSource _sqlDataSource;

		public SqlDataSourceViewModel(DataSource dataSource) : base(dataSource)
		{
			_sqlDataSource = (SqlDataSource)dataSource;
		}

		public override DataSource DataSource => _sqlDataSource;

		[Category("Database")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[ItemsSourceProperty("Connections")]
		[DisplayMemberPath("Name")]
		[SortIndex(100)]
		public Connection Connection
		{
			get => _sqlDataSource.Connection;
			set
			{
				SetProperty(_sqlDataSource.Connection, value, _sqlDataSource, (dataSource, connection) => _sqlDataSource.Connection = connection, true);
			}
		}

		[Category("Database")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryAttempts
		{
			get => _sqlDataSource.ConnectionRetryAttempts;
			set
			{
				SetProperty(_sqlDataSource.ConnectionRetryAttempts, value, _sqlDataSource, (dataSource, connectionRetryAttempts) => _sqlDataSource.ConnectionRetryAttempts = connectionRetryAttempts, true);
			}
		}

		[Category("Database")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryMinutes
		{
			get => _sqlDataSource.ConnectionRetryMinutes;
			set
			{
				SetProperty(_sqlDataSource.ConnectionRetryMinutes, value, _sqlDataSource, (dataSource, connectionRetryMinutes) => _sqlDataSource.ConnectionRetryMinutes = connectionRetryMinutes, true);
			}
		}

		[Category("Database")]
		[SortIndex(100)]
		public string? TableName
		{
			get => _sqlDataSource.TableName;
			set
			{
				SetProperty(_sqlDataSource.TableName, value, _sqlDataSource, (dataSource, tableName) => _sqlDataSource.TableName = tableName, true);
			}
		}

		[Category("Database")]
		[SortIndex(100)]
		public string? SqlStatement
		{
			get => _sqlDataSource.SqlStatement;
			set
			{
				SetProperty(_sqlDataSource.SqlStatement, value, _sqlDataSource, (dataSource, sqlStatement) => _sqlDataSource.SqlStatement = sqlStatement, true);
			}
		}

		[Category("Database")]
		[SortIndex(100)]
		[Description("Sql query to run in order to check if data is ready to be imported. Should return 1 for true, 0 or zero rows for false.")]
		[DataType(DataType.MultilineText)]
		public string? SqlReadyCondition
		{
			get => _sqlDataSource.SqlReadyCondition;
			set
			{
				SetProperty(_sqlDataSource.SqlReadyCondition, value, _sqlDataSource, (dataSource, sqlReadyCondition) => _sqlDataSource.SqlReadyCondition = sqlReadyCondition, true);
			}
		}
	}
}
