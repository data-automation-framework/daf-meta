// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;

namespace Daf.Meta.Editor
{
	public sealed class ModifiedRelationships { }

	public sealed class RefreshedMetadata { }

	/// <summary>
	/// Facilitates messages that inform MainViewModel that a specified Hub needs to be removed from Model.Hubs.
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
	/// Facilitates messages that inform MainViewModel that a new Hub needs to be created in Model.Hubs.
	/// <param name="hub">The Hub object to be added.</param>
	/// </summary>
	public sealed class AddHubToModel
	{
		public AddHubToModel(Hub hub)
		{
			Hub = hub;
		}

		public Hub Hub { get; }
	}

	/// <summary>
	/// Facilitates messages that inform MainViewModel that a specified Link needs to be removed from Model.Links.
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
	/// Facilitates messages that inform MainViewModel that a new Link needs to be created in Model.Links.
	/// <param name="link">The Link object to be added.</param>
	/// </summary>
	public sealed class AddLinkToModel
	{
		public AddLinkToModel(Link link)
		{
			Link = link;
		}

		public Link Link { get; }
	}

	/// <summary>
	/// Facilitates messages that inform MainViewModel that a StagingColumn needs to be added to a specified Link in Model.Links.
	/// <param name="link">The Link that the specified BusinessKey will be added to.</param>
	/// <param name="businessKey">The StagingColumn object to be added.</param>
	/// </summary>
	public sealed class AddBusinessKeyColumnToLink
	{
		public AddBusinessKeyColumnToLink(Link link, StagingColumn businessKey)
		{
			BusinessKey = businessKey;
			Link = link;
		}

		public StagingColumn BusinessKey { get; }
		public Link Link { get; }
	}

	/// <summary>
	/// Facilitates messages that inform MainViewModel that a StagingColumn needs to be removed from a specified Link in Model.Links.
	/// <param name="link">The Link that the specified BusinessKey belongs to.</param>
	/// <param name="businessKey">The StagingColumn object to be removed.</param>
	/// </summary>
	public sealed class RemoveBusinessKeyColumnFromLink
	{
		public RemoveBusinessKeyColumnFromLink(Link link, StagingColumn businessKey)
		{
			BusinessKey = businessKey;
			Link = link;
		}

		public StagingColumn BusinessKey { get; }
		public Link Link { get; }
	}

	/// <summary>
	/// Facilitates messages that inform MainViewModel that a specified Connection needs to be removed from Model.Connections.
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
	/// Facilitates messages that inform MainViewModel that a new Connection needs to be created in Model.Connections.
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
	/// For announcing to subscribers that a new Tenant has been created.
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
	/// For announcing to subscribers that a new SourceSystem has been created.
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
	/// For announcing to subscribers that a new HubRelationship has been created.
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
}
