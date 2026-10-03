using Godot;
using System;
using System.Collections.Generic;

public partial class Resolution : Control
{
	
	[Export] OptionButton resolutionDropDownList;
	
	private bool resolutionFound = false;
	
	
	private readonly List<Vector2I> CommonResolutions =
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
	
	private readonly List<Vector2I> ResolutionOptions = [];
	
	
	public override void _Ready()
	{
		Vector2I nativeResolution = Menu.GetPrimaryMonitorResolution();
		
		
		// Add resolutions, including monitors native if not in list
		if (!CommonResolutions.Contains(nativeResolution)) 
		{
			ResolutionOptions.Add(nativeResolution);
		} 
		
		ResolutionOptions.AddRange(CommonResolutions);
		
		
		// Reset and add resolutions to drop down
		resolutionDropDownList.Clear();
		
		for (int i = 0; i < ResolutionOptions.Count; i++)
		{
			Vector2I size = ResolutionOptions[i];
			resolutionDropDownList.AddItem($"{size.X} x {size.Y}");
		}
		
		
		// Select primary monitor resolution in drop down list
		int nativeIndex = ResolutionOptions.IndexOf(nativeResolution);
		
		if (nativeIndex >= 0)
		{
			resolutionDropDownList.Select(nativeIndex);
			DataManager.settingsData.DisplayResolutionWidth = nativeResolution.X;
			DataManager.settingsData.DisplayResolutionHeight = nativeResolution.Y;
		}
		
		// Update resolution on change
		resolutionDropDownList.ItemSelected += OnResolutionSelected;
	}
	
	
	private void OnResolutionSelected(long index)
	{
		Vector2I newResolution = ResolutionOptions[(int)index];
		
		DataManager.settingsData.DisplayResolutionWidth = newResolution.X;
		DataManager.settingsData.DisplayResolutionHeight = newResolution.Y;
		DataManager.SaveSettingsData();
		DataManager.ApplyAllSettings();
	}
	
	
}
