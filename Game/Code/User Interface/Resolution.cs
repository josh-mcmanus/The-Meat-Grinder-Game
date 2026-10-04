using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Resolution : Control
{
	
	[Export] OptionButton resolutionOptionButton;
	[Export] OptionButton windowModeOptionButton;
	[Export] OptionButton vsyncOptionButton;
	
	
	private List<Vector2I> commonResolutions =
    [
		// 16:9
        new(3840, 2160), // 4K
		new(2560, 1440), // 1440p
		new(1920, 1080), // 1080p
		new(1600, 900),
		new(1366, 768),
		new(1280, 720),  // 720p
		new(960,  540),
		
		// Ultrawide
		new(5120, 1440),
		new(3840, 1600),
		new(3440, 1440),
		new(2560, 1080),
		
		// 16:10
		new(2560, 1600),
		new(1920, 1200),
		new(1680, 1050),
		new(1440, 900),
		new(1280, 800),
		
		// Other common aspect ratios
		new(1280, 1024),
		new(1024, 768),
		new(1024, 600),
		new(800, 600)
	];
	
	private List<SettingsData.WindowMode> windowModes;
	
	private List<SettingsData.VSyncMode>  vsyncModes;
	
	
	public override void _Ready()
	{
		SetupResolutionOptionButton();
		SetupWindowModeOptionButton();
		SetupVSyncOptionButton();
	}
	
	
	
	private void SetupResolutionOptionButton()
	{
		// Add monitors native resolution to top if not in list
		Vector2I nativeResolution = Menu.GetPrimaryMonitorResolution();
		Vector2I savedResolution = DataManager.GetWindowResolution();
		
		if (!commonResolutions.Contains(nativeResolution)) 
		{
			commonResolutions.Insert(0, nativeResolution);
		}
		
		SetupOptionButton(resolutionOptionButton, commonResolutions, savedResolution, res => $"{res.X} x {res.Y}");
		
		resolutionOptionButton.ItemSelected += OnResolutionSelected;
	}
	
	private void OnResolutionSelected(long index)
	{
		Vector2I newResolution = commonResolutions[(int)index];
		
		DataManager.settingsData.DisplayResolutionWidth = newResolution.X;
		DataManager.settingsData.DisplayResolutionHeight = newResolution.Y;
		
		DataManager.SetWindowResolution();
	}
	
	
	private void SetupWindowModeOptionButton()
	{
		windowModes = [.. Enum.GetValues<SettingsData.WindowMode>()];
		
		SetupOptionButton(windowModeOptionButton, windowModes, DataManager.settingsData.Window, mode => mode.ToString());
		
		UpdateResolutionOptionButtonState();
		
		windowModeOptionButton.ItemSelected += OnWindowModeSelected;
	}
	
    private void OnWindowModeSelected(long index)
    {
        var mode = (SettingsData.WindowMode)index;
		
		DataManager.settingsData.Window = mode;
		
		DataManager.SetWindowMode();
		
		UpdateResolutionOptionButtonState();
    }
	
	
    private void SetupVSyncOptionButton()
	{
		vsyncModes = [.. Enum.GetValues<SettingsData.VSyncMode>()];
		
		SetupOptionButton(vsyncOptionButton, vsyncModes, DataManager.settingsData.VSync, mode => mode.ToString());
		
		vsyncOptionButton.ItemSelected += OnVSyncSelected;
	}
	
    private void OnVSyncSelected(long index)
    {
        var mode = (SettingsData.VSyncMode)index;
		
		DataManager.settingsData.VSync = mode;
		
		DataManager.SetVSyncMode();
    }
	
	
	
	// Reset and fill an option button with a list of items
	private void SetupOptionButton<T>(OptionButton optionButton, List<T> options, T selectedValue, Func<T, string> getDisplayText) where T : notnull
	{
		optionButton.Clear();
		
		foreach (T option in options)
		{
			optionButton.AddItem(getDisplayText(option));
		}
		
		SetSelectedOption(optionButton, options, selectedValue);
	}
	
	// Set the option button to the specified selected value from a list
	private void SetSelectedOption<T>(OptionButton optionButton, List<T> options, T selectedValue)
	{
		int selectedIndex = options.IndexOf(selectedValue);
		
		if (selectedIndex >= 0)
		{
			optionButton.Select(selectedIndex);
		}
	}
	
	
	
	// Save ALL changes to settings file
	private void SaveButtonPressed()
	{
		DataManager.SaveSettingsData();
	}
	
	// Dont save any changes, load saved settings data
	private void CancelButtonPressed()
	{
		DataManager.LoadSettingsData();
		DataManager.ApplyAllSettings();
		
		Vector2I res = DataManager.GetWindowResolution();
		
		SetSelectedOption(resolutionOptionButton, commonResolutions, res);
		SetSelectedOption(windowModeOptionButton, windowModes, DataManager.settingsData.Window);
		SetSelectedOption(vsyncOptionButton, vsyncModes, DataManager.settingsData.VSync);
		
		UpdateResolutionOptionButtonState();
	}
	
	
	// Enable / disable selecting resolution if windowed mode is selected
	// Only windowed mode uses resolution
	private void UpdateResolutionOptionButtonState()
	{
		resolutionOptionButton.Disabled = DataManager.settingsData.Window != SettingsData.WindowMode.Windowed;
	}
	
	
}
