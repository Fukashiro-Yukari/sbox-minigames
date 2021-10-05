using Sandbox;
using System.Collections.Generic;

[Library( "info_map_settings" )]
//[Hammer.EditorModel( "models/editor/playerstart.vmdl", FixedBounds = true )]
[Hammer.EntityTool( "Map Settings", "Map", "Change map settings for minigames" )]
public class MapSettingsEntity : Entity
{
	[Property( Title = "Players God Mode" )]
	public bool God { get; set; } = false;

	[Property( Title = "Game Team Type" )]
	public int TeamType { get; set; } = 2;

	[Property( Title = "Players Need In Team" )]
	public int PlyInTeam { get; set; } = 0;

	[Property( Title = "Players Can Respawn" )]
	public bool Respawn { get; set; } = false;

	[Property( Title = "Round Time" )]
	public float Time { get; set; } = 600;

	[Property( Title = "Round Time" )]
	public float PlayerScale { get; set; } = 1;

	[Property( Title = "Players Running Speed" )]
	public float RunSpeed { get; set; } = 300.0f;

	[Property( Title = "Players Walk Speed" )]
	public float WalkSpeed { get; set; } = 150.0f;

	[Property( Title = "Players Jump Power" )]
	public float JumpPower { get; set; } = 268.3281572999747f;

	[Property( Title = "Players Allow Use Flashlight" )]
	public bool AllowFlashlight { get; set; } = true;

	[Property( Title = "Players Start Weapon" )]
	public string StartWeapon { get; set; } = "";

	[Property( Title = "Players Infinite Ammo" )]
	public bool InfiniteAmmo { get; set; } = false;

	[Property( Title = "Players No Collide" )]
	public bool PlayersNoCollide { get; set; } = true;

	[Property( Title = "Players Team Damage" )]
	public bool TeamDamage { get; set; } = false;

	[Property( Title = "Players Can Take Damage" )]
	public bool PlayersDamage { get; set; } = true;

	[Property( Title = "Players Fall Damage" )]
	public bool FallDamage { get; set; } = true;

	[Property( Title = "Players Drop Weapons After Death" )]
	public bool DropAfterDeath { get; set; } = true;

	[Property( Title = "Game Gravity" )]
	public float Gravity { get; set; } = 800.0f;

	[Property( Title = "Players God Mode Time" )]
	public float TempGod { get; set; } = 3;

	[Property( Title = "Game Rounds" )]
	public int NumRounds { get; set; } = 15;

	[Property( Title = "Players Can't Drop Weapons" )]
	public List<string> CantDrop { get; set; } = new()
	{
		"mg_knife"
	};
}
