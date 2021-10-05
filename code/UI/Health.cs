using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Health : Panel
{
    public Label Healthn;
    public Label PlayerName;
    public Panel HealthBar;
	public Image Avatar;

    public Health()
    {
        var HealthBarBG = Add.Panel("HealthBarBG");

        HealthBar = HealthBarBG.Add.Panel("HealthBar");
        Healthn = Add.Label("0", "HealthText");
        PlayerName = Add.Label("", "PlayerName");
    }

    bool setAvatar;

    public override void Tick()
    {
		var ply = Local.Pawn;

		if ( ply == null ) return;

		if ( !setAvatar )
		{
			setAvatar = true;
			Avatar = Add.Image( $"avatar:{Local.Client.SteamId}" );
		}

		PlayerName.Text = Local.Client.Name;

		Healthn.Text = ply.Health.CeilToInt().ToString();

        HealthBar.Style.Width = Length.Percent(ply.Health);
		HealthBar.Style.Dirty();

		if (ply is MiniGamePlayer mply && mply.Team != null )
		{
			var color = mply.Team.Color.ToColor();

			Style.BackgroundColor = new Color( color.r, color.g, color.b, 0.5f );
			Style.Dirty();
		}
	}
}
