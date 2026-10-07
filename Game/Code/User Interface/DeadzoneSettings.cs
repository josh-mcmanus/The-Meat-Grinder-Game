using Godot;
using System;
using System.Text.RegularExpressions;

public partial class DeadzoneSettings : Control
{
	private const string TAG = "DeadzoneSettings";
	
	[Export] Deadzone DeadzoneLeft;
	[Export] Deadzone DeadzoneRight;
	
	[Export] LineEdit DeadzoneLineEditLeft;
	[Export] LineEdit DeadzoneLineEditRight;
	
	[Export] HSlider  deadzoneSliderLeft;
	[Export] HSlider  deadzoneSliderRight;
	
	private Utils.SettingControl<Utils.DeadzoneSide> [] deadzoneControls;
	
	private Vector2 prevLeft = Vector2.Zero;
	private Vector2 prevRight = Vector2.Zero;
	
	private const string Format = "0.00";
	private const float  LowerBound = 0;
	private const float  UpperBound = 1;
	
	
	public override void _Ready()
	{
		deadzoneControls =
		[
			new(DeadzoneLineEditLeft,  deadzoneSliderLeft,  Utils.DeadzoneSide.Left,  Format, LowerBound, UpperBound),
			new(DeadzoneLineEditRight, deadzoneSliderRight, Utils.DeadzoneSide.Right, Format, LowerBound, UpperBound),
		];
		
		LoadAllSettingsData();
		
		DeadzoneLineEditLeft.TextSubmitted  += text => OnTextSubmit(deadzoneControls[0], text);
		DeadzoneLineEditRight.TextSubmitted += text => OnTextSubmit(deadzoneControls[1], text);
		
		DeadzoneLineEditLeft.TextChanged  += text => Utils.SetLineEditText(text, DeadzoneLineEditLeft.CaretColumn, DeadzoneLineEditLeft, Utils.FormatType.Float);
		DeadzoneLineEditRight.TextChanged += text => Utils.SetLineEditText(text, DeadzoneLineEditRight.CaretColumn, DeadzoneLineEditRight, Utils.FormatType.Float);
		
		deadzoneSliderLeft.ValueChanged  += value => UpdateSetting(deadzoneControls[0], (float)value);
		deadzoneSliderRight.ValueChanged += value => UpdateSetting(deadzoneControls[1], (float)value);
	}
	
	
	// Get and set analog stick position on deadzone ui
	public override void _Process(double delta)
	{
		// GradientDeadzoneTest();
		
		Vector2 leftStickAxis = new(
			Input.GetJoyAxis(0, JoyAxis.LeftX), 
			Input.GetJoyAxis(0, JoyAxis.LeftY)
		);
		
		Vector2 rightStickAxis = new(
			Input.GetJoyAxis(0, JoyAxis.RightX), 
			Input.GetJoyAxis(0, JoyAxis.RightY)
		);
		
		
		if (leftStickAxis != prevLeft || leftStickAxis.Length() > DataManager.settingsData.DeadzoneLeft)
		{
			DeadzoneLeft.SetCursorPosition(leftStickAxis);
		}
		if (rightStickAxis != prevRight || rightStickAxis.Length() > DataManager.settingsData.DeadzoneRight)
		{
			DeadzoneRight.SetCursorPosition(rightStickAxis);
		}
		
		
		prevLeft  = leftStickAxis;
		prevRight = rightStickAxis;
	}
	
	
	
	// Run when line edit field data is submitted (Enter, Controller confirm, etc)
	// Update deadzone data if the submitted string is valid
	private void OnTextSubmit(Utils.SettingControl<Utils.DeadzoneSide> control, string text)
	{
		Utils.OnTextSubmit(control, text, UpdateSetting);
	}
	
	
	// Set UI elements, and save to settings data
	private void UpdateSetting(Utils.SettingControl<Utils.DeadzoneSide> control, float value)
	{
		float result = Utils.UpdateValues(control, value);
		SaveSetting(control.Type, result);
	}
	
	
	// Save Deadzone settings for a specified side
	// Sets the size of the deadzone circle on the UI, of the specified side
	private void SaveSetting(Utils.DeadzoneSide type, double input)
	{
		switch (type)
		{
			case Utils.DeadzoneSide.Left:
				DeadzoneLeft.SetDeadzoneUI((float)input);
				DataManager.settingsData.DeadzoneLeft = (float)input;
				break;
			
			case Utils.DeadzoneSide.Right:
				DeadzoneRight.SetDeadzoneUI((float)input);
				DataManager.settingsData.DeadzoneRight = (float)input;
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
	
	// Load and set all volume data from settings
	private void LoadAllSettingsData()
	{
		DeadzoneLeft.SetDeadzoneUI(DataManager.settingsData.DeadzoneLeft);
		DeadzoneRight.SetDeadzoneUI(DataManager.settingsData.DeadzoneRight);
		
		UpdateSetting(deadzoneControls[0], DataManager.settingsData.DeadzoneLeft);
		UpdateSetting(deadzoneControls[1], DataManager.settingsData.DeadzoneRight);
		
		
		// DeadzoneLeft.SetDeadzoneUI(DataManager.settingsData.DeadzoneLeft);
		// DeadzoneRight.SetDeadzoneUI(DataManager.settingsData.DeadzoneRight);
		
		// deadzoneSliderLeft.Value  = DataManager.settingsData.DeadzoneLeft;
		// deadzoneSliderRight.Value  = DataManager.settingsData.DeadzoneRight;
		
		// DeadzoneLineEditLeft.Text = DataManager.settingsData.DeadzoneLeft.ToString(deadzoneControls[0].Format);
		// DeadzoneLineEditRight.Text = DataManager.settingsData.DeadzoneRight.ToString(deadzoneControls[1].Format);
	}
	
	
	
	
	// Smoothly move the deadzone in and out
	private void GradientDeadzoneTest()
	{
		float deadzoneTest = Mathf.Lerp(0.05f, 0.95f, Mathf.PingPong(Time.GetTicksMsec() / 5000.0f, 1.0f));
		
		DeadzoneLeft.SetDeadzoneUI(deadzoneTest);
		DeadzoneRight.SetDeadzoneUI(deadzoneTest);
	}
	
	
}
