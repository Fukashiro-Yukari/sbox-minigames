using Sandbox;
using System.Collections.Generic;

public partial class BaseTeam : BaseNetworkable
{
	public virtual string Name => null;
	public virtual Color32 Color => Color32.White;

	[Net]
	public int Score { get; set; }

	public readonly List<Player> Players = new();

	public virtual int Count => Players.Count;

	public virtual void Join( Player ply )
	{
		if ( ply is MiniGamesPlayer p )
		{
			if ( p.Team != null )
				p.Team.Exit( p );

			p.Team = this;
		}

		if ( !Players.Contains( ply ) )
			Players.Add( ply );
	}

	public virtual void Exit( Player ply )
	{
		Players.Remove( ply );

		if ( ply is MiniGamesPlayer p )
			p.Team = null;
	}

	public virtual void Clear()
	{
		foreach ( var ply in Players )
		{
			if ( ply is MiniGamesPlayer p )
				p.Team = null;
		}

		Players.Clear();
	}

	public virtual void AddScore()
	{
		Score++;
	}
}
