using Godot;
using System;
using System.Text.RegularExpressions;

public partial class DeadzoneMenu : Control
{
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
		// GD.Print(Input.GetConnectedJoypads());
		
		deadzoneLeft.SetDeadzone(DataManager.settingsData.DeadzoneLeft);
		deadzoneRight.SetDeadzone(DataManager.settingsData.DeadzoneRight);
		
		GD.Print("DeadzoneMenu ready");
	}
	
	
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
	private void ValidateFloatSubmission(string text, LineEdit textField, HSlider slider, Side side)
	{
		if (!float.TryParse(text, out float inputNum))
		{
			return;
		}
		
		inputNum = Mathf.Clamp(inputNum, 0.00f, 1.00f);
		
		UpdateLineEditAndSlider(inputNum, textField, slider, side);
	}
	
	
	// Validate that the input text is in the correct format
	// only in the format   x.xx   , where x is any digit, called at every text change
	// so   x   x.   x.x   are all valid
	private void ValidateTextInput(string text, int oldCaretPos, LineEdit textField)
	{
		string filteredText = "";
		bool hasDecimal = false;
		int newCaretPos = 0;
		
	
		for(int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			bool valid = false;
			
			if (char.IsNumber(c))
			{
				valid = true;
			}
			else if (c == '.' && !hasDecimal && i == 1)
			{
				valid = true;
				hasDecimal = true;
			}
			
			if (valid)
			{
				filteredText += c;

				if (i < oldCaretPos)
				{
					newCaretPos++;
				}
			}
		}
		
		textField.Text = filteredText;
		textField.CaretColumn = newCaretPos;
	}
	
	
	private void UpdateLineEditAndSlider(float input, LineEdit textField, HSlider slider, Side side)
	{
		textField.Text = input.ToString();
		slider.Value = input;
		
		if (side == Side.Left)
		{
			deadzoneLeft.SetDeadzone(input);
			DataManager.settingsData.DeadzoneLeft = input;
		}
		else
		{
			deadzoneRight.SetDeadzone(input);
			DataManager.settingsData.DeadzoneRight = input;
		}
	}
	
	
	
	private void OnDeadzoneTextSubmitLeft(string text)
	{
		ValidateFloatSubmission(text, deadzoneTextInputLeft, deadzoneSliderInputLeft, Side.Left);
	}
	
	private void OnDeadzoneTextSubmitRight(string text)
	{
		ValidateFloatSubmission(text, deadzoneTextInputRight, deadzoneSliderInputRight, Side.Right);
	}
	
	
	private void OnDeadzoneTextChangedLeft(string text)
	{
		ValidateTextInput(text, deadzoneTextInputLeft.CaretColumn, deadzoneTextInputLeft);
	}
	
	private void OnDeadzoneTextChangedRight(string text)
	{
		ValidateTextInput(text, deadzoneTextInputRight.CaretColumn, deadzoneTextInputRight);
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
		DataManager.SaveGameSettings();
	}
	
	// Cancel all settings and load settings file
	private void CancelButtonPressed()
	{
		DataManager.LoadGameSettings();
		
		UpdateLineEditAndSlider(DataManager.settingsData.DeadzoneLeft,  deadzoneTextInputLeft,  deadzoneSliderInputLeft,  Side.Left);
		UpdateLineEditAndSlider(DataManager.settingsData.DeadzoneRight, deadzoneTextInputRight, deadzoneSliderInputRight, Side.Right);
	}
	
	
	
	// Smoothly move the deadzone in and out
	private void GradientDeadzoneTest()
	{
		float deadzoneTest = Mathf.Lerp(0.05f, 0.95f, Mathf.PingPong(Time.GetTicksMsec() / 5000.0f, 1.0f));
		
		deadzoneLeft.SetDeadzone(deadzoneTest);
		deadzoneRight.SetDeadzone(deadzoneTest);
	}
	
	
	private enum Side
	{
		Left,
		Right
	}
	
	
}
