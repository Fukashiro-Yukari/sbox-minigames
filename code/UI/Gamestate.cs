using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

public partial class Gamestate : Panel
{
	public Label RedScore;
	public Label BlueScore;
	public Label Timer;
	public Label Rounds;

	public Gamestate()
	{
		var TimerBG = Add.Panel( "TimerBG" );

		RedScore = TimerBG.Add.Label( "0", "Red" );
		Timer = TimerBG.Add.Label( "00:00", "Timer" );
		BlueScore = TimerBG.Add.Label( "0", "Blue" );
		Rounds = Add.Label( "Round 0", "Round" );
	}

	public override void Tick()
	{
		var game = Game.Current as MiniGame;

		RedScore.Text = game.TeamRedScore.ToString();
		BlueScore.Text = game.TeamBlueScore.ToString();

		var ts = new TimeSpan( 0, 0, game.GetRoundTime() );

		if ( ts.Hours > 0 )
			Timer.Text = string.Format( "{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds );
		else
			Timer.Text = string.Format( "{0:00}:{1:00}", ts.Minutes, ts.Seconds );

		if ( game.Round is Round.Preparing ) Rounds.Text = "Preparing";
		else if ( game.Round is Round.Waiting ) Rounds.Text = $"Waiting ( {Entity.All.OfType<MiniGamePlayer>().Count()} / 2 )";
		else Rounds.Text = $"Round {game.NumRounds}";
	}
}
