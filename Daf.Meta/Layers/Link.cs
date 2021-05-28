// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Daf.Meta.Layers
{
	public class Link : BusinessKey
	{
		public Link(string name)
		{
			Name = name;
		}

		public int GetNumDrivingColumns()
		{
			return BusinessKeys.Count(x => x.Driving == true);
		}

		public List<string> GetRelevantKeys()
		{
			List<string> relevantKeys = new();

			if (AllColumnsAreDriving)
			{
				relevantKeys.Add(Name + "HashKey");
			}
			else
			{
				relevantKeys.Add(Name + "HashKey");
				relevantKeys.Add(Name + "DrivingKey");
			}

			return relevantKeys;
		}

		public List<StagingColumn> GetSortedIndexColumns()
		{
			List<StagingColumn> indexColumns = new();
			List<StagingColumn> nonHubColumns = new();

			foreach (StagingColumn linkColumn in BusinessKeys)
			{
				if (linkColumn.Name.StartsWith("H_", StringComparison.Ordinal))
				{
					indexColumns.Add(linkColumn);
				}
				else
				{
					nonHubColumns.Add(linkColumn);
				}
			}

			indexColumns.AddRange(nonHubColumns);

			return indexColumns;
		}

		public List<Hub> GetAssociatedHubs()
		{
			List<Hub> associatedHubs = new();

			foreach (StagingColumn linkColumn in BusinessKeys)
			{
				if (linkColumn.Name.StartsWith("H_", StringComparison.Ordinal))
				{
					foreach (Hub hub in Model.Instance.Hubs)
					{
						if (hub.BusinessKeys[0].Name == linkColumn.Name)
						{
							associatedHubs.Add(hub);
							break;
						}
					}
				}
			}

			return associatedHubs;
		}

		[JsonIgnore]
		public bool AllColumnsAreDriving
		{
			get
			{
				// If any of the contained columns aren't driving, return false.
				foreach (StagingColumn column in BusinessKeys)
				{
					if (column.Driving == false)
					{
						return false;
					}
				}

				return true;
			}
		}
	}
}
