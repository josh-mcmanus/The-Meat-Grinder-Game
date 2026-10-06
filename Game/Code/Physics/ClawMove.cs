using Godot;
using System;

public partial class ClawMove : Node2D
{
	private const string TAG = "ClawMove";
	
	[Export] public RigidBody2D Top;
	[Export] public RigidBody2D Middle;
	[Export] public RigidBody2D Bottom;
	
	[Export] private Marker2D MouseTarget;
	[Export] private Camera2D Camera;
	
	
	private const float MoveSpeed = 6;
	private const float RotationSpeedDegrees = 45;
	
	private float rotationSpeedRads;
	private float relativeTopMass;
	private float relativeMiddleMass;
	private float relativeBottomMass;
	
	
	public override void _Ready()
	{
		// Prevent processing when paused
		ProcessMode = ProcessModeEnum.Pausable;
		
		// Capture mouse
		Utils.CaptureMouse();
		
		// Set phsyics properties to prevent tunneling on soft bodies
		Top.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Middle.ContinuousCd = RigidBody2D.CcdMode.CastShape;
    	Bottom.ContinuousCd = RigidBody2D.CcdMode.CastShape;
		
		Top.ContactMonitor = true;
		Middle.ContactMonitor = true;
		Bottom.ContactMonitor = true;
		
		Top.MaxContactsReported = 4;
		Middle.MaxContactsReported = 4;
		Bottom.MaxContactsReported = 4;
		
		relativeTopMass = Top.Mass + Middle.Mass + Bottom.Mass;
		relativeMiddleMass = Middle.Mass + Bottom.Mass;
		relativeBottomMass = Bottom.Mass;

		rotationSpeedRads = Mathf.DegToRad(RotationSpeedDegrees);
		
		// HapticTest();
	}
	
	
	// Switch control scheme based on current input method
	// Set mouse motion
	public override void _Input(InputEvent @event)
	{
		// Controller
		if (@event is InputEventJoypadButton)
		{
			DataManager.settingsData.ControlMode = SettingsData.InputMode.Controller;
			MouseTarget.GlobalPosition = Top.GlobalPosition;
			return;
		}
		if (@event is InputEventJoypadMotion motion)
		{
			float deadzone = motion.Axis switch
			{
				JoyAxis.LeftX  or JoyAxis.LeftY  => DataManager.settingsData.DeadzoneLeft,
				JoyAxis.RightX or JoyAxis.RightY => DataManager.settingsData.DeadzoneRight,
				_ => 1
			};
			
			if (Math.Abs(motion.AxisValue) > deadzone)
			{
				DataManager.settingsData.ControlMode = SettingsData.InputMode.Controller;
				MouseTarget.GlobalPosition = Top.GlobalPosition;
			}
			return;
		}
		
		// KBM
		if (@event is InputEventMouseMotion mouseMotion)
		{
			DataManager.settingsData.ControlMode = SettingsData.InputMode.KeyboardAndMouse;
			
			
			Vector2 targetOffset = MouseTarget.GlobalPosition - Top.GlobalPosition;
			targetOffset += mouseMotion.ScreenRelative * DataManager.settingsData.MouseSensitivity;
			targetOffset = targetOffset.LimitLength(DataManager.settingsData.MaxMouseDistance);
			
			MouseTarget.GlobalPosition = Top.GlobalPosition + targetOffset;
			
			return;
		}
		if (@event is InputEventKey || @event is InputEventMouseButton)
		{
			DataManager.settingsData.ControlMode = SettingsData.InputMode.KeyboardAndMouse;
		}
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		MoveClaw();
		RotateClaw();
		
		Camera.GlobalPosition = Top.GlobalPosition;
	}
	
	
	private void HapticTest()
	{
		Utils.Log(TAG, Input.HasJoyVibration(0));
		
		const int   Device = 0;
		const float WeakMotorStrength = 0;
		const float StrongMotorStrength = 0.1f;
		const float DurationSeconds = 2;

		if (Input.GetConnectedJoypads() != null)
		{
			Input.StartJoyVibration(Device, WeakMotorStrength, StrongMotorStrength, DurationSeconds);
		}
		
	}
	
	
	private void MoveClaw()
	{
		Vector2 movementVector = DataManager.settingsData.ControlMode switch
		{
			SettingsData.InputMode.KeyboardAndMouse => GetMouseMovement(),
			SettingsData.InputMode.Controller => GetControllerRightStickMovement(),
			_ => Vector2.Zero,
		};
		
		if (movementVector == Vector2.Zero)
			return;
		
		Top.ApplyImpulse(movementVector * MoveSpeed * relativeTopMass);
		Middle.ApplyImpulse(movementVector * MoveSpeed * relativeMiddleMass);
		Bottom.ApplyImpulse(movementVector * MoveSpeed * relativeBottomMass);
	}
	
	// Get the mouse position as a vector, of the mouses position relative to the center of the screen, not the scene
	private Vector2 GetMouseMovement()
	{
		
		Vector2 mouseOffset = MouseTarget.GlobalPosition - Top.GlobalPosition;
		float distance = mouseOffset.Length();
		
		float deadzone = DataManager.settingsData.MouseDeadzone;
		float maxDistance = DataManager.settingsData.MaxMouseDistance;

		if (distance <= deadzone)
		{
			return Vector2.Zero;
		}
		
		float strength = Mathf.Clamp((distance - deadzone) / (maxDistance - deadzone), -1, 1);
		
		return mouseOffset.Normalized() * strength;
	}
	
	// This is specifically for moving the claw around on controller, as it does not have mouse coordinates
	private Vector2 GetControllerRightStickMovement()
	{
		Vector2 joystickInput = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		
		if (joystickInput.Length() < DataManager.settingsData.DeadzoneRight)
		{
			return Vector2.Zero;
		}
		
		return joystickInput;
	}
	
	
	private void RotateClaw()
	{
		// Get rotation
		float rotationTopInput    = Input.GetAxis("RotateLeft", "RotateRight");
		float rotationMiddleInput = Input.GetAxis("Right", "Left");
		float rotationBottomInput = Input.GetAxis("Up", "Down");
		
		// Deadzone
		if (DataManager.settingsData.ControlMode == SettingsData.InputMode.Controller)
		{
			Vector2 joystickInput = Input.GetVector("Left", "Right", "Up", "Down");

			if (joystickInput.Length() < DataManager.settingsData.DeadzoneLeft)
			{
				rotationMiddleInput = 0;
				rotationBottomInput = 0;
			}
		}
		
		// Apply Rotation
		Top.AngularVelocity = rotationTopInput * rotationSpeedRads * 3;
		Middle.AngularVelocity = rotationMiddleInput * rotationSpeedRads * relativeMiddleMass;
		Bottom.AngularVelocity = rotationBottomInput * rotationSpeedRads * relativeBottomMass;
	}
	
	
}
