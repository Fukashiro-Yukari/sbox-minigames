using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

partial class MiniGames : Game
{
	[Net, Predicted] public BaseTeam TeamRed { get; set; }
	[Net, Predicted] public BaseTeam TeamBlue { get; set; }
	[Net, Predicted] public BaseTeam TeamSpectator { get; set; }
	[Net] public Round.BaseRound Round { get; set; }
	[Net] public float RoundTime { get; set; }
	[Net] public int NumRounds { get; set; }

	// NIce
	[Net] public int TeamRedScore { get; set; }
	[Net] public int TeamBlueScore { get; set; }

	public string NextMap;

	public MiniGames()
	{
		if ( IsServer )
		{
			TeamRed = new Red();
			TeamBlue = new Blue();
			TeamSpectator = new Spectator();
			Round = new Round.Preparing();
			NumRounds = MapSettings.NumRounds;

			_ = new MiniGamesHud();
		}
	}

	public int GetRoundTime()
	{
		return (int)MathF.Round( MathF.Max( RoundTime - Time.Now, 0 ) );
	}

	public void SetRoundTime( float time )
	{
		if ( !Host.IsServer ) return;

		RoundTime = Time.Now + time;
	}

	[Event.Tick.Server]
	public void OnServerTick()
	{
		TeamRedScore = TeamRed.Score;
		TeamBlueScore = TeamBlue.Score;

		if ( Round == null ) return;
		if ( !(Round is Round.Waiting) && !(Round is Round.Preparing) )
		{
			var plys = All.OfType<MiniGamesPlayer>();

			if ( plys.Count() < 2 )
			{
				AddHint( "Not Enough Players" );

				Round = new Round.Waiting();

				return;
			}
		}

		Round.OnTick();

		if ( GetRoundTime() == 0 && !(Round is Round.Waiting) )
		{
			if ( Round is Round.Preparing )
				Round = new Round.Freeze();
			else if ( Round is Round.Freeze )
				Round = new Round.Active();
			else if ( Round is Round.Active )
				Round = new Round.Ending( null );
			else if ( Round is Round.Ending )
				Round = new Round.Freeze();
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new MiniGamesPlayer( cl );

		if ((Round is Round.Freeze || MapSettings.Respawn ) && !(Round is Round.Waiting) && !(Round is Round.Preparing) )
		{
			switch ( MapSettings.TeamType )
			{
				case 1:
					TeamRed.Join( player );

					break;
				case 2:
					if ( TeamRed.Count > TeamBlue.Count )
						TeamBlue.Join( player );
					else
						TeamRed.Join( player );

					break;
				case 3:
					TeamBlue.Join( player );

					break;
			}

			if ( MapSettings.Respawn )
			{
				player.Freeze( true );
			}
		}
		else
		{
			TeamSpectator.Join( player );
		}

		player.Respawn();

		cl.Pawn = player;
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );
	}

	//TODO: CleanUpMap
	public virtual void CleanUpMap()
	{

	}

	[ServerCmd("give_weapon")]
	public static void GiveWeapon( string entName )
	{
		if ( ConsoleSystem.GetValue( "sv_cheats" ) == "0" ) return;

		var target = ConsoleSystem.Caller.Pawn;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		var wep = Library.Create<Weapon>( entName );

		if ( wep != null && wep.IsValid )
		{
			inventory.Add( wep );
		}
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( ConsoleSystem.GetValue( "sv_cheats" ) == "0" ) return;
		if ( player.Pawn is MiniGamesPlayer ply && ply.Team is Spectator ) return;

		base.DoPlayerNoclip( player );
	}

	public override void DoPlayerSuicide( Client cl )
	{
		if ( cl.Pawn is MiniGamesPlayer ply && ply.Team is Spectator ) return;

		base.DoPlayerSuicide( cl );
	}

	public override void DoPlayerDevCam( Client player )
	{
		//if ( ConsoleSystem.GetValue( "sv_cheats" ) == "0" ) return;

		base.DoPlayerDevCam( player );
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		if ( pawn is MiniGamesPlayer ply )
		{
			Entity spawnpoint;

			if ( ply.Team != null )
			{
				if ( ply.Team is Red )
					spawnpoint = All.OfType<RedSpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
				else if ( ply.Team is Blue )
					spawnpoint = All.OfType<BlueSpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
				else
					spawnpoint = All.OfType<RedSpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();
			}
			else
				spawnpoint = All.OfType<RedSpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( spawnpoint == null )
				spawnpoint = All.OfType<SpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( spawnpoint == null )
			{
				Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
				return;
			}

			pawn.Transform = spawnpoint.Transform;
		}
	}

	public override void OnKilled( Entity pawn )
	{
		Host.AssertServer();

		var client = pawn.Client;
		if ( client != null )
		{
			OnKilled( client, pawn );
		}
		else
		{
			OnEntKilled( pawn );
		}
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		Host.AssertServer();

		var isHeadShot = false;

		Log.Info( $"{client.Name} was killed" );

		if ( pawn is MiniGamesPlayer ply )
			isHeadShot = ply.IsHeadShot;

		if ( pawn.LastAttacker != null )
		{
			var attackerClient = pawn.LastAttacker.Client;

			if ( attackerClient != null )
			{
				if ( pawn.LastAttackerWeapon != null )
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttackerWeapon.ClassInfo?.Name, isHeadShot );
				else
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, client.SteamId, client.Name, pawn.LastAttacker.ClassInfo?.Name, isHeadShot );
			}
			else
			{
				OnKilledMessage( (ulong)pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), client.SteamId, client.Name, "killed", isHeadShot );
			}
		}
		else
		{
			OnKilledMessage( 0, "", client.SteamId, client.Name, "died", isHeadShot );
		}
	}

	public void OnEntKilled( Entity ent )
	{
		Host.AssertServer();

		if ( ent.LastAttacker != null )
		{
			var attackerClient = ent.LastAttacker.Client;

			if ( attackerClient != null )
			{
				if ( ent.LastAttackerWeapon != null )
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttackerWeapon?.ClassInfo?.Name );
				else
					OnKilledMessage( attackerClient.SteamId, attackerClient.Name, ent.ClassInfo.Title, ent.LastAttacker.ClassInfo?.Name );
			}
			else
			{
				OnKilledMessage( (ulong)ent.LastAttacker.NetworkIdent, ent.LastAttacker.ToString(), ent.ClassInfo.Title, "killed" );
			}
		}
		else
		{
			OnKilledMessage( 0, "", ent.ClassInfo.Title, "died" );
		}
	}

	[ClientRpc]
	public virtual void OnKilledMessage( ulong leftid, string left, ulong rightid, string right, string method, bool isHeadShot )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( leftid, left, rightid, right, method, isHeadShot );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( ulong leftid, string left, string right, string method )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( leftid, left, right, method );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( string left, ulong rightid, string right, string method )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( left, rightid, right, method );
	}

	[ClientRpc]
	public virtual void OnKilledMessage( string left, string right, string method )
	{
		var kf = Sandbox.UI.KillFeed.Current as KillFeed;

		kf?.AddEntry( left, right, method );
	}

	public static void AddHint( string text )
	{
		if ( Host.IsClient ) return;

		AddHintMessage( text );
	}

	[ClientRpc]
	public static void AddHintMessage( string text )
	{
		Hint.Current?.AddHint( text );
	}
}
