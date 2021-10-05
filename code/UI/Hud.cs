using Sandbox;
using Sandbox.UI;

[Library]
public partial class MiniGamesHud : HudEntity<RootPanel>
{
	public MiniGamesHud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load("/ui/hud.scss");

		RootPanel.AddChild<Health>();
		RootPanel.AddChild<Speedometer>();
		RootPanel.AddChild<Gamestate>();
		RootPanel.AddChild<Ammo>();
		RootPanel.AddChild<InventoryBar>();
        RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();
		RootPanel.AddChild<MiniGamesNameTags>();
		RootPanel.AddChild<CrosshairCanvas>();
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Hint>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
	}
}
