// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Xunit;

namespace Daf.Meta.Editor.Tests
{
	public class LoadMetadataTests
	{
		[Fact(Skip = "Need to change how MetadataPath is set in MainViewModel.")]
		public void LoadMetaData_LoadMetaDataFromDisk_DataGetsLoaded()
		{
			// I would prefer to set MetadataPath in the constructor but this may have to be discussed before implementation.
		}
	}
}
