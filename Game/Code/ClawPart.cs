using Godot;
using System;

public partial class ClawPart : RigidBody2D
{
	[Export] public float MinRelativeRotation = -15f; // For Middle and Bottom, this is how much to the RIGHT it will move
	[Export] public float MaxRelativeRotation =  15f;
	
	[Export] public RigidBody2D HigherPart;
	
	
	private float minRelativeRotationRads;
	private float maxRelativeRotationRads;
	
	
    public override void _Ready()
    {
        minRelativeRotationRads = Mathf.DegToRad(MinRelativeRotation);
        maxRelativeRotationRads = Mathf.DegToRad(MaxRelativeRotation);
    }
	
	
	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		float rotation = state.Transform.Rotation;
		float higherRotation = 0f;
		
		if (HigherPart != null)
		{
			higherRotation = HigherPart.GlobalRotation;
		}
		
		float relativeRotation = Mathf.AngleDifference(higherRotation, rotation);
		
		float min = higherRotation + minRelativeRotationRads;
		float max = higherRotation + maxRelativeRotationRads;
		
		// Limit rotation
		if (relativeRotation < minRelativeRotationRads)
		{
			rotation = min;
			if (state.AngularVelocity < 0) state.AngularVelocity = 0;
		}
		else if (relativeRotation > maxRelativeRotationRads)
		{
			rotation = max;
			if (state.AngularVelocity > 0) state.AngularVelocity = 0;
		}
		
		// Set part rotation
		state.Transform = new Transform2D(rotation, state.Transform.Origin);
	}
	
}
