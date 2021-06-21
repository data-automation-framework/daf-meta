// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Daf.Meta.JsonConverters;

namespace Daf.Meta.Layers
{
	public class HubRelationship : PropertyChangedBaseClass
	{
		public HubRelationship(Hub hub)
		{
			Hub = hub;

			Hub.ChangedBusinessKeyColumn += BusinessKeys_CollectionChanged;
		}

		private void BusinessKeys_CollectionChanged(object? sender, BusinessKeyEventArgs e)
		{
			if (e.BusinessKey == null)
				throw new InvalidOperationException("BusinessKey was null!");

			if (e.Action == BusinessKeyEventType.Add)
			{
				StagingColumn stagingColumn = e.BusinessKey;

				Mappings.Add(new HubMapping(stagingColumn));
			}
			else if (e.Action == BusinessKeyEventType.Remove)
			{
				StagingColumn stagingColumn = e.BusinessKey;

				foreach (HubMapping mapping in Mappings)
				{
					if (mapping.HubColumn == stagingColumn)
					{
						Mappings.Remove(mapping);
						break;
					}
				}
			}

			// Need to verify Mappings contain same number of HubMapping as there are BusinessKeys, and that each of the BusinessKeys correspond exactly to one HubMapping.
			// Will write unit tests for this.
		}

		public void Unsubscribe()
		{
			Hub.ChangedBusinessKeyColumn -= BusinessKeys_CollectionChanged;
		}

		[JsonConverter(typeof(HubConverter))]
		public Hub Hub { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public ObservableCollection<HubMapping> Mappings { get; init; } = new();

		[JsonIgnore]
		public List<StagingColumn> AvailableColumns
		{
			get
			{
				List<StagingColumn> availableColumns = new();

				foreach (StagingColumn businessKey in Hub.BusinessKeys)
				{
					availableColumns.Add(businessKey);
				}

				return availableColumns;
			}
		}
	}

	public class HubMapping : PropertyChangedBaseClass
	{
		// Event used for when the StagingColumn associated with a HubMapping is changed.
		// Currently only used to update the list of ColumnsNotInLinksOrHubs in DataSource.cs.
		// Subscribed to in Model.cs when a new HubMapping is created.
		internal event EventHandler? ChangedStagingColumn;

		public HubMapping(StagingColumn hubColumn)
		{
			_hubColumn = hubColumn;
		}

		private StagingColumn? _stagingColumn;

		[JsonConverter(typeof(StagingColumnConverter))]
		public StagingColumn? StagingColumn
		{
			get { return _stagingColumn; }
			set
			{
				if (_stagingColumn != value)
				{
					_stagingColumn = value;

					if (_stagingColumn != null)
						_stagingColumn.Satellite = null;

					NotifyPropertyChanged("StagingColumn");
					OnChangedStagingColumn();
				}
			}
		}

		private StagingColumn _hubColumn;

		[JsonConverter(typeof(StagingColumnConverter))]
		public StagingColumn HubColumn
		{
			get { return _hubColumn; }
			set
			{
				if (_hubColumn != value)
				{
					_hubColumn = value;

					NotifyPropertyChanged("HubColumn");
				}
			}
		}

		protected void OnChangedStagingColumn()
		{
			ChangedStagingColumn?.Invoke(this, new EventArgs());
		}
	}
}
