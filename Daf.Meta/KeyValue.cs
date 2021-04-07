// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Meta.Layers.Connections
{
	public class KeyValue : PropertyChangedBaseClass
	{
		private string? _key;

		public string? Key
		{
			get { return _key; }
			set
			{
				if (_key != value)
				{
					_key = value;

					NotifyPropertyChanged("Key");
				}
			}
		}

		private string? _value;

		public string? Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;

					NotifyPropertyChanged("Value");
				}
			}
		}
	}
}
