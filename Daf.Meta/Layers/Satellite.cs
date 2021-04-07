// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;

namespace Daf.Meta.Layers
{
	public class Satellite : PropertyChangedBaseClass
	{
		public Satellite(string name)
		{
			_name = name;
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;

					NotifyPropertyChanged("Name");
				}
			}
		}

		private SatelliteType _type;
		public SatelliteType Type
		{
			get { return _type; }
			set
			{
				if (_type != value)
				{
					_type = value;

					NotifyPropertyChanged("Type");
				}
			}
		}

		public List<StagingColumn> GetColumns(DataSource dataSource, bool includeHubs = false)
		{
			if (dataSource == null)
				throw new ArgumentNullException($"Can't get columns for a {nameof(DataSource)} that is null.");

			if (dataSource.BusinessKey == null)
				throw new InvalidOperationException(nameof(dataSource.BusinessKey));

			if (dataSource.StagingTable == null)
				throw new InvalidOperationException(nameof(dataSource.StagingTable));

			List<StagingColumn> columns = new();

			// Add staging columns that aren't part of the business key
			foreach (StagingColumn column in dataSource.StagingTable.Columns)
			{
				if (column.Satellite == this)
				{
					columns.Add(column);
				}
				else if (includeHubs)
				{
					foreach (HubRelationship h in dataSource.HubRelationships)
					{
						foreach (HubMapping hubMapping in h.Mappings)
						{
							bool includedInBK = false;

							foreach (LinkRelationship l in dataSource.LinkRelationships)
							{
								// Don't include the hub if it's included in this data source's BK.
								if (h.Hub.Name != dataSource.BusinessKey.Name && l.Link.Name == dataSource.BusinessKey.Name)
								{
									foreach (LinkMapping linkMapping in l.Mappings)
									{
										if (linkMapping.StagingColumn == column)
										{
											includedInBK = true;

											break;
										}
									}
								}

								if (includedInBK)
									break;
							}

							if (includedInBK)
								break;

							// Don't include the hub if it's included in this data source's BK.
							if ((h.Hub.Name != dataSource.BusinessKey.Name && column == hubMapping.StagingColumn) || (column == hubMapping.StagingColumn && "H_" + column.Name != h.Hub.Name))
							{
								columns.Add(column);

								break;
							}
						}
					}
				}
			}

			return columns;
		}

		public List<string> GetRelevantKeys(string prefix = "", bool selfAlias = false)
		{
			// Since a satellite's table always refer to the same links, we can look at these links and see if they have
			// driving keys that are complete or not. Thus the data source does not matter at all, just pick the first one.
			DataSource dataSource = Model.Instance.DataSources.FirstOrDefault(x => x.GetSatelliteNames()?.Contains(Name) ?? false)!;

			List<string> relevantKeys = new();

			if (dataSource.BusinessKey is Hub hub)
			{
				relevantKeys.Add($"{(string.IsNullOrEmpty(prefix) ? "" : $"{prefix}.")}{hub.Name}NaturalKey{(selfAlias ? $" AS {hub.Name}NaturalKey" : "")}");
			}
			else if (dataSource.BusinessKey is Link link)
			{
				if (link.AllColumnsAreDriving)
				{
					relevantKeys.Add($"{(string.IsNullOrEmpty(prefix) ? "" : $"{prefix}.")}{link.Name}HashKey{(selfAlias ? $" AS {link.Name}HashKey" : "")}");
				}
				else
				{
					relevantKeys.Add($"{(string.IsNullOrEmpty(prefix) ? "" : $"{prefix}.")}{link.Name}HashKey{(selfAlias ? $" AS {link.Name}HashKey" : "")}");
					relevantKeys.Add($"{(string.IsNullOrEmpty(prefix) ? "" : $"{prefix}.")}{link.Name}DrivingKey{(selfAlias ? $" AS {link.Name}DrivingKey" : "")}");
				}
			}

			return relevantKeys;
		}

		public List<string> GetFormattedInsertColumns(DataSource dataSource)
		{
			List<string> columnNames = new();

			columnNames.AddRange(GetRelevantKeys());

			columnNames.Add("LoadDate");
			if (Type == SatelliteType.HashDiff)
			{
				columnNames.Add(Name + "HashDiff");
			}

			foreach (StagingColumn column in GetColumns(dataSource, includeHubs: true))
			{
				if (column.GetHub(dataSource) == null)
					columnNames.Add(Converter.CheckKeyword(column.Name.Replace("HashKey", "DrivingKey")));
				else
					columnNames.Add(Converter.CheckKeyword(column.GetHubColumnName(dataSource).Replace("HashKey", "DrivingKey")));
			}

			return columnNames;
		}

		public List<string> GetFormattedSelectColumns(DataSource dataSource, string prefix)
		{
			List<string> columnNames = new();

			columnNames.AddRange(GetRelevantKeys().Select(x => $"{prefix}.{x} AS {x}"));

			columnNames.Add($"{prefix}.LoadDate AS LoadDate");
			if (Type == SatelliteType.HashDiff)
			{
				columnNames.Add($"{prefix}.{Name}HashDiff AS {Name}HashDiff");
			}

			foreach (StagingColumn column in GetColumns(dataSource, includeHubs: true))
			{
				if (column.GetHub(dataSource) == null)
					columnNames.Add($"{prefix}.{Converter.CheckKeyword(column.Name)} AS {Converter.CheckKeyword(column.Name)}");
				else
					columnNames.Add($"{prefix}.{Converter.CheckKeyword(column.GetHubColumnName(dataSource))} AS {Converter.CheckKeyword(column.GetHubColumnName(dataSource))}");
			}

			return columnNames;
		}
	}

	public enum SatelliteType
	{
		Transaction,
		HashDiff,
		Enabled
	}
}
