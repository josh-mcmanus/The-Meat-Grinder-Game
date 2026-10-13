

using System.Collections.Generic;
using Godot;

public class SoftBodyData
{
	public List<SoftBodyAttachment> activeSoftBodies = [];
	
	
	
	public static Dictionary<SoftBodyTypes, PackedScene> softBodyScenes = new()
	{
		[SoftBodyTypes.MeatCube1] = GD.Load<PackedScene>("res://Scenes/Physics/meat_cube_1.tscn"),
		[SoftBodyTypes.MeatLong1] = GD.Load<PackedScene>("res://Scenes/Physics/meat_long_1.tscn"),
	};
	
	
	// public struct SoftBodySceneData(SoftBodyTypes type, PackedScene scene, )
	// {
		
	// }
	
	
	public enum SoftBodyTypes
	{
		MeatCube1,
		MeatLong1,
	}
}
