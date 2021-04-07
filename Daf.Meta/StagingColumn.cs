// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using Daf.Meta.JsonConverters;
using Daf.Meta.Layers;

namespace Daf.Meta
{
	public class StagingColumn : Column
	{
		public StagingColumn(string name) : base(name)
		{
		}

		private string? _logic;

		public string? Logic
		{
			get { return _logic; }
			set
			{
				if (_logic != value)
				{
					_logic = value;

					if (string.IsNullOrEmpty(_logic))
						_logic = null;

					NotifyPropertyChanged("Logic");
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "For now, we want to be able to set this in order to prevent every single column from having an ExtendedProperties array, includings hub and link columns.")]

		public Dictionary<string, string>? ExtendedProperties { get; set; }

		private Column? _loadColumn;

		[JsonConverter(typeof(ColumnConverter))]
		public Column? LoadColumn
		{
			get { return _loadColumn; }
			set
			{
				if (_loadColumn != value)
				{
					_loadColumn = value;

					NotifyPropertyChanged("LoadColumn");
				}
			}
		}

		public StagingColumn Clone(LoadTable cloneLoadTable, ObservableCollection<Satellite> cloneSatellites)
		{
			if (cloneLoadTable == null)
				throw new ArgumentNullException(nameof(cloneLoadTable));

			StagingColumn cloneColumn = new(Name)
			{
				AddedOnBusinessDate = AddedOnBusinessDate,
				DataType = DataType,
				Length = Length,
				Precision = Precision,
				Scale = Scale,
				Nullable = Nullable,
				Driving = Driving,
				Logic = Logic
			};

			if (Satellite != null)
			{
				Satellite cloneSatellite = cloneSatellites.Single(cloneSatellite => cloneSatellite.Name == Satellite.Name);

				cloneSatellite.Type = Satellite.Type;

				cloneColumn.Satellite = cloneSatellite;
			}

			if (LoadColumn != null)
			{
				cloneColumn.LoadColumn = cloneLoadTable.Columns.Single(loadCol => loadCol.Name == LoadColumn.Name);
			}

			if (ExtendedProperties != null && ExtendedProperties.Count > 0)
			{
				cloneColumn.ExtendedProperties = new Dictionary<string, string>();

				foreach (KeyValuePair<string, string> extendedProperty in ExtendedProperties)
				{
					cloneColumn.ExtendedProperties.Add(extendedProperty.Key, extendedProperty.Value);
				}
			}

			return cloneColumn;
		}

		private Satellite? _satellite;

		[JsonConverter(typeof(SatelliteConverter))]
		public Satellite? Satellite
		{
			get { return _satellite; }
			set
			{
				if (_satellite != value)
				{
					_satellite = value;

					NotifyPropertyChanged("Satellite");
				}
			}
		}

		private bool? _driving;

		// Note: This should only be set to non-null for StagingColumns in links. It's irrelevant in every other context.
		public bool? Driving
		{
			get { return _driving; }
			set
			{
				if (_driving != value)
				{
					_driving = value;

					NotifyPropertyChanged("Driving");
				}
			}
		}

		// Check if column is associated with hub and return the hub column name, if not crash.
		public string GetHubColumnName(DataSource dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource));

			foreach (HubRelationship relationship in dataSource.HubRelationships)
			{
				foreach (HubMapping mapping in relationship.Mappings)
				{
					if (mapping.HubColumn == this)
					{
						return Name;
					}
					else if (mapping.StagingColumn == this)
					{
						if (mapping.HubColumn.Name.Replace("H_", string.Empty).Replace("NaturalKey", string.Empty) == Name)
						{
							return "H_" + Name + "NaturalKey";
						}
						else
						{
							return mapping.HubColumn.Name + "_" + Name;
						}
					}
				}
			}

			throw new InvalidOperationException("Attempted to call GetHubColumnName on a column not associated with a hub.");
		}

		public string? GetLoadLogic()
		{
			if (LoadColumn != null)
			{
				if (Logic != null)
				{
					return Logic.ToUpper(CultureInfo.InvariantCulture).Replace("LOADCOLUMN", Converter.CheckSqlCompatibility(Converter.CheckKeyword(LoadColumn.Name)));
				}
				else
				{
					return Converter.CheckSqlCompatibility(Converter.CheckKeyword(LoadColumn.Name));
				}
			}

			return null;
		}

		public Hub? GetHub(DataSource dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource));

			foreach (HubRelationship hubRelationship in dataSource.HubRelationships)
			{
				foreach (HubMapping mapping in hubRelationship.Mappings)
				{
					if (mapping.StagingColumn == this || mapping.HubColumn == this)
					{
						return Model.Instance.GetHub(hubRelationship.Hub.Name);
					}
				}
			}

			return null;
		}

		public Link? GetLink(DataSource dataSource)
		{
			if (dataSource == null)
				throw new ArgumentNullException(nameof(dataSource));

			foreach (LinkRelationship linkRelationship in dataSource.LinkRelationships)
			{
				foreach (LinkMapping mapping in linkRelationship.Mappings)
				{
					if (mapping.StagingColumn == this)
					{
						return Model.Instance.GetLink(linkRelationship.Link.Name);
					}
				}
			}

			return null;
		}
	}
}
