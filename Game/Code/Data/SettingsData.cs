

public class SettingsData
{
	// Controller
	public InputMode ControlMode { get; set; } = InputMode.Controller;
	public float Deadzone { get; set; } = 0.1f;
	
	//Video Settings
    public WindowMode Window { get; set; } = WindowMode.Windowed;
    public VSyncMode  VSync { get; set; } = VSyncMode.Disabled;
	public int  ResolutionWidth { get; set; } = 1920;
	public int  ResolutionHeight { get; set; } = 1080;
	
	//Audio Volumes
    public float MasterVolume { get; set; } = 0.5f;
    public float MusicVolume { get; set; } = 0.5f;
    public float SoundEffectVolume { get; set; } = 0.5f;
    public float UIVolume { get; set; } = 0.5f;
	
	
	
	public enum InputMode
	{
		KeyboardAndMouse,
		Controller
	}
	
	public enum WindowMode
	{
		Windowed,
		FullScreen,
		Borderless
	}
	
	public enum VSyncMode
	{
		Disabled,
		Enabled,
		Adaptive,
		Mailbox
	}
}
