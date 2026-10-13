using Godot;
using System;

public partial class Main : Node2D
{
	[Export] private MeatDetectionArea MeatDetectionArea;
	
	// TEMP for testing collision area works
	[Export] private SoftBodyAttachment MeatCube;
	
	
	
	public override void _Ready()
	{
		MeatDetectionArea.CollisionDetected += Test;
	}

    public override void _PhysicsProcess(double delta)
    {
		
    }
	
	
	// Detecting if a soft body enters a collision area
	// WILL CHANGE TO SOMETHING LATER, JUST TO TEST COLLISION IS WORKING
	// Will create a way to initialize soft bodies / generate them or something and store to a global list, removing when fully destroyed or put into the goal zone.
	// Which will be looped through instead of just checing this specific singular meat cube
	private void Test(Node2D node)
	{
		
		
		
		int totalBonesAtInitialization = Utils.GetSoftBodyBones(MeatCube).Count;
		int currentBoneCount = MeatCube.boneList.Count;
		
		
		
		GD.Print(totalBonesAtInitialization);
		GD.Print(currentBoneCount);
		GD.Print("==========================\n");
		
		// int removedBones = totalBonesAtInitialization - currentBoneCount;
		// GD.Print(removedBones);
		
		
		
		// GD.Print("Skeleton child count: ", MeatCube.Skeleton.GetChildCount());
		// GD.Print("Skeleton bone count: ", MeatCube.Skeleton.GetBoneCount());

		// foreach (Node child in MeatCube.Skeleton.GetChildren())
		// {
		// 	GD.Print(
		// 		child.Name,
		// 		" | Type: ", child.GetType().Name,
		// 		" | Valid: ", IsInstanceValid(child),
		// 		" | In tree: ", child.IsInsideTree()
		// 	);
		// }
	}
	
	
}
