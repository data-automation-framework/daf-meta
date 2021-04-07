// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Layers
{
	public abstract class Connection : PropertyChangedBaseClass, IComparable<Connection>
	{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		private string _name; // This is initialized in the constructor of each derived class. Dahomey.Json doesn't support constructors in abstract classes.
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

		[Category("General")]
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

		private ConnectionType _connectionType;

		[Browsable(false)]
		public ConnectionType ConnectionType
		{
			get { return _connectionType; }
			set
			{
				if (_connectionType != value)
				{
					_connectionType = value;

					NotifyPropertyChanged("Type");
				}
			}
		}

		// Returns the database name and a dot, to be used in a 3-part qualifier.
		public string GetQualifier()
		{
			if (Model.Instance.DataWarehouse.SingleDatabase)
				return "";
			else
				return Name + ".";
		}

		public int CompareTo(Connection? other)
		{
			if (other == null)
				return -1;

			if (Name == other.Name)
				return 0;

			if (string.Compare(Name, other.Name, StringComparison.InvariantCulture) < 0)
				return -1;

			return 1;
		}
	}
}
