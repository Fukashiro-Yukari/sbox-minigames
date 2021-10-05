using Sandbox;

namespace Round
{
	public partial class Ending : BaseRound
	{
		public Ending()
		{
			// Create an empty method because the client will also load
			// Because is BaseNetworkable class
		}

		public Ending( object winner )
		{
			if ( !Host.IsServer ) return;

			var game = Game.Current as MiniGames;

			game.SetRoundTime( Config.EndTime );
			game.NumRounds--;

			if ( winner == null )
			{
				MiniGames.AddHint( "Draw" );
			}
			else if ( MapSettings.TeamType == 1 && winner is MiniGamesPlayer ply )
			{
				MiniGames.AddHint( $"Winner {ply.Client.Name}" );

				//winner: AddFrags( 1 )
			}
			else if ( (MapSettings.TeamType == 2 || MapSettings.TeamType == 3) && winner is BaseTeam team )
			{
				if ( team is Red )
				{
					MiniGames.AddHint( "Red Team Won" );

					game.TeamRed.AddScore();
				}
				else if ( team is Blue )
				{
					MiniGames.AddHint( "Blue Team Won" );

					game.TeamBlue.AddScore();
				}
			}
		}
	}
}
