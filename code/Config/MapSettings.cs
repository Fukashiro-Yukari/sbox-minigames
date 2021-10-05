using System.Collections.Generic;

public class MapSettings
{
	public static bool God { get; set; } = false;
	public static int TeamType { get; set; } = 2;
	public static int PlyInTeam { get; set; } = 0;
	public static bool Respawn { get; set; } = false;
	public static float Time { get; set; } = 600;
	public static float PlayerScale { get; set; } = 1;
	public static float RunSpeed { get; set; } = 300.0f;
	public static float WalkSpeed { get; set; } = 150.0f;
	public static float JumpPower { get; set; } = 268.3281572999747f;
	public static bool AllowFlashlight { get; set; } = true;
	public static string StartWeapon { get; set; } = "";
	public static bool InfiniteAmmo { get; set; } = false;
	public static bool PlayersNoCollide { get; set; } = true;
	public static bool TeamDamage { get; set; } = false;
	public static bool PlayersDamage { get; set; } = true;
	public static bool FallDamage { get; set; } = true;
	public static bool DropAfterDeath { get; set; } = true;
	public static float Gravity { get; set; } = 800.0f;
	public static float TempGod { get; set; } = 3;
	public static int NumRounds { get; set; } = 15;
	public static List<string> CantDrop = new()
	{
		"mg_knife"
	};
}
