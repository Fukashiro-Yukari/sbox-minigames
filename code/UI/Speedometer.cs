using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Speedometer : Panel
{
	public Label Speed;

	public Speedometer()
	{
		Speed = Add.Label( "0", "SpeedText" );
	}

	public override void Tick()
	{
		var ply = Local.Pawn;

		if ( ply == null ) return;

		Speed.Text = $"{ply.Velocity.Length.CeilToInt()} Vel";

		if ( ply is MiniGamesPlayer mply && mply.Team != null )
		{
			var color = mply.Team.Color.ToColor();

			Speed.Style.BackgroundColor = new Color( color.r, color.g, color.b, 0.5f );
			Speed.Style.Dirty();
		}
	}
}
