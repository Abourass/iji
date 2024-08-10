using Godot;
using System;
using Iji.Utilities;

public partial class Player : CharacterBody2D {
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public short Speed { get; set; } = 300; // How fast the player will move

	[Export]
	public short JumpForce { get; set; } = 300; // How high the player will jump

	[Export]
	public short Gravity { get; set; } = 100; // How fast the player will fall

	public Vector2 ScreenSize; // Size of the game window

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void Start(Vector2 pos) {
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}

	private AnimatedSprite2D PlayerSprite; // Reference to the AnimatedSprite2D node
	private bool _isJumping = false;
	private Vector2 _velocity = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenSize = GetViewportRect().Size;
		PlayerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		PlayerSprite.Play("idle");
	}

	// This method is used to detect when direction has stopped, to play the idle animation
	private void _on_Player_idle() {
		PlayerSprite.Play("idle");
		// Dim the screen slightly when the player is idle
		PlayerSprite.Modulate = new Color(0.5f, 0.5f, 0.5f);
	}

	public override void _PhysicsProcess(double delta) {
		ProcessInput(ConversionHelper.ToSingle(delta));
	}

	private static float GetHorizontalInput() {
		return Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
	}

	private void ProcessInput(float delta) {
	/**
		* This is a simple way to get the direction the player is moving.
		* It will return a value between -1 and 1.
		* If the player is moving right, it will return 1.
		* If the player is moving left, it will return -1.
		* If the player is not moving, it will return 0.
		* ------------
		* Then we normalize the direction and multiply it by the speed.
		* This will give us a vector that we can use to move the player.
		*/
		Vector2 direction = new(){
			X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up")
		};
		// If Input if digital we need to normalize direction to avoid diagonal movement being faster
		if (Math.Abs(direction.X) > 0 && Math.Abs(direction.Y) > 0) {
			direction = direction.Normalized();
		}

		var movement = direction * Speed * delta;

		// _velocity.X = Mathf.Lerp(_velocity.X, direction.X, 0.1f);

		// // Apply gravity
		// if (!_isJumping || _velocity.Y < 0) {
		// 	_velocity.Y += Gravity * delta;
		// }

		// Check for jump
		// if (Input.GetActionStrength("move_up") > 0 && _isJumping == false){
		// 	_isJumping = true;
		// 	_velocity.Y = -JumpForce;
		// }

		// Move and Slide
		// _velocity = MoveAndSlide(_velocity, Vector2.Up);

		// Vector2 newPosition = Position + _velocity * delta;

		// Check if the new position is within the screen, if not clamp
		// movement.X = Mathf.Clamp(movement.X, 0, ScreenSize.X);
		// movement.Y = Mathf.Clamp(movement.Y, 0, ScreenSize.Y);

		// // Move the player
		// Position = newPosition;

		// Flip the sprite based on direction
		if (movement.X < 0.1) {
			PlayerSprite.Play("walk");
			PlayerSprite.FlipH = movement.X < 0;
		} else if (movement.Y < 0.1) {
			PlayerSprite.Play("jump");
		}

		// // If they stop moving, play the idle animation
		// if (_velocity.X == 0 && _velocity.Y == 0) {
		// 	PlayerSprite.Play("idle");
		// }

		// Brighten the screen when the player is moving
		PlayerSprite.Modulate = new Color(1, 1, 1);

		// // Emit signal when the player is hit
		// if (Input.IsActionJustPressed("hit")) {
		// 	EmitSignal(SignalName.Hit);
		// }

		

		MoveAndCollide(movement);
	}

	// Use this method to reset jumping state when the player lands
	private void _on_Player_body_entered(Node body) {
		_isJumping = false;
	}

	private void OnBodyEntered(Node2D body) {
		Hide(); // Disappear on being hit for now
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
