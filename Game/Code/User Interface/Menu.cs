using Godot;
using System;
using System.Text;

public partial class Menu : Node
{
	
	
	// Check if the input text string from a line edit is a float
	public static float? ValidateStringIsFloat(string text)
	{
		if (float.TryParse(text, out float value))
		{
			return value;
		}
		return null;
	}
	
	
	public static float ClampFloat(float input, float min, float max)
	{
		return Mathf.Clamp(input, min, max);
	}
	
	
	
	public static TextValidationResult ValidateFloatText(string text, int oldCaretPosition)
	{
		StringBuilder filteredText = new();
		bool hasDecimal = false;
		int newCaretPosition = 0;
		
		
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
				filteredText.Append(c);
				
				if (i < oldCaretPosition)
				{
					newCaretPosition++;
				}
			}
		}
		
		return new TextValidationResult(filteredText.ToString(), newCaretPosition);
	}
	
	
	public static void SetLineEditText(string text, int oldCaretPos, LineEdit textField)
	{
        TextValidationResult result = ValidateFloatText(text, oldCaretPos);
		
		textField.Text = result.Text;
		textField.CaretColumn = result.CaretPosition;
	}
	
	
	public struct TextValidationResult
	{
		public string Text;
		public int CaretPosition;
		
		public TextValidationResult(string text, int caretPosition)
		{
			Text = text;
			CaretPosition = caretPosition;
		}
	}
	
	
	
	public static Vector2I GetPrimaryMonitorResolution()
	{
		int primaryScreen = DisplayServer.GetPrimaryScreen();
		return DisplayServer.ScreenGetSize(primaryScreen);
	}
	
	
}
