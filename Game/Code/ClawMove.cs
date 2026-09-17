using Godot;
using System;

public partial class ClawMove : Node2D
{
	
	[Export] public RigidBody2D Top;
	[Export] public RigidBody2D Middle;
	[Export] public RigidBody2D Bottom;
	
	
	
	private const float MoveSpeed = 300f;
	private const float RotationSpeedDegrees = 45f;
	
	private float rotationSpeedRads;
	private float relativeTopMass;
	private float relativeMiddleMass;
	private float relativeBottomMass;
	
	
	public override void _Ready()
	{
		// Set phsyics properties to prevent tunneling on soft bodies
		Top.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Middle.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Bottom.ContinuousCd = RigidBody2D.CcdMode.CastShape;
		
		Top.ContactMonitor = true;
		Middle.ContactMonitor = true;
		Bottom.ContactMonitor = true;
		
		Top.MaxContactsReported = 4;
		Middle.MaxContactsReported = 4;
		Bottom.MaxContactsReported = 4;
		
		relativeTopMass = Top.Mass + Middle.Mass + Bottom.Mass;
		relativeMiddleMass = Middle.Mass + Bottom.Mass;
		relativeBottomMass = Bottom.Mass;

		rotationSpeedRads = Mathf.DegToRad(RotationSpeedDegrees);
	}
	
	
	
	public override void _PhysicsProcess(double delta)
	{
		// Move
		Vector2 direction = Input.GetVector("ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown");
		// Top.LinearVelocity = direction * MoveSpeed;
		Top.ApplyImpulse(direction * MoveSpeed * relativeTopMass *  0.02f);
		Middle.ApplyImpulse(direction * MoveSpeed * relativeMiddleMass * 0.02f);
		Bottom.ApplyImpulse(direction * MoveSpeed * relativeBottomMass * 0.02f);
		
		// Calculate rotation
		float rotationTopInput = Input.GetAxis("RotateLeft", "RotateRight");
		float rotationMiddleInput = -Input.GetAxis("Left", "Right"); // is flipped for some reason, so negative
		float rotationBottomInput = Input.GetAxis("Up", "Down");
		
		// Apply Rotation
		Top.AngularVelocity = rotationTopInput * rotationSpeedRads * 3;
		Middle.AngularVelocity = rotationMiddleInput * rotationSpeedRads * relativeMiddleMass;
		Bottom.AngularVelocity = rotationBottomInput * rotationSpeedRads * relativeBottomMass;
		
		// Top.ApplyTorqueImpulse(rotationTopInput * rotationSpeedRads * 3);
		// Middle.ApplyTorqueImpulse(rotationMiddleInput * rotationSpeedRads * relativeMiddleMass);
		// Bottom.ApplyTorqueImpulse(rotationBottomInput * rotationSpeedRads * relativeBottomMass);
	}
	

}
