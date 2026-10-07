using Godot;
using System;

public partial class VolumeSettings : Control
{
	private const string TAG = "VolumeSettings";
	
	
	[Export] LineEdit masterLineEdit;
	[Export] LineEdit musicLineEdit;
	[Export] LineEdit SFXLineEdit;
	[Export] LineEdit UILineEdit;
	
	[Export] HSlider masterSlider;
	[Export] HSlider musicSlider;
	[Export] HSlider SFXSlider;
	[Export] HSlider UISlider;
	
	private Utils.SettingControl<Utils.VolumeType>[] volumeControls;
	
	
	private const string Format = "0.00";
	private const float  LowerBound = 0;
	private const float  UpperBound = 1;
	
	
	
	public override void _Ready()
	{
		// Prevent processing when paused
		ProcessMode = ProcessModeEnum.WhenPaused;
		
		volumeControls = 
		[
			new(masterLineEdit, masterSlider, Utils.VolumeType.Master, Format, LowerBound, UpperBound),
			new(musicLineEdit, musicSlider, Utils.VolumeType.Music, Format, LowerBound, UpperBound),
			new(SFXLineEdit, SFXSlider, Utils.VolumeType.SFX, Format, LowerBound, UpperBound),
			new(UILineEdit, UISlider, Utils.VolumeType.UI, Format, LowerBound, UpperBound),
		];
		
		LoadAllSettingsData();
		
		Utils.Log(TAG, "Volume Menu Loaded");
		
		
		// Connect UI to functions
		masterLineEdit.TextSubmitted += text => OnTextSubmit(volumeControls[0], text);
		musicLineEdit.TextSubmitted += text => OnTextSubmit(volumeControls[1], text);
		SFXLineEdit.TextSubmitted += text => OnTextSubmit(volumeControls[2], text);
		UILineEdit.TextSubmitted += text => OnTextSubmit(volumeControls[3], text);
		
		masterLineEdit.TextChanged += text => Utils.SetLineEditText(text, masterLineEdit.CaretColumn, masterLineEdit, Utils.FormatType.Float);
		musicLineEdit.TextChanged += text => Utils.SetLineEditText(text, musicLineEdit.CaretColumn, musicLineEdit, Utils.FormatType.Float);
		SFXLineEdit.TextChanged += text => Utils.SetLineEditText(text, SFXLineEdit.CaretColumn, SFXLineEdit, Utils.FormatType.Float);
		UILineEdit.TextChanged += text => Utils.SetLineEditText(text, UILineEdit.CaretColumn, UILineEdit, Utils.FormatType.Float);
		
		masterSlider.ValueChanged += value => UpdateSetting(volumeControls[0], (float)value);
		musicSlider.ValueChanged += value => UpdateSetting(volumeControls[1], (float)value);
		SFXSlider.ValueChanged += value => UpdateSetting(volumeControls[2], (float)value);
		UISlider.ValueChanged += value => UpdateSetting(volumeControls[3], (float)value);
		
	}
	
	
	
	// Run when line edit field data is submitted (Enter, Controller confirm, etc)
	// Updates volume if the submitted string is valid
	private void OnTextSubmit(Utils.SettingControl<Utils.VolumeType> control, string text)
	{
		Utils.OnTextSubmit(control, text, UpdateSetting);
	}
	
	
    // Set UI elements, audio player volume, and save to settingsData
    private void UpdateSetting(Utils.SettingControl<Utils.VolumeType>  control, float value)
	{
		float result = Utils.UpdateValues(control, value);
		AudioServer.SetBusVolumeDb((int)control.Type, Mathf.LinearToDb(result));
		SaveSetting(control.Type, result);
	}
	
	
	// Save volume to settingsData for a specified type
	private void SaveSetting(Utils.VolumeType type, float value)
	{
		switch (type)
		{
			case Utils.VolumeType.Master:
				DataManager.settingsData.MasterVolume = value;
				break;
			
			case Utils.VolumeType.Music:
				DataManager.settingsData.MusicVolume = value;
				break;
			
			case Utils.VolumeType.SFX:
				DataManager.settingsData.SFXVolume = value;
				break;
			
			case Utils.VolumeType.UI:
				DataManager.settingsData.UIVolume = value;
				break;
		}
	}
	
	
	// Return volume for a specified type
	private float GetVolume(Utils.VolumeType type)
	{
		return type switch
		{
			Utils.VolumeType.Master => DataManager.settingsData.MasterVolume,
			Utils.VolumeType.Music => DataManager.settingsData.MusicVolume,
			Utils.VolumeType.SFX => DataManager.settingsData.SFXVolume,
			Utils.VolumeType.UI => DataManager.settingsData.UIVolume,
			_ => 1.0f
		};
	}
	
	
	
	// Saving and Loading
	
	// Save ALL changes to settings file
	private void SaveButtonPressed()
	{
		DataManager.SaveSettingsData();
	}
	
	// Dont save any changes, load saved settings data
	private void CancelButtonPressed()
	{
		DataManager.LoadSettingsData();

		LoadAllSettingsData();
	}
	
	// Load and set all volume data from settings
	private void LoadAllSettingsData()
	{
		foreach (Utils.SettingControl<Utils.VolumeType> control in volumeControls)
		{
			UpdateSetting(control, GetVolume(control.Type));
		}
	}
	
	
	
}
