using Godot;
using System;

public partial class VolumeMenu : Control
{
	private const string TAG = "VolumeMenu";
	
	
	[Export] LineEdit masterInput;
	[Export] LineEdit musicInput;
	[Export] LineEdit soundEffectInput;
	[Export] LineEdit userInterfaceInput;
	
	[Export] HSlider masterSlider;
	[Export] HSlider musicSlider;
	[Export] HSlider soundEffectSlider;
	[Export] HSlider userInterfaceSlider;
	
	
	public override void _Ready()
	{
		masterInput.Text = DataManager.settingsData.MasterVolume.ToString("F2");
		musicInput.Text = DataManager.settingsData.MusicVolume.ToString("F2");
		soundEffectInput.Text = DataManager.settingsData.SFXVolume.ToString("F2");
		userInterfaceInput.Text = DataManager.settingsData.UIVolume.ToString("F2");
		
		masterSlider.Value = DataManager.settingsData.MasterVolume;
		musicSlider.Value = DataManager.settingsData.MusicVolume;
		soundEffectSlider.Value = DataManager.settingsData.SFXVolume;
		userInterfaceSlider.Value = DataManager.settingsData.UIVolume;
		
		Utils.Log(TAG, "Volume Menu Loaded");
	}
	
	
	
	private void OnVolumeTextSubmit(string text, LineEdit textField, HSlider slider, VolumeType type)
	{
		float? inputNum  = Menu.ValidateStringIsFloat(text);
		
        if (inputNum is float value)
		{
            float result = Menu.ClampFloat(value, 0.00f, 1.00f);
            UpdateLineEditAndSlider(result, textField, slider, type);
		}
	}
	
	
	private void UpdateLineEditAndSlider(float input, LineEdit textField, HSlider slider, VolumeType type)
	{
		textField.Text = input.ToString("F2");
		slider.Value = input;
		
		switch (type)
		{
			case VolumeType.Master:
				DataManager.settingsData.MasterVolume = input;
				break;
			
			case VolumeType.Music:
				DataManager.settingsData.MasterVolume = input;
				break;
			
			case VolumeType.SFX:
				DataManager.settingsData.MasterVolume = input;
				break;
			
			case VolumeType.UI:
				DataManager.settingsData.MasterVolume = input;
				break;
		}
	}
	
	
	
	private void OnLineEditMasterSubmit(string text)
	{
		OnVolumeTextSubmit(text, masterInput, masterSlider, VolumeType.Master);
	}
	
	private void OnLineEditMusicSubmit(string text)
	{
		OnVolumeTextSubmit(text, musicInput, musicSlider, VolumeType.Music);
	}
	
	private void OnLineEditSFXSubmit(string text)
	{
		OnVolumeTextSubmit(text, soundEffectInput, soundEffectSlider, VolumeType.SFX);
	}
	
	private void OnLineEditUISubmit(string text)
	{
		OnVolumeTextSubmit(text, userInterfaceInput, userInterfaceSlider, VolumeType.UI);
	}
	
	
	private void OnLineEditMasterChanged(string text)
	{
		Menu.SetLineEditText(text, masterInput.CaretColumn, masterInput);
	}
	
	private void OnLineEditMusicChanged(string text)
	{
		Menu.SetLineEditText(text, musicInput.CaretColumn, musicInput);
	}
	
	private void OnLineEditSFXChanged(string text)
	{
		Menu.SetLineEditText(text, soundEffectInput.CaretColumn, soundEffectInput);
	}
	
	private void OnLineEditUIChanged(string text)
	{
		Menu.SetLineEditText(text, userInterfaceInput.CaretColumn, userInterfaceInput);
	}
	
	
	private void OnSliderMasterChanged(float value)
	{
		UpdateLineEditAndSlider(value, masterInput, masterSlider, VolumeType.Master);
	}
	
	private void OnSliderMusicChanged(float value)
	{
		UpdateLineEditAndSlider(value, musicInput, musicSlider, VolumeType.Music);
	}
	
	private void OnSliderSFXChanged(float value)
	{
		UpdateLineEditAndSlider(value, soundEffectInput, soundEffectSlider, VolumeType.SFX);
	}
	
	private void OnSliderUIChanged(float value)
	{
		UpdateLineEditAndSlider(value, userInterfaceInput, userInterfaceSlider, VolumeType.UI);
	}
	
	
	
	// Save all changes to settings file
	private void SaveButtonPressed()
	{
		DataManager.SaveGameSettings();
	}
	
	// Cancel all settings and load settings file
	private void CancelButtonPressed()
	{
		DataManager.LoadGameSettings();
		
		UpdateLineEditAndSlider(DataManager.settingsData.MasterVolume, masterInput, masterSlider, VolumeType.Master);
		UpdateLineEditAndSlider(DataManager.settingsData.MusicVolume, musicInput, musicSlider, VolumeType.Music);
		UpdateLineEditAndSlider(DataManager.settingsData.SFXVolume, soundEffectInput, soundEffectSlider, VolumeType.SFX);
		UpdateLineEditAndSlider(DataManager.settingsData.UIVolume, userInterfaceInput, userInterfaceSlider, VolumeType.UI);
	}
	
	
	
	
	private enum VolumeType
	{
		Master,
		Music,
		SFX,
		UI
	}
	
	
	
}
