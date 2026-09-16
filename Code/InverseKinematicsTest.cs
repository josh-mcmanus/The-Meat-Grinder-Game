using Godot;
using System;

public partial class InverseKinematicsTest : Node2D
{
	[Export] Marker2D IKTarget;
	
	[Export] public RigidBody2D Top;
	[Export] public RigidBody2D Middle;
	[Export] public RigidBody2D Bottom;
	
	private Vector2 lastLocation = Vector2.Zero;
	
	[Export] private float IKTargetSmoothness = 10f;
	[Export] private float MoveForce = 300f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Top.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Middle.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Bottom.ContinuousCd = RigidBody2D.CcdMode.CastShape;
		
		Top.ContactMonitor = true;
		Middle.ContactMonitor = true;
		Bottom.ContactMonitor = true;
		
		Top.MaxContactsReported = 4;
		Middle.MaxContactsReported = 4;
		Bottom.MaxContactsReported = 4;
	}

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = Input.GetVector("ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown");
		
		Top.ApplyCentralForce(direction * MoveForce);
		Middle.ApplyCentralForce(direction * MoveForce);
		Bottom.ApplyCentralForce(direction * MoveForce);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

		Vector2 IKTargetPosition = GetGlobalMousePosition();
		
		IKTarget.GlobalPosition = IKTargetPosition;
		
		// GD.Print(IKTarget.GlobalPosition );
		
		// GD.Print(IKTargetPosition);
		// float weight = 1.0f - Mathf.Exp(
    	//     -IKTargetSmoothness * (float)delta
    	// );
		
    	// IKTarget.GlobalPosition = IKTarget.GlobalPosition.Lerp(
    	//     IKTarget.GlobalPosition * IKTargetPosition,
    	//     weight
    	// );
	}
	
}
