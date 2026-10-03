using System;

public class GameData
{
	public string SaveFileVersion { get; set; } = DataManager.GAME_VERSION;
	public TimeSpan TimePlayed { get; set; } = TimeSpan.Zero;
	
}
