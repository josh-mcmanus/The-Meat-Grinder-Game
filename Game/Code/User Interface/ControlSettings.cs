using Godot;
using System;

public partial class ControlSettings : Control
{
	[Export] LineEdit SensitivityLineEdit;
	[Export] LineEdit MaxDistanceLineEdit;
	[Export] LineEdit DeadzoneLineEdit;
	
	[Export] HSlider  SensitivitySlider;
	[Export] HSlider  MaxDistanceSlider;
	[Export] HSlider  DeadzoneSlider;
	
	private Utils.SettingControl<Utils.MouseType> [] mouseControls;
	
	
	private const string SensitivityFormat = "0.00";
	private const float  SensitivityLowerBound = 0.01f;
	private const float  SensitivityUpperBound = 1;
	
	private const string DistanceFormat = "0";
	private const float  DistanceLowerBound = 100;
	private const float  DistanceUpperBound = 500;
	
	private const string DeadzoneFormat = "0";
	private const float  DeadzoneLowerBound = 0;
	private const float  DeadzoneUpperBound = 50;
	
	
	public override void _Ready()
	{
		mouseControls =
		[
			new(SensitivityLineEdit, SensitivitySlider, Utils.MouseType.Sensitivity, SensitivityFormat, SensitivityLowerBound, SensitivityUpperBound),
			new(MaxDistanceLineEdit, MaxDistanceSlider, Utils.MouseType.Distance,    DistanceFormat,    DistanceLowerBound,    DistanceUpperBound),
			new(DeadzoneLineEdit,    DeadzoneSlider,    Utils.MouseType.Deadzone,    DeadzoneFormat,    DeadzoneLowerBound,    DeadzoneUpperBound),
		];
		
		
		LoadAllSettingsData();
		
		// Text submitted
		SensitivityLineEdit.TextSubmitted += text => OnTextSubmit(mouseControls[0], text);
		MaxDistanceLineEdit.TextSubmitted += text => OnTextSubmit(mouseControls[1], text);
		DeadzoneLineEdit.TextSubmitted    += text => OnTextSubmit(mouseControls[2], text);
		
		// Text changed
		SensitivityLineEdit.TextChanged += text => Utils.SetLineEditText(text, SensitivityLineEdit.CaretColumn, SensitivityLineEdit, Utils.FormatType.Float);
		MaxDistanceLineEdit.TextChanged += text => Utils.SetLineEditText(text, MaxDistanceLineEdit.CaretColumn, MaxDistanceLineEdit, Utils.FormatType.Int);
		DeadzoneLineEdit.TextChanged    += text => Utils.SetLineEditText(text, DeadzoneLineEdit.CaretColumn,    DeadzoneLineEdit, Utils.FormatType.Int);
		
		// Slider moved
		SensitivitySlider.ValueChanged += value => UpdateSetting(mouseControls[0], (float)value);
		MaxDistanceSlider.ValueChanged += value => UpdateSetting(mouseControls[1], (float)value);
		DeadzoneSlider.ValueChanged    += value => UpdateSetting(mouseControls[2], (float)value);
	}
	
	
	// Run when line edit field data is submitted (Enter, Controller confirm, etc)
	// Updates mouse data if the submitted string is valid
	private void OnTextSubmit(Utils.SettingControl<Utils.MouseType> control, string text)
	{
		Utils.OnTextSubmit(control, text, UpdateSetting);
	}
	
	
	// Set UI elements, and save to settings data
	private void UpdateSetting(Utils.SettingControl<Utils.MouseType> control, float value)
	{
		float result = Utils.UpdateValues(control, value);
		SaveSetting(control.Type, result);
	}
	
	
	// Save Mouse settings for a specified type
	private void SaveSetting(Utils.MouseType type, float input)
	{
		switch (type)
		{
			case Utils.MouseType.Sensitivity:
				DataManager.settingsData.MouseSensitivity = input;
				break;
			
			case Utils.MouseType.Distance:
				DataManager.settingsData.MaxMouseDistance = (int)input;
				break;
			
			case Utils.MouseType.Deadzone:
				DataManager.settingsData.MouseDeadzone = (int)input;
				break;
		}
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
	
	// Load and set all mouse data from settings
	private void LoadAllSettingsData()
	{
		UpdateSetting(mouseControls[0], DataManager.settingsData.MouseSensitivity);
		UpdateSetting(mouseControls[1], DataManager.settingsData.MaxMouseDistance);
		UpdateSetting(mouseControls[2], DataManager.settingsData.MouseDeadzone);
	}
	
	
}
