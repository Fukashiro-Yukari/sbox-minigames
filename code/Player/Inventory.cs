using Sandbox;
using System;
using System.Linq;

partial class Inventory : BaseInventory
{
	public Inventory( Player player ) : base( player )
	{
	}

	public override bool CanAdd( Entity entity )
	{
		if ( !entity.IsValid() )
			return false;

		if ( !base.CanAdd( entity ) )
			return false;

		if ( Owner is MiniGamePlayer ply && ply.Team is Spectator )
			return false;

		if ( entity is Weapon weapon )
		{
			if ( weapon.Bucket >= 0 && weapon.Bucket <= 2 )
			{
				foreach ( var wep in List )
				{
					if ( wep is Weapon w )
					{
						if ( w.Bucket == weapon.Bucket ) return false;
					}
				}
			}
		}

		return !IsCarryingType( entity.GetType() );
	}

	public virtual bool CanReplace( Entity entity )
	{
		if ( !entity.IsValid() )
			return false;

		if ( !base.CanAdd( entity ) )
			return false;

		if ( entity is Weapon weapon )
		{
			if ( weapon.Bucket >= 0 && weapon.Bucket <= 2 )
			{
				foreach ( var wep in List )
				{
					if ( wep is Weapon w )
					{
						if ( w.Bucket == weapon.Bucket ) return true;
					}
				}
			}
		}

		return false;
	}

	public Entity GetReplaceEntity( Entity entity )
	{
		if ( entity is Weapon weapon )
		{
			if ( weapon.Bucket >= 0 && weapon.Bucket <= 2 )
			{
				foreach ( var wep in List )
				{
					if ( wep is Weapon w )
					{
						if ( w.Bucket == weapon.Bucket ) return w;
					}
				}
			}
		}

		return null;
	}

	public virtual Entity Replace( Entity entity )
	{
		if ( !Host.IsServer ) return null;

		var repent = GetReplaceEntity( entity );

		if ( repent != null && repent.IsValid )
		{
			var ac = Owner.ActiveChild;

			if ( Drop( repent ) )
			{
				if ( ac != null && ac == repent )
				{
					Owner.ActiveChild = null;
				}

				Owner.StartTouch( entity );

				return repent;
			}
		}

		return null;
	}

	public override bool Add( Entity entity, bool makeActive = false )
	{
		if ( !entity.IsValid() )
			return false;

		var player = Owner as MiniGamePlayer;
		var notices = !player.SupressPickupNotices;

		if ( IsCarryingType( entity.GetType() ) ) return false;
		if ( !CanAdd( entity ) ) return false;

		if ( entity is Weapon && notices && entity.Owner == null )
		{
			Sound.FromWorld( "dm.pickup_weapon", entity.Position );
		}

		return base.Add( entity, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x?.GetType() == t );
	}

	public override Entity DropActive()
	{
		if ( !Host.IsServer ) return null;

		var ac = Owner.ActiveChild;
		if ( ac == null ) return null;
		if ( MapSettings.CantDrop.Contains( ac.ClassInfo.Name ) ) return null;

		if ( Drop( ac ) )
		{
			Owner.ActiveChild = null;
			return ac;
		}

		return null;
	}

	public virtual bool DropAll()
	{
		if ( !Host.IsServer ) return false;

		Owner.ActiveChild = null;

		for ( int i = 0; i < List.Count; i++ )
		{
			var wep = List[i];

			if ( MapSettings.CantDrop.Contains( wep.ClassInfo.Name ) ) continue;

			Drop( wep );
		}

		return true;
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
