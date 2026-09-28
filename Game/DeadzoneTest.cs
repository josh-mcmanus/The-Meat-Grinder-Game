using Godot;
using System;

public partial class DeadzoneTest : Control
{
	private float axis = 0;
	private Vector2 prevRight = Vector2.Zero;
	private Vector2 prevLeft = Vector2.Zero;
	
	
	
	
	
	public override void _Ready()
	{
		GD.Print(Input.GetConnectedJoypads());
	}
	
	
	
	public override void _Process(double delta)
	{
		float LX = Input.GetJoyAxis(0, JoyAxis.LeftX);
		float LY = Input.GetJoyAxis(0, JoyAxis.LeftY);
		Vector2 leftStickAxis = new(LX, LY);
		
		float RX = Input.GetJoyAxis(0, JoyAxis.RightX);
		float RY = Input.GetJoyAxis(0, JoyAxis.RightY);
		Vector2 rightStickAxis = new(RX, RY);
		
		if (leftStickAxis != prevLeft)
		{
			GD.Print("LEFT: " + leftStickAxis);
		}
		if (rightStickAxis != prevRight)
		{
			GD.Print("RIGHT: " + rightStickAxis);
		}
		
		prevLeft  = leftStickAxis;
		prevRight = rightStickAxis;
	}
	
}
