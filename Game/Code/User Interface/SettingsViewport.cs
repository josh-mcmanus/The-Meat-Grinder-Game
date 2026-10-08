using Godot;
using System;


/*
	Sets the menu resolution size based on the current size of the game window resolution
	
	
*/
public partial class SettingsViewport : SubViewportContainer
{
	private SubViewport _viewport;
	
	public override void _Ready()
	{
		_viewport = GetNode<SubViewport>("SubViewport");

        UpdateViewportSize();

        GetTree().Root.SizeChanged += UpdateViewportSize;
	}
	
	private void UpdateViewportSize()
	{
		_viewport.Size = (Vector2I)GetViewport().GetVisibleRect().Size;
	}
	
}
