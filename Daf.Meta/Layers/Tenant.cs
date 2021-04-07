// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Meta.Layers
{
	public class Tenant : PropertyChangedBaseClass, IComparable<Tenant>
	{
		public Tenant(string name, string shortName)
		{
			_name = name;
			_shortName = shortName;
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

		private string _shortName;

		public string ShortName
		{
			get { return _shortName; }
			set
			{
				if (_shortName != value)
				{
					_shortName = value;

					NotifyPropertyChanged("ShortName");
				}
			}
		}

		public int CompareTo(Tenant? other)
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
