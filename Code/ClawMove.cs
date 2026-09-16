
using Godot;
using System;

public partial class ClawMove : Node2D
{
	
	[Export] public RigidBody2D Top;
	[Export] public RigidBody2D Middle;
	[Export] public RigidBody2D Bottom;
	[Export] public Marker2D IKTarget;
	
	[Export] public Bone2D TopBone;
	[Export] public Bone2D MiddleBone;
	[Export] public Bone2D BottomBone;
	
	
	
	private const float MoveSpeed = 300f;
	private const float IKTargetMaxDistance = 250f;
	
	private float relativeTopMass;
	private float relativeMiddleMass;
	private float relativeBottomMass;
	
	private Vector2 ikTargetOffset;
	
	
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
		
		ikTargetOffset = IKTarget.GlobalPosition - Top.GlobalPosition;
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		// Move claw
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		
		Vector2 movement = direction * MoveSpeed;
		
		Top.ApplyCentralForce(movement * relativeTopMass);
		Middle.ApplyCentralForce(movement * relativeMiddleMass);
		Bottom.ApplyCentralForce(movement * relativeBottomMass);
		
		
		// Move IK target
		Vector2 ikDirection = Input.GetVector(
			"ArrowLeft",
			"ArrowRight",
			"ArrowUp",
			"ArrowDown"
		);
		
		ikTargetOffset += ikDirection * MoveSpeed * (float)delta;
		
		
		// Constrain IK target to a fixed distance from the claw
		if (ikTargetOffset.Length() > IKTargetMaxDistance)
		{
			ikTargetOffset = ikTargetOffset.Normalized() * IKTargetMaxDistance;
		}
		
		
		// Position target relative to claw
		IKTarget.GlobalPosition = Top.GlobalPosition + ikTargetOffset;
		
		
		// Get IK solved rotations
		float topRotation = TopBone.GlobalRotation;
		float middleRotation = MiddleBone.GlobalRotation;
		float bottomRotation = BottomBone.GlobalRotation;
		
		
		// Apply IK rotations to physical claw
		Top.AngularVelocity = Mathf.AngleDifference(
			Top.GlobalRotation,
			topRotation
		) * 20f;
		
		Middle.AngularVelocity = Mathf.AngleDifference(
			Middle.GlobalRotation,
			middleRotation
		) * 20f;
		
		Bottom.AngularVelocity = Mathf.AngleDifference(
			Bottom.GlobalRotation,
			bottomRotation
		) * 20f;
	}
	

}
