using Godot;
using System;

public partial class Chain : RigidBody2D
{
	private const string TAG = "Chain";

	[Export] public RigidBody2D abovePart;
	[Export] public PinJoint2D  joint;
	
	[Export] public float lowerLimitDegrees = -30f; // Right side, NEGATIVE
	[Export] public float upperLimitDegrees =  30f; // Left side,  POSITIVE
	
	private float lowerLimitRads;
	private float upperLimitRads;
	
	private float rotationSpeed = 20000f;
	
	
	public override void _Ready()
	{
		lowerLimitRads = Mathf.DegToRad(lowerLimitDegrees);
		upperLimitRads = Mathf.DegToRad(upperLimitDegrees);
		
		joint.AngularLimitEnabled = true;
		// joint.MotorEnabled = true;
		// joint.MotorTargetVelocity = 2f;
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		float newLimitLower = Utils.CalculateAngleLimit(abovePart.GlobalRotation, lowerLimitRads);
		float newLimitUpper = Utils.CalculateAngleLimit(abovePart.GlobalRotation, upperLimitRads);
		
		// Offset the joints angular limit to the rotation of its above part, if it changed since last physics tick
		if (Math.Round(joint.AngularLimitLower, 2) != Math.Round(newLimitLower, 2))
		{
			joint.AngularLimitLower = newLimitLower;
		}
		if (Math.Round(joint.AngularLimitUpper, 2) != Math.Round(newLimitUpper, 2))
		{
			joint.AngularLimitUpper = newLimitUpper;
		}
		
		
		// Next physics tick rotation calculation
		float inertia = (float)PhysicsServer2D.BodyGetParam(GetRid() ,PhysicsServer2D.BodyParameter.Inertia);
		
		if (inertia <= 0)
		{
			Utils.Log(TAG, "Inertia is 0, cannot continue physics this tick!");
			return;
		}
		
		float nextAngularVelocity = AngularVelocity + rotationSpeed / inertia * dt;
		float nextRotation = Rotation + nextAngularVelocity * dt;
		
		
		// Next physics tick rotation would be out of bounds, so stop
		if (nextRotation <= newLimitLower || nextRotation >= newLimitUpper)
		{
			// Utils.Log(TAG, "Next physics tick is out of bounds, stopping torque");
			return;
		}
		
		
		// Get and apply rotation to part
		float axis = Input.GetAxis("Right", "Left");
		if (axis != 0)
		{
			ApplyTorque(axis * rotationSpeed);
		}
		
	}
}
