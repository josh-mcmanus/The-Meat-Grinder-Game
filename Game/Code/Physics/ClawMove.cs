using Godot;
using System;

public partial class ClawMove : Node2D
{
	private const string TAG = "ClawMove";
	
	[Export] public RigidBody2D Top;
	[Export] public RigidBody2D Middle;
	[Export] public RigidBody2D Bottom;
	
	
	private const float MoveSpeed = 6;
	private const float RotationSpeedDegrees = 45;
	private const float MouseControl_MaxSpeedAtDistance = 100;
	
	private float rotationSpeedRads;
	private float relativeTopMass;
	private float relativeMiddleMass;
	private float relativeBottomMass;
	
	
	public override void _Ready()
	{
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
	public override void _Input(InputEvent @event)
	{
		// Controller
		if (@event is InputEventJoypadButton)
		{
			DataManager.settingsData.ControlMode = SettingsData.InputMode.Controller;
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
			}
			return;
		}
		
		// KBM
		if (@event is InputEventKey || @event is InputEventMouseButton || @event is InputEventMouseMotion)
		{
			DataManager.settingsData.ControlMode = SettingsData.InputMode.KeyboardAndMouse;
		}
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		MoveClaw();
		RotateClaw();
		
		// Utils.Log(TAG, DataManager.settingsData.ControlMode);
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
	
	private Vector2 GetMouseMovement()
	{
		Vector2 mouseOffset = GetGlobalMousePosition() - Top.GlobalPosition;
		
		float distance = mouseOffset.Length();
		
		if(distance == 0)
		{
			return Vector2.Zero;
		}
		
		float strength = Mathf.Clamp(distance / MouseControl_MaxSpeedAtDistance, 0, 1);
		
		Vector2 direction = mouseOffset / distance;
		
		return direction * strength;
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
	
	// private float ApplyDeadzone(float input, float deadzone)
	// {
	// 	float magnitude = Mathf.Abs(input);

	// 	if (magnitude <= deadzone)
	// 	{
	// 		return 0.0f;
	// 	}
		
	// 	return Mathf.Sign(input) * ((magnitude - deadzone) / (1.0f - deadzone));
	// }
	
}
