using Godot;
using System;

public partial class Chain : RigidBody2D
{
	[Export] public RigidBody2D abovePart;
	[Export] public PinJoint2D  joint;
	
	[Export] public float lowerLimitDegrees = -30f;
	[Export] public float upperLimitDegrees =  30f;
	
	private float lowerLimitRads;
	private float upperLimitRads;
	
	private float rotationSpeed = 10000f;
	
	
	public override void _Ready()
	{
		lowerLimitRads = Mathf.DegToRad(lowerLimitDegrees);
		upperLimitRads = Mathf.DegToRad(upperLimitDegrees);
	}
	
	
	public override void _Process(double delta)
	{		
		float axis = Input.GetAxis("Right", "Left");
		
		ApplyTorque(axis * rotationSpeed);
		
		// Offset by above part rotation
		joint.AngularLimitLower = abovePart.Rotation + lowerLimitRads;
		joint.AngularLimitUpper = abovePart.Rotation + upperLimitRads;
		
	}
}
