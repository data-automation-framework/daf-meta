// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Daf.Meta.JsonConverters;

namespace Daf.Meta.Layers
{
	public class LinkRelationship : PropertyChangedBaseClass
	{
		public LinkRelationship(Link link)
		{
			Link = link;

			Link.ChangedBusinessKeyColumn += BusinessKeys_CollectionChanged;
		}

		private void BusinessKeys_CollectionChanged(object? sender, BusinessKeyEventArgs e)
		{
			if (e.BusinessKey == null)
				throw new InvalidOperationException("BusinessKey was null!");

			if (e.Action == BusinessKeyEventType.Add)
			{
				StagingColumn stagingColumn = e.BusinessKey;

				Mappings.Add(new LinkMapping(stagingColumn));
			}
			else if (e.Action == BusinessKeyEventType.Remove)
			{
				StagingColumn stagingColumn = e.BusinessKey;

				foreach (LinkMapping mapping in Mappings)
				{
					if (mapping.LinkColumn == stagingColumn)
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
			Link.ChangedBusinessKeyColumn -= BusinessKeys_CollectionChanged;
		}

		[JsonConverter(typeof(LinkConverter))]
		public Link Link { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public ObservableCollection<LinkMapping> Mappings { get; init; } = new ObservableCollection<LinkMapping>();

		[JsonIgnore]
		public List<StagingColumn> AvailableColumns
		{
			get
			{
				List<StagingColumn> availableColumns = new();

				foreach (StagingColumn businessKey in Link.BusinessKeys)
				{
					availableColumns.Add(businessKey);
				}

				return availableColumns;
			}
		}
	}

	public class LinkMapping : PropertyChangedBaseClass
	{
		internal event EventHandler? ChangedStagingColumn;

		public LinkMapping(StagingColumn linkColumn)
		{
			_linkColumn = linkColumn;
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

		private StagingColumn _linkColumn;

		[JsonConverter(typeof(StagingColumnConverter))]
		public StagingColumn LinkColumn
		{
			get { return _linkColumn; }
			set
			{
				if (_linkColumn != value)
				{
					_linkColumn = value;

					NotifyPropertyChanged("LinkColumn");
				}
			}
		}

		protected void OnChangedStagingColumn()
		{
			ChangedStagingColumn?.Invoke(this, new EventArgs());
		}
	}
}
