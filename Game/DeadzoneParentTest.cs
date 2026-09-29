using Godot;
using System;

public partial class DeadzoneParentTest : Control
{
	[Export] DeadzoneTest deadzoneLeft;
	[Export] DeadzoneTest deadzoneRight;
	
	private Vector2 prevLeft = Vector2.Zero;
	private Vector2 prevRight = Vector2.Zero;
	
	
	public override void _Ready()
	{
		// GD.Print(Input.GetConnectedJoypads());
		
		deadzoneLeft.SetDeadzone(DataManager.settingsData.DeadzoneLeft);
		deadzoneRight.SetDeadzone(DataManager.settingsData.DeadzoneRight);
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
	
	
	// Smoothly move the deadzone in and out
	private void GradientDeadzoneTest()
	{
		float deadzoneTest = Mathf.Lerp(0.05f, 0.95f, Mathf.PingPong(Time.GetTicksMsec() / 5000.0f, 1.0f));
		
		deadzoneLeft.SetDeadzone(deadzoneTest);
		deadzoneRight.SetDeadzone(deadzoneTest);
	}
}
