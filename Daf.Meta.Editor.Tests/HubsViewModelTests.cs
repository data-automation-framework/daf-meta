// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using Daf.Meta.Editor.ViewModels;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Xunit;

namespace Daf.Meta.Editor.Tests
{
	/// <summary>
	/// Tests that Hubs stay synchronised across Model and ViewModel when Hubs are added, removed or re-named,
	/// going from Model to ViewModel and vice versa. Also for testing the same behavior for BusinessKeys.
	/// </summary>
	public class HubsViewModelTests : IClassFixture<TestFixture>
	{
		[Fact]
		public void Setup_ConfirmEmpty_CollectionsEmpty()
		{
			MainViewModel MVM = new();

			// Confirm that both Model.Hubs and HubsVM.Hubs are empty.
			Assert.Empty(MVM.Model.Hubs);
			Assert.Empty(MVM.HubsVM.Hubs);
			Assert.Equal(MVM.Model.Hubs.Count, MVM.HubsVM.Hubs.Count);
		}

		[Fact(Skip = "MainViewModel will always read Metadata from disk or initialize empty model in its constructor. Adding Hubs to Model before initialization is thus not possible.")]
		public void AddHubToModel_BeforeLoadingViewModel_HubGetsAddedToViewModel()
		{
			// Can't make this test work without changing how MainViewModel loads metadata.
			// It always requires a filepath, and reading data from that filepath. Thus no changes
			// can be made to the model that will be reflected in the View Model.
			Model.Initialize();
			Model.Instance.Hubs.Add(new Hub("New Hub"));
			MainViewModel MVM = new();

			Assert.Equal(MVM.Model.Hubs.Count, MVM.HubsVM.Hubs.Count);
			Assert.Same(MVM.Model.Hubs[0], MVM.HubsVM.Hubs[0].Hub);
		}

		[Fact]
		public void AddHub_AddSingleHubToViewModel_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			MVM.HubsVM.AddHub("New Hub");

			// Confirm that Model and ViewModel reference the same Hub object.
			Assert.Same(MVM.Model.Hubs[0], MVM.HubsVM.Hubs[0].Hub);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(2)]
		public void AddHub_AddSeveralHubsToViewModel_SynchronisesWithModel(int index)
		{
			MainViewModel MVM = new();

			MVM.HubsVM.AddHub($"New Hub");
			MVM.HubsVM.AddHub($"New Hub");
			MVM.HubsVM.AddHub($"New Hub");

			Hub expected = MVM.HubsVM.Hubs[index].Hub;
			Hub actual = MVM.Model.Hubs[index];

			// Check that the HubViewModels in HubsVM wrap the same object as exists in Model.Hubs.
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		public void DeleteHub_AddHubViewModelsThenDelete_SynchronisesWithModel(int index)
		{
			MainViewModel MVM = new();

			// Test will fail if Hubs are identical by value even if they're not the same object.
			MVM.HubsVM.AddHub($"New Hub1");
			MVM.HubsVM.AddHub($"New Hub2");
			MVM.HubsVM.AddHub($"New Hub3");

			Hub hubToBeDeleted = MVM.HubsVM.Hubs[index].Hub;

			// Delete a HubViewModel.
			MVM.HubsVM.DeleteHub(MVM.HubsVM.Hubs[index]);

			// This assert only checks if two items are identical, not if they are the same object.
			Assert.DoesNotContain(hubToBeDeleted, MVM.Model.Hubs);

		}

		[Fact]
		public void AddHubColumn_CreateHubViewModelThenAddBusinessKeyViewModel_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			MVM.HubsVM.AddHub($"New Hub");
			// Must set the SelectedHub as AddHubColumn() takes no arguments.
			MVM.HubsVM.SelectedHub = MVM.HubsVM.Hubs[0];
			// Add a new HubColumn, passing the HubViewModel that the Column should be added to as an argument.
			MVM.HubsVM.AddHubColumn();

			StagingColumn expected = MVM.HubsVM.Hubs[0].BusinessKeys[0].StagingColumn;
			StagingColumn actual = MVM.Model.Hubs[0].BusinessKeys[0];

			// Ensure that the StagingColumn in the model is the same object as the StagingColumn wrapped by
			// the BusinessKeyViewModel in the corresponding Hub.
			Assert.Same(expected, actual);
		}

		[Fact]
		public void DeleteHubColumn_CreateHubViewModelWithColumnsThenDeleteColumns_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			// AddHub sets SelectedHub to the new HubViewModel that's been created.
			MVM.HubsVM.AddHub($"New Hub");

