using Godot;

#pragma warning disable CS0162 // Unreachable code detected


public partial class Utils
{
	// Debug mode, prints out debug information
	public const bool DEBUG = true;
	
	
	public static void Log(string TAG, object msg)
	{
        if (!DEBUG) return;
        GD.Print($"[{TAG}] {msg}");
	}
	
	public static void Err(string TAG, object msg)
	{
		if (!DEBUG) return;
        GD.PrintErr($"[{TAG}] {msg}");
	}
	
}
