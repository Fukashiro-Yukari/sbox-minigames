using Sandbox;

namespace Round
{
	public partial class Preparing : BaseRound
	{
		public Preparing()
		{
			if ( !Host.IsServer ) return;

			(Game.Current as MiniGames).SetRoundTime( Config.PreparationTime );
		}
	}
}
