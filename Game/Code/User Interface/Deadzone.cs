using Godot;
using System;

public partial class Deadzone : Control
{
	[Export] Sprite2D area;
	[Export] Sprite2D cursor;
	[Export] Sprite2D deadzoneCircle;
	
	private Vector2 radius        = Vector2.Zero;
	private Vector2 cursorRadius  = Vector2.Zero;
	private Vector2 useableRadius = Vector2.Zero;
	
	
	public override void _Ready()
	{
		radius = area.Texture.GetSize() / 2;
		cursorRadius = cursor.Texture.GetSize() / 2;
		
		useableRadius = radius - cursorRadius;
		
		// duplicate texture and gradient so both deadzones dont share the same resource
		GradientTexture2D texture = deadzoneCircle.Texture.Duplicate() as GradientTexture2D;
		
		texture.Gradient = texture.Gradient.Duplicate() as Gradient;
		
		deadzoneCircle.Texture = texture;
	}
	
	
	public void SetCursorPosition(Vector2 joystickAxis)
	{
		cursor.SetPosition(joystickAxis.LimitLength(1) * useableRadius);
	}
	
	public void SetDeadzone(float input)
	{
		float deadzone = Mathf.Clamp(input, 0, 1);
		
		deadzoneCircle.Scale = Vector2.One * deadzone;
		
		GradientTexture2D texture = deadzoneCircle.Texture as GradientTexture2D;
		
		float middle = Mathf.Lerp(0.66f, 0.70f, deadzone);
		
		texture.Gradient.SetOffset(1, middle);
	}
	
}
