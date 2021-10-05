using Sandbox;
using System.Linq;

namespace Round
{
	public partial class Active : BaseRound
	{
		MiniGame game;

		public Active()
		{
			if ( !Host.IsServer ) return;

			game = Game.Current as MiniGame;
			game.SetRoundTime( MapSettings.Time );

			foreach ( var ply in Entity.All.OfType<MiniGamePlayer>() )
				ply.Freeze( false );

			MiniGame.AddHint( "RoundStart" );
		}

		public override void OnTick()
		{
			if ( MapSettings.Respawn ) return;
			if ( MapSettings.TeamType == 2 || MapSettings.TeamType == 3 )
			{
				var RedTeam = game.TeamRed.Players.Where( x => x.IsValid() && x.LifeState == LifeState.Alive );
				var BlueTeam = game.TeamBlue.Players.Where( x => x.IsValid() && x.LifeState == LifeState.Alive );
				var RedAlive = false;
				var BlueAlive = false;

				if ( RedTeam.Count() > 0 ) RedAlive = true;
				if ( BlueTeam.Count() > 0 ) BlueAlive = true;

				if ( !RedAlive && !BlueAlive )
				{
					game.Round = new Ending( null );

					return;
				}

				if ( !RedAlive )
				{
					game.Round = new Ending( game.TeamBlue );

					return;
				}

				if ( !BlueAlive )
				{
					game.Round = new Ending( game.TeamRed );

					return;
				}
			}
			else if ( MapSettings.TeamType == 1 )
			{
				var AlivePlayer = Entity.All.OfType<MiniGamePlayer>().Where( x => x.IsValid() && x.LifeState == LifeState.Alive && x.Team is not Spectator );

				if ( AlivePlayer.Count() == 0 )
				{
					game.Round = new Ending( null );

					return;
				}

				if ( AlivePlayer.Count() == 1 )
				{
					game.Round = new Ending( AlivePlayer.ToList()[0] );

					return;
				}
			}
		}
	}
}
