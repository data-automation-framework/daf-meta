// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using CommunityToolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddConnectionViewModel : ObservableObject
	{
		private string _name = string.Empty;
		public string Name
		{
			get { return _name; }
			set
			{
				SetProperty(ref _name, value);
			}
		}

		private ConnectionType _selectedConnectionType;
		public ConnectionType SelectedConnectionType
		{
			get { return _selectedConnectionType; }
			set
			{
				SetProperty(ref _selectedConnectionType, value);
			}
		}
	}
}
