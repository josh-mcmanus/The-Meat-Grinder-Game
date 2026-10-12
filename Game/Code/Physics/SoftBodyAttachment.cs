using Godot;
using System;
using System.Collections.Generic;


/*	
	Used for attaching a single rigid body to a soft body.
	The rigid body has 2 attachment points, JointA and JointB.
	Connect both joints NodeA's to a different bone, and NodeB's to the rigidbody
	
	This Checks if JointA or B has its attached node removed, when a soft body experiences a break.
	If it does, remove the joint before the node in the soft body is removed.
	So there are no references to null nodes.
	
*/
public partial class SoftBodyAttachment : Node2D
{
	[Export] public PinJoint2D[] Joints;
	[Export] public Skeleton2D Skeleton;
	
	
	private PhysicsBody2D[] jointNodes;
	public  List<PhysicsBody2D> boneList;
	
	
    public override void _Ready()
    {
		jointNodes = new PhysicsBody2D[Joints.Length];
		
		// Generate n number of joint nodes, for comparison later
		for (int i = 0; i < Joints.Length; i++)
		{
			PinJoint2D joint = Joints[i];
		
			if(IsInstanceValid(joint) && !joint.NodeA.IsEmpty)
			{
				jointNodes[i] =  joint.GetNode(joint.NodeA) as PhysicsBody2D;
			}
		}
		
		// save a list of all starting bones, for score calculation later
		boneList = Utils.GetBonesFromNode(this);
		GD.Print(boneList.Count);
    }
	
	
	// Connected signal, run removal over every joint in list
	public void OnSoftBody2DJointRemoved(PhysicsBody2D bodyA, PhysicsBody2D bodyB)
	{
		for (int i = 0; i < Joints.Length; i++)
		{
			// GD.Print(bodyA.Name);
			// GD.Print(bodyB.Name);
			RemoveJoint(i, bodyA, bodyB);
		}
		
		boneList.Remove(bodyB);
		
	}
	
	
	// Remove joint node, if soft body break is removing the physics body associated with the joint
	private void RemoveJoint(int index, PhysicsBody2D bodyA, PhysicsBody2D bodyB)
	{
		PinJoint2D joint = Joints[index];
		PhysicsBody2D jointNode = jointNodes[index];
		
		
		if (!IsInstanceValid(joint)) return;
		
		if (!IsInstanceValid(jointNode)) return;
		
		if (bodyA != jointNode && bodyB != jointNode) return;
		
		
		joint.QueueFree();
		Joints[index] = null;
		jointNodes[index] = null;
	}
}
