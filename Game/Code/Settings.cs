using Godot;
using System;

public partial class Settings : Node
{
	public static float deadzone = 0.1f;
	
	public static InputMode inputMode = InputMode.Mouse_and_Keyboard;
	
	public enum InputMode
	{
		Mouse_and_Keyboard,
		Controller
	}
}
