
using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class DataManager : Node
{
	private const string TAG = "DataManager";
	public  const string GAME_VERSION = "0.1.0 Date: 04/10/26";
	
	public static GameData gameData = new();
    public static SettingsData settingsData = new();
    public enum   SaveTypes { game, settings }
    
    public static SoftBodyData softBodyData = new();
    
    
	public override void _Ready()
	{
        LaunchSetup();
        
		LoadAllData();
		ApplyAllSettings();
	}
    
    
    
    // Game Save data
    public static void SaveGameData() { Save(SaveTypes.game); }
    public static void LoadGameData() { Load(SaveTypes.game); }
    public static void CreateDefaultGameData() { CreateDefaultFile(SaveTypes.game); }
    
    // Settings data
    public static void SaveSettingsData() { Save(SaveTypes.settings); }
    public static void LoadSettingsData() { Load(SaveTypes.settings); }
    public static void CreateDefaultSettingsData() { CreateDefaultFile(SaveTypes.settings); }
    
    
    
    // Data manipulation
    private static void Save(SaveTypes saveType)
    {
        string filePath = CreateFilePath(saveType);
        
		// Update current save file version
		gameData.SaveFileVersion = GAME_VERSION;
		
		
        // Save File Object
        using FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        
        var options = new JsonSerializerOptions{ WriteIndented = true };
        string jsonString = string.Empty;
        
        
        // Convert class to json
        if (saveType == SaveTypes.game)
        {
            jsonString = JsonSerializer.Serialize(gameData, options);
            Utils.Log(TAG, "Save data converted to json");
        }
        else if (saveType == SaveTypes.settings)
        {
            jsonString = JsonSerializer.Serialize(settingsData, options);
            Utils.Log(TAG, "Settings converted to json");
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
        
        
        // Save data
        if (saveType == SaveTypes.game)
        {
            gameData = JsonSerializer.Deserialize<GameData>(jsonString);
            Utils.Log(TAG, "Player data loaded");
            
            if(gameData == null)
            {
                Utils.Err(TAG, "Failed to load player data");
                gameData = new GameData();
            }
            return;
        }
        
        // Settings data
        if (saveType == SaveTypes.settings)
        {
            settingsData = JsonSerializer.Deserialize<SettingsData>(jsonString);
            Utils.Log(TAG, "Settings loaded");
            
            if(settingsData == null)
            {
                Utils.Err(TAG, "Failed to load settings data");
                settingsData = new SettingsData();
            }
        }
    }
    
    
    public static void LoadAllData()
    {
        // Save data
        if (!DoesFileExist(SaveTypes.game))
        {
			Utils.Log(TAG, "No save file found");
            CreateDefaultGameData();
			SaveGameData();
        }
        
		// Game settings
        if (!DoesFileExist(SaveTypes.settings))
        {
			Utils.Log(TAG, "No settings file found");
			CreateDefaultSettingsData();
			SaveSettingsData();
        }
		
        LoadGameData();
		LoadSettingsData();
    }
    
	
    
    // Helper functions
    public static void CreateDefaultFile(SaveTypes saveType)
    {
        Utils.Log(TAG, "Creating new file with default values");
        
        if (saveType == SaveTypes.game)
        {
            gameData = new GameData();
            Utils.Log(TAG, "Default Save data file created");
            return;
        }
        
        if (saveType == SaveTypes.settings)
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
    
    
    private static void LaunchSetup()
    {
        if (!DoesFileExist(SaveTypes.settings))
        {
            Vector2I resolution = Utils.GetPrimaryMonitorResolution();
            
            settingsData.DisplayResolutionWidth  = resolution.X;
            settingsData.DisplayResolutionHeight = resolution.Y;
            settingsData.Window = SettingsData.WindowMode.FullScreen;
            
            SaveSettingsData();
        }
    }
    
    
	
	// Settings
    
    // Called on startup
    // Try to call individual functions when possible
	public static void ApplyAllSettings()
	{
        SetWindowMode();
		SetVSyncMode();
	}
    
    
    public static void SetWindowMode()
    {
        switch (settingsData.Window)
		{
			case SettingsData.WindowMode.Windowed:
                SetWindowResolution();
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
    }
    
    
    // Resolution only seems to be a factor in windowed mode
    // In exclusive / fullscreen it just sets resolution to whatever the monitor is
    public static void SetWindowResolution()
    {
        if (settingsData.Window != SettingsData.WindowMode.Windowed)
        {
            return;
        }
        
        DisplayServer.WindowSetSize(GetWindowResolution());
    }
    
    
    public static void SetVSyncMode()
    {
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
    
    
    public static Vector2I GetWindowResolution()
    {
        return new(
            settingsData.DisplayResolutionWidth,
			settingsData.DisplayResolutionHeight
        );
    }
    
    
}
