using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Round
{
	public partial class Freeze : BaseRound
	{
		public Freeze()
		{
			if ( !Host.IsServer ) return;

			var game = Game.Current as MiniGame;

			if ( !string.IsNullOrEmpty( game.NextMap ) )
			{
				Log.Info( $"Next map is {game.NextMap}" );
				ConsoleSystem.Run( $"changelevel {game.NextMap}" );
			}

			game.CleanUpMap();

			var players = Entity.All.OfType<MiniGamePlayer>();

			game.SetRoundTime( Config.FreezeTime );
			game.TeamRed.Clear();
			game.TeamBlue.Clear();

			if ( MapSettings.TeamType == 1 )
			{
				foreach ( var ply in players )
				{
					game.TeamRed.Join( ply );

					ply.Inventory.DeleteContents();
					ply.Respawn();
					ply.Freeze( true );
				}
			}
			else
			{
				List<MiniGamePlayer> PlayerTbl = new();
				int NeedPly;

				if ( MapSettings.TeamType == 2 )
					NeedPly = players.Count() / 2;
				else
				{
					NeedPly = MapSettings.PlyInTeam;

					if ( players.Count() <= NeedPly )
						NeedPly = players.Count() - 1;
				}

				for ( int i = 0; i < NeedPly; i++ )
				{
					var find = true;
					MiniGamePlayer rand = null;

					while ( find )
					{
						find = false;
						rand = players.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

						foreach ( var ply in PlayerTbl )
						{
							if ( rand == ply )
							{
								find = true;
								break;
							}
						}
					}

					if ( rand != null )
						PlayerTbl.Add( rand );
				}

				// FUCK System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
				for ( int i = 0; i < players.Count(); i++ )
				{
					var ply = players.ToList()[i];

					if ( PlayerTbl.Contains( ply ) )
						game.TeamRed.Join( ply );
					else
						game.TeamBlue.Join( ply );

					ply.Inventory.DeleteContents();
					ply.Respawn();
					ply.Freeze( true );
				}
			}

			if ( game.NumRounds == 1 )
			{
				//StartVote()
			}
		}
	}
}
