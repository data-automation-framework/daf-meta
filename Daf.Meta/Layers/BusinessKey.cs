// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;

namespace Daf.Meta.Layers
{
	public abstract class BusinessKey : PropertyChangedBaseClass
	{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private string _name; // This is initialized in the constructor of each derived class. Dahomey.Json doesn't support constructors in abstract classes.
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public ObservableCollection<StagingColumn> BusinessKeys { get; init; } = new ObservableCollection<StagingColumn>();

		/// <summary>
		/// Adds a new StagingColumn to <see cref="BusinessKeys">BusinessKeys</see>.
		/// </summary>
		/// <returns>The staging column object which was added to the collection.</returns>
		public StagingColumn AddBusinessKeyColumn()
		{
			StagingColumn stagingColumn = new(name: "New Column");

			stagingColumn.PropertyChanged += (s, e) =>
			{
				NotifyPropertyChanged("StagingColumn"); // I don't know what this does. There's no property named StagingColumn??
			};

			BusinessKeys.Add(stagingColumn);

			return stagingColumn;
		}

		public void RemoveBusinessKeyColumn(StagingColumn columnToRemove)
		{
			if (columnToRemove == null)
				throw new ArgumentNullException($"Can't remove a {nameof(StagingColumn)} that is null.");

			columnToRemove.ClearSubscribers();

			BusinessKeys.Remove(columnToRemove);
		}
	}
}
