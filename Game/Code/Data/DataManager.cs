
using Godot;
using System;
using System.Text.Json;

public partial class DataManager : Node
{
	private const string TAG = "DataManager";
	public  const string GAME_VERSION = "0.1.0 Date: 25/09/26";
	
	public static PlayerData playerData = new();
    public static SettingsData settingsData = new();
    public enum   SaveTypes { player, game }


	public override void _Ready()
	{
		LoadAllData();
		ApplyAllSettingsStartup();
	}

	
    
    // Player data
    public static void SavePlayerData() { Save(SaveTypes.player); }
    public static void LoadPlayerData() { Load(SaveTypes.player); }
    public static void CreateDefaultPlayerData() { CreateDefaultFile(SaveTypes.player); }
    
    // Game data
    public static void SaveGameSettings() { Save(SaveTypes.game); }
    public static void LoadGameSettings() { Load(SaveTypes.game); }
    public static void CreateDefaultGameData() { CreateDefaultFile(SaveTypes.game); }
    
    
    
    // Data manipulation
    private static void Save(SaveTypes saveType)
    {
        string filePath = CreateFilePath(saveType);
        
		// Update current save file version
		playerData.SaveFileVersion = GAME_VERSION;
		
		
        // Save File Object
        using FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        
        var options = new JsonSerializerOptions{ WriteIndented = true };
        string jsonString = string.Empty;
        
        
        // Convert class to json
        if (saveType == SaveTypes.player)
        {
            jsonString = JsonSerializer.Serialize(playerData, options);
            Utils.Log(TAG, "Player data converted to json");
        }
        else if (saveType == SaveTypes.game)
        {
            jsonString = JsonSerializer.Serialize(settingsData, options);
            Utils.Log(TAG, "Game settings converted to json");
        }
        else
        {
            Utils.Err(TAG, "Save did not complete. Could not find save type: " + saveType);
            return;
        }
        
        // Write json to File
        file.StoreString(jsonString);
        Utils.Log(TAG, "Data written to file");
    }
    
    
    private static void Load(SaveTypes saveType)
    {
        string filePath = CreateFilePath(saveType);
        
        // File doesnt exist
        if (!DoesFileExist(saveType))
        {
            Utils.Err(TAG, "File does not exist at path: " + filePath);
            CreateDefaultFile(saveType);
        }
        
        
        // Open and read file
        using FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();
        
        
        // File has no data
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            Utils.Err(TAG, "File is empty");
            CreateDefaultFile(saveType);
        }
        
        
        // Player data
        if (saveType == SaveTypes.player)
        {
            playerData = JsonSerializer.Deserialize<PlayerData>(jsonString);
            Utils.Log(TAG, "Player data loaded");
            
            if(playerData == null)
            {
                Utils.Err(TAG, "Failed to load player data");
                playerData = new PlayerData();
            }
            return;
        }
        
        // Game data
        if (saveType == SaveTypes.game)
        {
            settingsData = JsonSerializer.Deserialize<SettingsData>(jsonString);
            Utils.Log(TAG, "Game settings loaded");
            
            if(settingsData == null)
            {
                Utils.Err(TAG, "Failed to load game data");
                settingsData = new SettingsData();
            }
        }
    }
    
    
    public static void LoadAllData()
    {
        // Player data
        if (!DoesFileExist(SaveTypes.player))
        {
			Utils.Log(TAG, "No save file found");
            CreateDefaultPlayerData();
			SavePlayerData();
        }
        
		// Game settings
        if (!DoesFileExist(SaveTypes.game))
        {
			Utils.Log(TAG, "No settings file found");
			CreateDefaultGameData();
			SaveGameSettings();
        }
		
        LoadPlayerData();
		LoadGameSettings();
    }
    
	
    
    // Helper functions
    public static void CreateDefaultFile(SaveTypes saveType)
    {
        Utils.Log(TAG, "Creating new file with default values");
        
        if (saveType == SaveTypes.player)
        {
            playerData = new PlayerData();
            Utils.Log(TAG, "Default player save file created");
            return;
        }
        
        if (saveType == SaveTypes.game)
        {
            settingsData = new SettingsData();
            Utils.Log(TAG, "Default game settings file created");
            return;
        }
    }
    
    
    private static string CreateFilePath(SaveTypes saveType)
    {
        return "user://" + saveType.ToString() + ".json";
    }
    
    
    public static bool DoesFileExist(SaveTypes saveType)
    {
        string filePath = CreateFilePath(saveType);
        
        if (FileAccess.FileExists(filePath))
        {
            return true;
        }
        
        return false;
    }
    
    
	
	// Settings
	private static void ApplyAllSettingsStartup()
	{
		// Game Window
		DisplayServer.WindowSetSize(new Vector2I(settingsData.ResolutionWidth, settingsData.ResolutionHeight));
		
		switch (settingsData.Window)
		{
			case SettingsData.WindowMode.Windowed:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			
			case SettingsData.WindowMode.FullScreen:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			
			case SettingsData.WindowMode.Borderless:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
				break;
		}
		
		switch (settingsData.VSync)
		{
			case SettingsData.VSyncMode.Disabled:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
				break;
			
			case SettingsData.VSyncMode.Enabled:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
				break;
			
			case SettingsData.VSyncMode.Adaptive:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
				break;
			
			case SettingsData.VSyncMode.Mailbox:
				DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Mailbox);
				break;
		}
		
		
	}
    
    
}
