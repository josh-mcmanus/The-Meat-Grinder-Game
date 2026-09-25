using Godot;
using System;

public partial class Claw2 : Node2D
{
	
	[Export] RigidBody2D Top;
	[Export] RigidBody2D Middle;
	[Export] RigidBody2D Bottom;
	
	private float rotationSpeed = 20000f;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Set Rotations
		// Top
		float topAxis = Input.GetAxis("RotateRight", "RotateLeft");
		if (topAxis != 0)
		{
			Top.ApplyTorque(topAxis * rotationSpeed);
		}
		
		// Middle
		float middleAxis = Input.GetAxis("Right", "Left");
		if (middleAxis != 0)
		{
			Middle.ApplyTorque(middleAxis * rotationSpeed);
		}
		
		// Bottom
		float bottomAxis = Input.GetAxis("Up", "Down");
		if (bottomAxis != 0)
		{
			Bottom.ApplyTorque(bottomAxis * rotationSpeed);
		}
		
		// float rotationTopInput = Input.GetAxis("RotateLeft", "RotateRight");
		// float rotationMiddleInput = -Input.GetAxis("Left", "Right"); // is flipped for some reason, so negative
		// float rotationBottomInput = Input.GetAxis("Up", "Down");
	}
}
