using Godot;
using System;

public partial class MainRender : Node2D
{
	[Export] Node2D Main;
	[Export] CanvasLayer Settings;
	
	[Export] AudioStreamPlayer MenuMusicPlayer;
	
	
	
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Settings.Visible = false;
		Main.GetTree().Paused = false;
		
		// Background menu music
		MenuMusicPlayer.Finished += OnMusicFinished;
	}
	
	
	
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ToggleMenu"))
		{
			// GD.Print("Toggle");
			// GD.Print(Settings.Visible);
			Settings.Visible = !Settings.Visible;
			Main.GetTree().Paused = !Main.GetTree().Paused;

			if (Settings.Visible)
			{
				MenuMusicPlayer.Play();
			}
			else
			{
				MenuMusicPlayer.Stop();
			}
			
		}
    }
	
	
	
	private void OnMusicFinished()
    {
        MenuMusicPlayer.Play();
    }
	
	
}
