using Godot;
using System;
using System.Text.RegularExpressions;

public partial class DeadzoneSettings : Control
{
	private const string TAG = "DeadzoneSettings";
	
	[Export] Deadzone deadzoneLeft;
	[Export] Deadzone deadzoneRight;
	
	[Export] LineEdit deadzoneTextInputLeft;
	[Export] HSlider  deadzoneSliderInputLeft;
	
	[Export] LineEdit deadzoneTextInputRight;
	[Export] HSlider  deadzoneSliderInputRight;
	
	
	
	private Vector2 prevLeft = Vector2.Zero;
	private Vector2 prevRight = Vector2.Zero;
	
	
	public override void _Ready()
	{
		// Load saved deadzones for UI
		deadzoneLeft.SetDeadzoneUI(DataManager.settingsData.DeadzoneLeft);
		deadzoneTextInputLeft.Text = DataManager.settingsData.DeadzoneLeft.ToString("F2");
		deadzoneSliderInputLeft.Value = DataManager.settingsData.DeadzoneLeft;
		
		deadzoneRight.SetDeadzoneUI(DataManager.settingsData.DeadzoneRight);
		deadzoneTextInputRight.Text = DataManager.settingsData.DeadzoneRight.ToString("F2");
		deadzoneSliderInputRight.Value = DataManager.settingsData.DeadzoneRight;
		
		Utils.Log(TAG, "Deadzone Menu Loaded");
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
			deadzoneLeft.SetCursorPosition(leftStickAxis);
		}
		if (rightStickAxis != prevRight || rightStickAxis.Length() > DataManager.settingsData.DeadzoneRight)
		{
			deadzoneRight.SetCursorPosition(rightStickAxis);
		}
		
		
		prevLeft  = leftStickAxis;
		prevRight = rightStickAxis;
	}
	
	
	
	// Validate if the text is a valid float
	// Update UI elements and deadzone setting
	private void OnDeadzoneTextSubmit(string text, LineEdit textField, HSlider slider, Side side)
	{
		float? inputNum  = Utils.ValidateStringIsFloat(text);
		
        if (inputNum is float value)
		{
            float result = Utils.ClampFloat(value, 0.00f, 1.00f);
            UpdateLineEditAndSlider(result, textField, slider, side);
		}
	}
	
	
	
	// Updates the line edit field and slider to be the input float
	// Sets the deadzone (Datamanager.settingsData not save file) of the corresponding side to the input
	private void UpdateLineEditAndSlider(float input, LineEdit textField, HSlider slider, Side side)
	{
		textField.Text = input.ToString("F2");
		slider.Value = input;
		
		if (side == Side.Left)
		{
			deadzoneLeft.SetDeadzoneUI(input);
			DataManager.settingsData.DeadzoneLeft = input;
		}
		else
		{
			deadzoneRight.SetDeadzoneUI(input);
			DataManager.settingsData.DeadzoneRight = input;
		}
	}
	
	
	
	
	private void OnDeadzoneTextSubmitLeft(string text)
	{
		OnDeadzoneTextSubmit(text, deadzoneTextInputLeft, deadzoneSliderInputLeft, Side.Left);
	}
	
	private void OnDeadzoneTextSubmitRight(string text)
	{
		OnDeadzoneTextSubmit(text, deadzoneTextInputRight, deadzoneSliderInputRight, Side.Right);
	}
	
	
	private void OnDeadzoneTextChangedLeft(string text)
	{
		Utils.SetLineEditText(text, deadzoneTextInputLeft.CaretColumn, deadzoneTextInputLeft);
	}
	
	private void OnDeadzoneTextChangedRight(string text)
	{
		Utils.SetLineEditText(text, deadzoneTextInputRight.CaretColumn, deadzoneTextInputRight);
	}
	
	
	private void OnSliderValueChangedLeft(float value)
	{
		UpdateLineEditAndSlider(value, deadzoneTextInputLeft, deadzoneSliderInputLeft, Side.Left);
	}
	
	private void OnSliderValueChangedRight(float value)
	{
		UpdateLineEditAndSlider(value, deadzoneTextInputRight, deadzoneSliderInputRight, Side.Right);
		
	}
	
	
	
	// Save all changes to settings file
	private void SaveButtonPressed()
	{
		DataManager.SaveGameData();
	}
	
	// Cancel all settings and load settings file
	private void CancelButtonPressed()
	{
		DataManager.LoadSettingsData();
		
		UpdateLineEditAndSlider(DataManager.settingsData.DeadzoneLeft,  deadzoneTextInputLeft,  deadzoneSliderInputLeft,  Side.Left);
		UpdateLineEditAndSlider(DataManager.settingsData.DeadzoneRight, deadzoneTextInputRight, deadzoneSliderInputRight, Side.Right);
	}
	
	
	
	// Smoothly move the deadzone in and out
	private void GradientDeadzoneTest()
	{
		float deadzoneTest = Mathf.Lerp(0.05f, 0.95f, Mathf.PingPong(Time.GetTicksMsec() / 5000.0f, 1.0f));
		
		deadzoneLeft.SetDeadzoneUI(deadzoneTest);
		deadzoneRight.SetDeadzoneUI(deadzoneTest);
	}
	
	
	private enum Side
	{
		Left,
		Right
	}
	
	
}
