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
	
	
	public override void _Ready()
	{
		LoadSettings();
		
		// Text submitted
		SensitivityLineEdit.TextSubmitted += text => OnTextSubmit(text, SensitivityLineEdit, SensitivitySlider, value => Utils.ValidateStringIsFloat(value), SliderType.Sensitivity);
		MaxDistanceLineEdit.TextSubmitted += text => OnTextSubmit(text, MaxDistanceLineEdit, MaxDistanceSlider, value => Utils.ValidateStringIsInt(value), SliderType.Distance);
		DeadzoneLineEdit.TextSubmitted += text => OnTextSubmit(text, DeadzoneLineEdit, DeadzoneSlider, value => Utils.ValidateStringIsInt(value), SliderType.Deadzone);
		
		// Text changed
		SensitivityLineEdit.TextChanged += text => Utils.SetLineEditText(text, SensitivityLineEdit.CaretColumn, SensitivityLineEdit);
		MaxDistanceLineEdit.TextChanged += text => Utils.SetLineEditText(text, MaxDistanceLineEdit.CaretColumn, MaxDistanceLineEdit);
		DeadzoneLineEdit.TextChanged += text => Utils.SetLineEditText(text, DeadzoneLineEdit.CaretColumn, DeadzoneLineEdit);
		
		// Slider moved
		SensitivitySlider.ValueChanged += value =>
		{
			UpdateLineEditAndSliderDecimal(value, SensitivityLineEdit, SensitivitySlider);
			Save(value, SliderType.Sensitivity);
		};
		MaxDistanceSlider.ValueChanged += value =>
		{
			UpdateLineEditAndSlider(value, MaxDistanceLineEdit, MaxDistanceSlider);
			Save(value, SliderType.Distance);
		};
		DeadzoneSlider.ValueChanged += value =>
		{
			UpdateLineEditAndSlider(value, DeadzoneLineEdit, DeadzoneSlider);
			Save(value, SliderType.Deadzone);
		};
	}
	
	
	private void OnTextSubmit<T>(string text, LineEdit textField, HSlider slider, Func<string, T> validateString, SliderType type) where T : notnull
	{
		T inputNum  = validateString(text);
		
		if (inputNum is float floatVal)
		{
            float result = Mathf.Clamp(floatVal, 0.00f, 1.00f);
            UpdateLineEditAndSliderDecimal(result, textField, slider);
			
			Save(result, type);
		}
		
        else if (inputNum is int intVal)
		{
			int result = type switch
			{
				SliderType.Distance => Mathf.Clamp(intVal, 50, 500),
				SliderType.Deadzone => Mathf.Clamp(intVal, 0, 50),
				_ => intVal
			};
			
            UpdateLineEditAndSlider(result, textField, slider);
			Save(result, type);
		}
		
		
	}
	
	private void UpdateLineEditAndSlider(double input, LineEdit textField, HSlider slider)
	{
		textField.Text = input.ToString("0");
		slider.Value = input;
	}
	
	private void UpdateLineEditAndSliderDecimal(double input, LineEdit textField, HSlider slider)
	{
		textField.Text = input.ToString("0.00");
		slider.Value = input;
	}
	
	private void Save(double input, SliderType type)
	{
		if (type == SliderType.Sensitivity)
		{
			DataManager.settingsData.MouseSensitivity = (float)input;
		}
		else if (type == SliderType.Distance)
		{
			DataManager.settingsData.MaxMouseDistance = (int)input;
		}
		else if (type == SliderType.Deadzone)
		{
			DataManager.settingsData.MouseDeadzone = (int)input;
		}
	}
	
	
	
	// Save ALL changes to settings file
	private void SaveButtonPressed()
	{
		DataManager.SaveSettingsData();
		
		GD.Print(DataManager.settingsData.MouseSensitivity);
		GD.Print(DataManager.settingsData.MaxMouseDistance);
		GD.Print(DataManager.settingsData.MouseDeadzone);
	}
	
	// Dont save any changes, load saved settings data
	// Cancel button press
	private void LoadSettings()
	{
		DataManager.LoadSettingsData();
		
		UpdateLineEditAndSliderDecimal(DataManager.settingsData.MouseSensitivity, SensitivityLineEdit, SensitivitySlider);
		UpdateLineEditAndSlider(DataManager.settingsData.MaxMouseDistance, MaxDistanceLineEdit, MaxDistanceSlider);
		UpdateLineEditAndSlider(DataManager.settingsData.MouseDeadzone, DeadzoneLineEdit, DeadzoneSlider);
	}
	
	
	enum SliderType
	{
		Sensitivity,
		Distance,
		Deadzone
	}
	
	
}
