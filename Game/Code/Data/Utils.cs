using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable CS0162 // Unreachable code detected


public partial class Utils
{
	// Debug mode, prints out debug information
	public const bool DEBUG = true;
	
	
	public static void Log(string TAG, object msg)
	{
        if (!DEBUG) return;
        GD.Print($"[{TAG}] {msg}");
	}
	
	public static void Err(string TAG, object msg)
	{
		if (!DEBUG) return;
        GD.PrintErr($"[{TAG}] {msg}");
	}
	
	
	
	// ===================== //
	// Text input validation //
	// ===================== //
	
	// Check if the input string is a float
	public static float? ValidateStringIsFloat(string text)
	{
		if (float.TryParse(text, out float value))
		{
			return value;
		}
		return null;
	}
	
	// Check if the input string is in the correct format
	// Float: "x.xx", Int: "xxxx"
	public static TextValidationResult ValidateStringIsFormattedNum(string text, int oldCaretPosition, FormatType type)
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
			else if (type == FormatType.Float && c == '.' && !hasDecimal && i == 1)
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
	
	// Update the input line edit to the correct format for the input string and text caret position
	public static void SetLineEditText(string text, int oldCaretPos, LineEdit textField, FormatType type)
	{
        TextValidationResult result = ValidateStringIsFormattedNum(text, oldCaretPos, type);
		
		textField.Text = result.Text;
		textField.CaretColumn = result.CaretPosition;
	}
	
	
	// Run when line edit field data is submitted (Enter, Controller confirm, etc)
	// Updates mouse data if the submitted string is valid
	public static void OnTextSubmit<T>(T control, string text, Action<T, float> UpdateSettings)
	{
		float? inputNum  = ValidateStringIsFloat(text);

		if (inputNum is float value)
		{
			UpdateSettings(control, value);
		}
	}
	
	
	public static float UpdateValues<T>(SettingControl<T> control, float value)
	{
		float result = Mathf.Clamp(value, control.LowerBound, control.UpperBound);
		
		control.LineEdit.Text = result.ToString(control.Format);
		control.Slider.Value = result;
		
		return result;
	}
	
	
	
	public struct TextValidationResult(string text, int caretPosition)
    {
		public string Text = text;
		public int CaretPosition = caretPosition;
    }
	
    public struct SettingControl<T>(LineEdit lineEdit, HSlider slider, T type, string format, float lowerBound, float upperBound)
    {
		public LineEdit LineEdit = lineEdit;
		public HSlider Slider = slider;
		public T Type = type;
		public string Format = format;
		public float LowerBound = lowerBound;
		public float UpperBound = upperBound;
    }
	
	
	
	public enum FormatType
	{
		Float,
		Int
	}
	
	public enum VolumeType
	{
		Master,
		Music,
		SFX,
		UI
	}
	
	public enum MouseType
	{
		Sensitivity,
		Distance,
		Deadzone
	}
	
	public enum DeadzoneSide
	{
		Left,
		Right
	}
	
	
	
	// ========== //
	// Resolution //
	// ========== //
	
	public static Vector2I GetPrimaryMonitorResolution()
	{
		int primaryScreen = DisplayServer.GetPrimaryScreen();
		return DisplayServer.ScreenGetSize(primaryScreen);
	}
	
	
	
	// ===== //
	// Mouse //
	// ===== //
	
	public static void CaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	public static void UncaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
	
	
	
	// ========= //
	// Collision //
	// ========= //
	
	public static List<PhysicsBody2D> GetSoftBodyBones(Node2D node)
	{
		List<PhysicsBody2D> boneList = [];
		
		// foreach (Node part in node.GetChildren())
		// {
		// 	GD.Print($"Name: {part.Name}");
		// 	GD.Print($"Type: {part.GetType()}");
		// 	GD.Print($"Parent: {part.GetParent().Name}");
		// 	GD.Print("---");
		// }
		
		var childBones = node.FindChildren("*", type:"RigidBody2D");

		foreach (var bone in childBones)
		{
			if (bone is PhysicsBody2D body)
			{
				boneList.Add(body);
			}
			
			
			
			// GD.Print(bone.Name);
			// GD.Print(bone.GetType());
		}
		
		// foreach (var part in node.GetChildren())
		// {
		// 	string name = part.Name;
		// 	// GD.Print(name);
		// 	// GD.Print(name.Contains("Bone"));
		// 	// GD.Print(part.GetType() == typeof(PhysicsBody2D));
		// 	// GD.Print(part is PhysicsBody2D);
		// 	var test = part.GetType();
		// 	// GD.Print(test);
			
		// 	if (part is PhysicsBody2D bone && name.Contains("Bone"))
		// 	{
		// 		boneList.Add(bone);
		// 		// GD.Print(bone.Name);
		// 	}
		// }
		
		return [.. boneList];
	}
	
}
