// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;


namespace Daf.Meta.Editor.ViewModels
{
	public abstract class ConnectionViewModel : ObservableValidator
	{
		protected ConnectionViewModel(Connection connection)
		{
			Connection = connection;
		}

		[Browsable(false)]
		public virtual Connection Connection { get; }

		[Category("General")]
		public string Name
		{
			get { return Connection!.Name; }
			set
			{
				SetProperty(Connection!.Name, value, Connection, (connection, name) => Connection.Name = name, true);
			}
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
