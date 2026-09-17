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
		float newLimitLower = CalculateAbsoluteAngleLimit(abovePart.GlobalRotation, lowerLimitRads);
		float newLimitUpper = CalculateAbsoluteAngleLimit(abovePart.GlobalRotation, upperLimitRads);

		// Offset the joints angular limit to the rotation of its above part, if it changed since last physics tick
		if (!EvaluateNumsRounded(joint.AngularLimitLower, newLimitLower, 2))
		{
			joint.AngularLimitLower = newLimitLower;
		}
		if (!EvaluateNumsRounded(joint.AngularLimitUpper, newLimitUpper, 2))
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
		
		float nextRotation = CalculateRotationNextPhysicsTick(AngularVelocity, rotationSpeed, inertia, dt, Rotation);

		// Next physics tick rotation would be out of bounds, so stop
		if (IsRotationOutsideLimits(nextRotation, newLimitLower, newLimitUpper))
		{
			return;
		}
		
		
		// Get and apply rotation to part
		float axis = Input.GetAxis("Right", "Left");
		if (axis != 0)
		{
			ApplyTorque(axis * rotationSpeed);
		}
		
	}
	
	
	// Angular limit relative to above part
	public static float CalculateAbsoluteAngleLimit(float aboveRotationRads, float relativeLimitRads)
	{
		return aboveRotationRads + relativeLimitRads;
	}
	
	
	// Both input angular limits are equal when rounded to n decimal places
	public static bool EvaluateNumsRounded(float oldAngularLimit, float newAngularLimit, int decimalPlaces)
	{
		return Math.Round(oldAngularLimit, decimalPlaces) == Math.Round(newAngularLimit, decimalPlaces);
	}
	
	// Angular velocity next physics tick, of the rigidbody node assigned this script
	public static float CaluclateAngularVelocityNextPhysicsTick(float angularVelocity, float torque, float inertia, float delta)
	{
		return angularVelocity + torque / inertia * delta;
	}
	
	// Rotation in next physics tick, of the rigidbody node assigned this script
	public static float CalculateRotationNextPhysicsTick(float angularVelocity, float rotationSpeed, float inertia, float delta, float rotation)
	{
		float nextAngularVelocity = CaluclateAngularVelocityNextPhysicsTick(angularVelocity, rotationSpeed, inertia, delta);
		
		return rotation + nextAngularVelocity * delta;
	}
	
	// Rotation is outside the bounds of the lower and upper limit
	public static bool IsRotationOutsideLimits(float rotation, float lowerLimit, float upperLimit)
	{
		return rotation <= lowerLimit || rotation >= upperLimit;
	}
	
	
}
