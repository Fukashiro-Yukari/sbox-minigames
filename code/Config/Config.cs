using System.Collections.Generic;

public class Config
{
	public static int NumMaps { get; set; } = 6;
	public static float VoteTime { get; set; } = 30;
	public static float Percent { get; set; } = 50;
	public static bool UsePrefixes { get; set; } = true;
	//publistatic c bool Restriction { get; set; } = true;
	public static int MinVotes { get; set; } = 3;
	public static float PreparationTime { get; set; } = 20;
	public static float EndTime { get; set; } = 5;
	public static float FreezeTime { get; set; } = 3;
	public static List<string> ChatCommands { get; set; } = new()
	{
		"rtv",
		"!rtv",
		"/rtv"
	};
	public static List<string> Prefixes { get; set; } = new()
	{
		"mg_"
	};

	public Config()
	{
		//var test = new Test();

		//BaseFileSystem.WriteJson<Test>( "config/gmconfig.json", test );
	}
}
