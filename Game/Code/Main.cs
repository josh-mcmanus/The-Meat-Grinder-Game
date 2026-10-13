using Godot;
using System;

public partial class Main : Node2D
{
	[Export] private MeatDetectionArea MeatDetectionArea;
	
	[Export] private ClawMove Claw;
	
	
	public override void _Ready()
	{
		// Utils.SummonSoftBody(this, SoftBodyData.SoftBodyTypes.MeatCube1, new(250,400));
		// Utils.SummonSoftBody(this, SoftBodyData.SoftBodyTypes.MeatCube1, new(500,400));
		// Utils.SummonSoftBody(this, SoftBodyData.SoftBodyTypes.MeatLong1, new(350,350));
		
		MeatDetectionArea.CollisionDetected += Test;
		
	}
	
	
	// TEST
    public override void _Input(InputEvent @event)
    {
		if (Input.IsActionJustPressed("TEST"))
		{
			// var global = GetGlobalMousePosition();
			// Vector2 pos = new(global.X + 100, global.Y + 100);
			
			Claw.StopClawForTest();
			
			
			GD.Print($"Mouse: {GetGlobalMousePosition()}");
			GD.Print($"Top position: {Claw.Top.GlobalPosition}");
			GD.Print($"Top velocity: {Claw.Top.LinearVelocity}");
			GD.Print($"Top angular velocity: {Claw.Top.AngularVelocity}");
			GD.Print($"Physics frames: {Engine.GetPhysicsFrames()}");

			Utils.SummonSoftBody(
				this,
				SoftBodyData.SoftBodyTypes.MeatCube1,
				new Vector2(500, 100)
			);
			
			// GD.Print(GetGlobalMousePosition());
			// Utils.SummonSoftBody(this, SoftBodyData.SoftBodyTypes.MeatCube1, new(500, 100));
		}
    }
	
	
	// Detecting if a soft body enters a collision area
	private void Test(Node2D node)
	{
		foreach (SoftBodyAttachment body in DataManager.softBodyData.activeSoftBodies)
		{
			int totalBonesAtInitialization = Utils.GetSoftBodyBones(body).Count;
			int currentBoneCount = body.boneList.Count;
			
			// GD.Print(body.Name);
			// GD.Print(totalBonesAtInitialization);
			// GD.Print(currentBoneCount);
		}
		
		// GD.Print("===================\n");
	}
	
	
}
