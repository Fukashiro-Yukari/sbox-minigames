namespace Sandbox
{
	/// <summary>
	/// This entity defines the spawn point of the player in first person shooter gamemodes.
	/// </summary>
	[Library( "info_player_red" )]
	[Hammer.EditorModel( "models/editor/playerstart.vmdl", FixedBounds = true )]
	[Hammer.EntityTool( "Red Team Player Spawnpoint", "Player", "Defines a point where the player can (re)spawn" )]
	public class RedSpawnPoint : Entity
	{

	}
}
