// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel;

namespace Daf.Meta
{
	public class PropertyChangedBaseClass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		public void ClearSubscribers()
		{
			PropertyChanged = null;
		}
	}
}
