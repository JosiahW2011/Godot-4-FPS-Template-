using Godot;
using System;

public partial class CharacterBody3d : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	
	public Node3D _Neck;
	public Camera3D _Cam;
	
	public override void _Ready() {
		_Neck = GetNode<Node3D>("Neck");
		_Cam = GetNode<Camera3D>("Neck/Camera3D");
	}
	
	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton) {
			Input.MouseMode = Input.MouseModeEnum.Captured;
		} else if (@event.IsActionPressed("ui_cancel")) {
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
		if (Input.MouseMode == Input.MouseModeEnum.Captured) {
			if (@event is InputEventMouseMotion mouseMotion) {
				_Neck.RotateY(-mouseMotion.Relative.X * 0.01f);
				_Cam.RotateX(-mouseMotion.Relative.Y * 0.01f);
				
				Vector3 _camRot = _Cam.Rotation;
				_camRot.X = Mathf.Clamp(
					_camRot.X,
					Mathf.DegToRad(-45),
					Mathf.DegToRad(75)
				);
				_Cam.Rotation = _camRot;
			}
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor()) {
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector("a", "d", "w", "s");
		Vector3 direction = (_Neck.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero) {
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		} else {
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
