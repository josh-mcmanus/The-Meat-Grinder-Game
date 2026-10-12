using Godot;
using System;
using System.Collections.Generic;

public partial class MeatDetectionArea : Area2D
{
	[Signal] public delegate void CollisionDetectedEventHandler(Node2D node);
	
	
	public override void _Ready()
	{
		BodyShapeEntered += EnteredBodyShape;
	}
	
	
    public void EnteredBodyShape(Rid ID, Node2D node, long bodyShapeIndex, long localShapeIndex)
	{
		EmitSignal(SignalName.CollisionDetected, node);
	}
	
	
}