			// Add a new HubColumn, passing the HubViewModel that the Column should be added to as an argument.
			MVM.HubsVM.AddHubColumn();

			// Must set SelectedHubColumn to be able to remove HubColumn.
			MVM.HubsVM.SelectedHubColumn = MVM.HubsVM.Hubs[0].BusinessKeys[0];
			StagingColumn hubColumnToDelete = MVM.HubsVM.Hubs[0].BusinessKeys[0].StagingColumn;

			// Deletes the SelectedHubColumn.
			MVM.HubsVM.DeleteHubColumn();

			Assert.DoesNotContain(hubColumnToDelete, MVM.Model.Hubs[0].BusinessKeys);
		}

		[Fact]
		public void AddLink_AddSingleLinkToViewModel_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			MVM.LinksVM.AddLink("New Link");

			// Confirm that Model and ViewModel reference the same Link object.
			Assert.Same(MVM.Model.Links[0], MVM.LinksVM.Links[0].Link);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(2)]
		public void AddLink_AddSeveralLinksToViewModel_SynchronisesWithModel(int index)
		{
			MainViewModel MVM = new();

			MVM.LinksVM.AddLink($"New Link");
			MVM.LinksVM.AddLink($"New Link");
			MVM.LinksVM.AddLink($"New Link");

			Link expected = MVM.LinksVM.Links[index].Link;
			Link actual = MVM.Model.Links[index];

			// Check that the LinkViewModels in LinksVM wrap the same object as exists in Model.Links.
			Assert.Same(expected, actual);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		public void DeleteLink_AddLinkViewModelsThenDelete_SynchronisesWithModel(int index)
		{
			MainViewModel MVM = new();

			// Test will fail if Links are identical by value even if they're not the same object.
			MVM.LinksVM.AddLink($"New Link1");
			MVM.LinksVM.AddLink($"New Link2");
			MVM.LinksVM.AddLink($"New Link3");

			Link linkToBeDeleted = MVM.LinksVM.Links[index].Link;

			// Delete a LinkViewModel.
			MVM.LinksVM.DeleteLink(MVM.LinksVM.Links[index]);

			// This assert only checks if two items are identical, not if they are the same object.
			Assert.DoesNotContain(linkToBeDeleted, MVM.Model.Links);

		}

		[Fact]
		public void AddLinkColumn_CreateLinkViewModelThenAddBusinessKeyViewModel_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			MVM.LinksVM.AddLink($"New Link");
			// Must set the SelectedLink as AddLinkColumn() takes no arguments.
			MVM.LinksVM.SelectedLink = MVM.LinksVM.Links[0];
			// Add a new LinkColumn, passing the LinkViewModel that the Column should be added to as an argument.
			MVM.LinksVM.AddLinkColumn();

			StagingColumn expected = MVM.LinksVM.Links[0].BusinessKeys[0].StagingColumn;
			StagingColumn actual = MVM.Model.Links[0].BusinessKeys[0];

			// Ensure that the StagingColumn in the model is the same object as the StagingColumn wrapped by
			// the BusinessKeyViewModel in the corresponding Link.
			Assert.Same(expected, actual);
		}

		[Fact]
		public void DeleteLinkColumn_CreateLinkViewModelWithColumnsThenDeleteColumns_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			// AddLink sets SelectedLink to the new LinkViewModel that's been created.
			MVM.LinksVM.AddLink($"New Link");

			// Add a new LinkColumn, passing the LinkViewModel that the Column should be added to as an argument.
			MVM.LinksVM.AddLinkColumn();

			// Must set SelectedLinkColumn to be able to remove LinkColumn.
			MVM.LinksVM.SelectedLinkColumn = MVM.LinksVM.Links[0].BusinessKeys[0];
			StagingColumn linkColumnToDelete = MVM.LinksVM.Links[0].BusinessKeys[0].StagingColumn;

			// Deletes the SelectedLinkColumn.
			MVM.LinksVM.DeleteLinkColumn();

			Assert.DoesNotContain(linkColumnToDelete, MVM.Model.Links[0].BusinessKeys);
		}

		[Fact]
		public void AddConnection_AddConnectionViewModel_SynchronisesWithModel()
		{
			MainViewModel MVM = new();

			ObservableCollection<ConnectionViewModel> viewModels = MainViewModel.GetConnections(MVM.Model.Connections);

			ConnectionsViewModel connectionsViewModel = new(viewModels);

			connectionsViewModel.AddConnection(new RestConnection("New Connection"));

			// Confirm that Model and ViewModel reference the same Connection object.
			Assert.Same(MVM.Model.Connections[0], connectionsViewModel.Connections[0].Connection);
		}
	}
}
