using Sandbox;
using System.Linq;

namespace Round
{
	public partial class Waiting : BaseRound
	{
		MiniGames game;

		public Waiting()
		{
			if ( !Host.IsServer ) return;

			game = Game.Current as MiniGames;
			game.SetRoundTime( 0 );

			foreach ( var ply in Entity.All.OfType<MiniGamesPlayer>() )
			{
				ply.Inventory.DeleteContents();
				ply.Freeze( false );

				game.TeamSpectator.Join( ply );
			}
		}

		public override void OnTick()
		{
			var plys = Entity.All.OfType<MiniGamesPlayer>();

			if ( plys.Count() > 1 )
				game.Round = new Freeze();
		}
	}
}
