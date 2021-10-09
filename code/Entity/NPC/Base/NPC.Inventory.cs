﻿using Sandbox;
using System;
using System.Linq;

partial class NPCInventory : BaseInventory
{
	public NPCInventory( NPC npc ) : base( npc )
	{
	}

	public override bool CanAdd( Entity entity )
	{
		if ( !entity.IsValid() )
			return false;

		if ( !base.CanAdd( entity ) )
			return false;

		if ( Owner is NPC npc && !npc.UseWeapon )
			return false;

		return !IsCarryingType( entity.GetType() );
	}

	public override bool Add( Entity entity, bool makeActive = false )
	{
		if ( !entity.IsValid() )
			return false;

		if ( IsCarryingType( entity.GetType() ) )
			return false;

		return base.Add( entity, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x?.GetType() == t );
	}

	public override bool Drop( Entity ent )
	{
		if ( !Host.IsServer )
			return false;

		if ( !Contains( ent ) )
			return false;

		ent.OnCarryDrop( Owner );

		return ent.Parent == null;
	}
}
