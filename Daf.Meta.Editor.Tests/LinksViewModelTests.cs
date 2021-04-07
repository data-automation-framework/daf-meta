//// SPDX-License-Identifier: MIT
//// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

//using Daf.Meta.Editor.ViewModels;
//using Daf.Meta.Layers;
//using Xunit;

//namespace Daf.Meta.Editor.Tests
//{
//	/// <summary>
//	/// Tests that Links stay synchronised across Model and ViewModel when Links are added, removed or re-named,
//	/// going from Model to ViewModel and vice versa. Also for testing the same behavior for BusinessKeys.
//	/// </summary>
//	public class LinksViewModelTests : IClassFixture<TestFixture>
//	{
//		[Fact]
//		public void Setup_ConfirmEmpty_CollectionsEmpty()
//		{
//			MainViewModel MVM = new();

//			// Confirm that both Model.Links and LinksVM.Links are empty.
//			Assert.Empty(MVM.Model.Links);
//			Assert.Empty(MVM.LinksVM.Links);
//			Assert.Equal(MVM.Model.Links.Count, MVM.LinksVM.Links.Count);
//		}

//		[Fact]
//		public void AddLink_AddSingleLinkToViewModel_SynchronisesWithModel()
//		{
//			MainViewModel MVM = new();

//			MVM.LinksVM.AddLink("New Link");

//			// Confirm that Model and ViewModel reference the same Link object.
//			Assert.Same(MVM.Model.Links[0], MVM.LinksVM.Links[0].Link);
//		}

//		[Theory]
//		[InlineData(0)]
//		[InlineData(1)]
//		[InlineData(2)]
//		public void AddLink_AddSeveralLinksToViewModel_SynchronisesWithModel(int index)
//		{
//			MainViewModel MVM = new();

//			MVM.LinksVM.AddLink($"New Link");
//			MVM.LinksVM.AddLink($"New Link");
//			MVM.LinksVM.AddLink($"New Link");

//			Link expected = MVM.LinksVM.Links[index].Link;
//			Link actual = MVM.Model.Links[index];

//			// Check that the LinkViewModels in LinksVM wrap the same object as exists in Model.Links.
//			Assert.Same(expected, actual);
//		}

//		[Theory]
//		[InlineData(0)]
//		[InlineData(1)]
//		public void DeleteLink_AddLinkViewModelsThenDelete_SynchronisesWithModel(int index)
//		{
//			MainViewModel MVM = new();

//			// Test will fail if Links are identical by value even if they're not the same object.
//			MVM.LinksVM.AddLink($"New Link1");
//			MVM.LinksVM.AddLink($"New Link2");
//			MVM.LinksVM.AddLink($"New Link3");

//			Link linkToBeDeleted = MVM.LinksVM.Links[index].Link;

//			// Delete a LinkViewModel.
//			MVM.LinksVM.DeleteLink(MVM.LinksVM.Links[index]);

//			// This assert only checks if two items are identical, not if they are the same object.
//			Assert.DoesNotContain(linkToBeDeleted, MVM.Model.Links);

//		}

//		[Fact]
//		public void AddLinkColumn_CreateLinkViewModelThenAddBusinessKeyViewModel_SynchronisesWithModel()
//		{
//			MainViewModel MVM = new();

//			MVM.LinksVM.AddLink($"New Link");
//			// Must set the SelectedLink as AddLinkColumn() takes no arguments.
//			MVM.LinksVM.SelectedLink = MVM.LinksVM.Links[0];
//			// Add a new LinkColumn, passing the LinkViewModel that the Column should be added to as an argument.
//			MVM.LinksVM.AddLinkColumn();

//			StagingColumn expected = MVM.LinksVM.Links[0].BusinessKeys[0].StagingColumn;
//			StagingColumn actual = MVM.Model.Links[0].BusinessKeys[0];

//			// Ensure that the StagingColumn in the model is the same object as the StagingColumn wrapped by
//			// the BusinessKeyViewModel in the corresponding Link.
//			Assert.Same(expected, actual);
//		}

//		[Fact]
//		public void DeleteLinkColumn_CreateLinkViewModelWithColumnsThenDeleteColumns_SynchronisesWithModel()
//		{
//			MainViewModel MVM = new();

//			// AddLink sets SelectedLink to the new LinkViewModel that's been created.
//			MVM.LinksVM.AddLink($"New Link");

//			// Add a new LinkColumn, passing the LinkViewModel that the Column should be added to as an argument.
//			MVM.LinksVM.AddLinkColumn();

//			// Must set SelectedLinkColumn to be able to remove LinkColumn.
//			MVM.LinksVM.SelectedLinkColumn = MVM.LinksVM.Links[0].BusinessKeys[0];
//			StagingColumn linkColumnToDelete = MVM.LinksVM.Links[0].BusinessKeys[0].StagingColumn;

//			// Deletes the SelectedLinkColumn.
//			MVM.LinksVM.DeleteLinkColumn();

//			Assert.DoesNotContain(linkColumnToDelete, MVM.Model.Links[0].BusinessKeys);
//		}
//	}
//}
