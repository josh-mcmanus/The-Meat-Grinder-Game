

public class SettingsData
{
	// Controls
	// Controller
	public InputMode ControlMode { get; set; } = InputMode.Controller;
	public float DeadzoneLeft { get; set; } = 0.05f;
	public float DeadzoneRight { get; set; } = 0.05f;
	
	// Keyboard and Mouse
	public float MouseSensitivity  { get; set; } = 0.3f; // 0.01 - 1.00
	public int   MaxMouseDistance  { get; set; } = 100;  // 50 - 500
	public int   MouseDeadzone     { get; set; } = 5;	 // 0  - 50
	
	
	//Video Settings
    public WindowMode Window { get; set; } = WindowMode.Windowed;
    public VSyncMode  VSync { get; set; } = VSyncMode.Disabled;
	public int  DisplayResolutionWidth { get; set; } = 1920;
	public int  DisplayResolutionHeight { get; set; } = 1080;
	
	//Audio Volumes
    public float MasterVolume { get; set; } = 0.5f;
    public float MusicVolume { get; set; } = 0.5f;
    public float SFXVolume { get; set; } = 0.5f;
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
