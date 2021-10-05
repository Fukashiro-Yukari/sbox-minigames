using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Ammo : Panel
{
    public Label Clip;
    public Label Slash;
    public Label AmmoText;

    public Ammo()
    {
        Clip = Add.Label("0", "Clip");
        Slash = Clip.Add.Label("/");
        AmmoText = Slash.Add.Label("0", "AmmoText");
    }

    private void Clear()
    {
        Clip.Text = "";
        Slash.Text = "";
        AmmoText.Text = "";
    }

    public override void Tick()
    {
        base.Tick();

        var player = Local.Pawn;

        if (player == null)
        {
            Clear();
            return;
        }

        var wep = player.ActiveChild as Weapon;

        if (wep == null)
        {
            Clear();
            return;
        }

        var clipsize = wep.ClipSize;
		var ammoMultiplier = wep.AmmoMultiplier;
		var ammo = wep.AmmoCount;
        var clipammo = wep.AmmoClip;

		if ( clipsize < 0 && ammoMultiplier <= 0 )
        {
            Clear();
            return;
        }

		if ( clipsize < 0 )
		{
			Clip.Text = "";
			Slash.Text = "";
			AmmoText.Text = $"{ammo}";

			return;
		}

        if (clipammo > clipsize)
            Clip.Text = $"{clipammo - (clipammo - clipsize)} + {clipammo - clipsize}";
        else
            Clip.Text = $"{clipammo}";

        Slash.Text = "/";
        AmmoText.Text = $"{ammo}";
    }
}
