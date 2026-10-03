using Godot;
using System;

public partial class VolumeMenu : Control
{
	private const string TAG = "VolumeMenu";
	
	
	[Export] LineEdit masterLineEdit;
	[Export] LineEdit musicLineEdit;
	[Export] LineEdit SFXLineEdit;
	[Export] LineEdit UILineEdit;
	
	[Export] HSlider masterSlider;
	[Export] HSlider musicSlider;
	[Export] HSlider SFXSlider;
	[Export] HSlider UISlider;
	
	[Export] AudioStreamPlayer musicPlayer;
	
	
	private VolumeControl[] volumeControls;
	
	
	
	public override void _Ready()
	{
		volumeControls = 
		[
			new(masterLineEdit, masterSlider, VolumeType.Master),
			new(musicLineEdit, musicSlider, VolumeType.Music),
			new(SFXLineEdit, SFXSlider, VolumeType.SFX),
			new(UILineEdit, UISlider, VolumeType.UI),
		];
		
		LoadAllVolumeData();
		
		musicPlayer.Play();
		
		Utils.Log(TAG, "Volume Menu Loaded");
		
		
		// Connect UI to functions
		masterLineEdit.TextSubmitted += text => OnVolumeTextSubmit(volumeControls[0], text);
		musicLineEdit.TextSubmitted += text => OnVolumeTextSubmit(volumeControls[1], text);
		SFXLineEdit.TextSubmitted += text => OnVolumeTextSubmit(volumeControls[2], text);
		UILineEdit.TextSubmitted += text => OnVolumeTextSubmit(volumeControls[3], text);
		
		masterLineEdit.TextChanged += text => Menu.SetLineEditText(text, masterLineEdit.CaretColumn, masterLineEdit);
		musicLineEdit.TextChanged += text => Menu.SetLineEditText(text, musicLineEdit.CaretColumn, musicLineEdit);
		SFXLineEdit.TextChanged += text => Menu.SetLineEditText(text, SFXLineEdit.CaretColumn, SFXLineEdit);
		UILineEdit.TextChanged += text => Menu.SetLineEditText(text, UILineEdit.CaretColumn, UILineEdit);
		
		masterSlider.ValueChanged += value => UpdateVolume(volumeControls[0], (float)value);
		musicSlider.ValueChanged += value => UpdateVolume(volumeControls[1], (float)value);
		SFXSlider.ValueChanged += value => UpdateVolume(volumeControls[2], (float)value);
		UISlider.ValueChanged += value => UpdateVolume(volumeControls[3], (float)value);
		
	}
	
	
	// Set UI elements, audio player volume, and save to settingsData
	private void UpdateVolume(VolumeControl control, float value)
	{
		control.LineEdit.Text = value.ToString("F2");
		control.Slider.Value = value;
		
		AudioServer.SetBusVolumeDb((int)control.Type, Mathf.LinearToDb(value));
		
		SetVolume(control.Type, value);
	}
	
	// Return volume for a specified type
	private float GetVolume(VolumeType type)
	{
		return type switch
		{
			VolumeType.Master => DataManager.settingsData.MasterVolume,
			VolumeType.Music => DataManager.settingsData.MusicVolume,
			VolumeType.SFX => DataManager.settingsData.SFXVolume,
			VolumeType.UI => DataManager.settingsData.UIVolume,
			_ => 1.0f
		};
	}
	
	// Save volume to settingsData for a specified type
	private void SetVolume(VolumeType type, float value)
	{
		switch (type)
		{
			case VolumeType.Master:
				DataManager.settingsData.MasterVolume = value;
				break;
			
			case VolumeType.Music:
				DataManager.settingsData.MusicVolume = value;
				break;
			
			case VolumeType.SFX:
				DataManager.settingsData.SFXVolume = value;
				break;
			
			case VolumeType.UI:
				DataManager.settingsData.UIVolume = value;
				break;
		}
	}
	
	
	// Run when line edit field data is submitted (Enter, Controller confirm, etc)
	// Updates volume if the submitted string is valid
	private void OnVolumeTextSubmit(VolumeControl control, string text)
	{
		float? inputNum  = Menu.ValidateStringIsFloat(text);
		
        if (inputNum is float value)
		{
            float result = Menu.ClampFloat(value, 0.00f, 1.00f);
			UpdateVolume(control, result);
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

		LoadAllVolumeData();
	}
	
	// Load and set all volume data from settings
	private void LoadAllVolumeData()
	{
		foreach (VolumeControl control in volumeControls)
		{
			UpdateVolume(control, GetVolume(control.Type));
		}
	}
	
	
	private struct VolumeControl(LineEdit lineEdit, HSlider slider, VolumeMenu.VolumeType type)
    {
		public LineEdit LineEdit = lineEdit;
		public HSlider Slider = slider;
		public VolumeType Type = type;
    }



    private enum VolumeType
	{
		Master,
		Music,
		SFX,
		UI
	}
	
	
	
}
