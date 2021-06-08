// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;

namespace Daf.Meta.Editor
{
	public sealed class RefreshedMetadata { }

	/// <summary>
	/// For announcing to subscribers that a Hub needs to be removed.
	/// <param name="hub">The Hub object to be removed.</param>
	/// </summary>
	public sealed class DeleteHub
	{
		public DeleteHub(Hub hub)
		{
			Hub = hub;
		}

		public Hub Hub { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new Hub will be created.
	/// <param name="name">The name of the new Hub.</param>
	/// </summary>
	public sealed class AddHubToModel
	{
		public AddHubToModel(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a Link needs to be removed.
	/// <param name="link">The Link object to be removed.</param>
	/// </summary>
	public sealed class DeleteLink
	{
		public DeleteLink(Link link)
		{
			Link = link;
		}

		public Link Link { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new Link will be created.
	/// <param name="name">The name of the new Link.</param>
	/// </summary>
	public sealed class AddLinkToModel
	{
		public AddLinkToModel(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a specified Tenant will be removed.
	/// <param name="connection">The Connection object to be removed.</param>
	/// </summary>
	public sealed class RemoveConnection
	{
		public RemoveConnection(Connection connection)
		{
			Connection = connection;
		}

		public Connection Connection { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new Tenant will be created.
	/// <param name="connection">The Connection object to be added.</param>
	/// </summary>
	public sealed class AddConnection
	{
		public AddConnection(Connection connection)
		{
			Connection = connection;
		}

		public Connection Connection { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a specified Tenant will be removed.
	/// <param name="tenant">The Tenant object to be removed.</param>
	/// </summary>
	public sealed class RemoveTenant
	{
		public RemoveTenant(Tenant tenant)
		{
			Tenant = tenant;
		}

		public Tenant Tenant { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new Tenant will be created.
	/// <param name="tenant">The Tenant object that was created.</param>
	/// </summary>
	public sealed class AddTenant
	{
		public AddTenant(Tenant tenant)
		{
			Tenant = tenant;
		}

		public Tenant Tenant { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a specified SourceSystem will be removed.
	/// <param name="sourceSystem">The SourceSystem object to be removed.</param>
	/// </summary>
	public sealed class RemoveSourceSystem
	{
		public RemoveSourceSystem(SourceSystem sourceSystem)
		{
			SourceSystem = sourceSystem;
		}

		public SourceSystem SourceSystem { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new SourceSystem will be created.
	/// <param name="sourceSystem">The SourceSystem object to be added.</param>
	/// </summary>
	public sealed class AddSourceSystem
	{
		public AddSourceSystem(SourceSystem sourceSystem)
		{
			SourceSystem = sourceSystem;
		}

		public SourceSystem SourceSystem { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new HubRelationship will be created.
	/// <param name="hub">The Hub that the HubRelationship belongs to.</param>
	/// <param name="dataSource">The DataSource that the HubRelationship belongs to.</param>
	/// </summary>
	public sealed class AddHubRelationship
	{
		public AddHubRelationship(Hub hub, DataSource dataSource)
		{
			Hub = hub;
			DataSource = dataSource;
		}

		public Hub Hub { get; }
		public DataSource DataSource { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a HubRelationship needs to be removed.
	/// <param name="hub">The Hub that the HubRelationship belongs to.</param>
	/// <param name="dataSource">The DataSource that the HubRelationship belongs to.</param>
	/// </summary>
	public sealed class RemoveHubRelationship
	{
		public RemoveHubRelationship(HubRelationship hubRelationship, DataSource dataSource)
		{
			HubRelationship = hubRelationship;
			DataSource = dataSource;
		}

		public HubRelationship HubRelationship { get; }
		public DataSource DataSource { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a new LinkRelationship will be created.
	/// <param name="link">The Link that the LinkRelationship belongs to.</param>
	/// <param name="dataSource">The DataSource that the LinkRelationship belongs to.</param>
	/// </summary>
	public sealed class AddLinkRelationship
	{
		public AddLinkRelationship(Link link, DataSource dataSource)
		{
			Link = link;
			DataSource = dataSource;
		}

		public Link Link { get; }
		public DataSource DataSource { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a LinkRelationship needs to be removed.
	/// <param name="link">The Link that the LinkRelationship belongs to.</param>
	/// <param name="dataSource">The DataSource that the LinkRelationship belongs to.</param>
	/// </summary>
	public sealed class RemoveLinkRelationship
	{
		public RemoveLinkRelationship(LinkRelationship linkRelationship, DataSource dataSource)
		{
			LinkRelationship = linkRelationship;
			DataSource = dataSource;
		}

		public LinkRelationship LinkRelationship { get; }
		public DataSource DataSource { get; }
	}

	/// <summary>
	/// For announcing to subscribers that a StagingColumn has been added or removed.
	/// </summary>
	public sealed class StagingColumnAddedRemoved { }

	/// <summary>
	/// For announcing to subscribers that a HubMapping on a HubRelationship has changed.
	/// </summary>
	public sealed class HubLinkRelationshipChanged { }
}
