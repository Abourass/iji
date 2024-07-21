using Godot;
using System;
using Iji.Utilities;

public partial class Player: Area2D {
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public float Speed { get; set; } = 300f; // How fast the player will move

	[Export]
	public float JumpForce { get; set; } = 300f; // How high the player will jump

	public Vector2 ScreenSize; // Size of the game window

		// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void Start(Vector2 pos){
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
	private void _on_Player_idle(){
		PlayerSprite.Play("idle");
		// Dim the screen slightly when the player is idle
		PlayerSprite.Modulate = new Color(0.5f, 0.5f, 0.5f);
	}

	public override void _PhysicsProcess(double delta){
		var Delta = ConversionHelper.toFloat(delta);
		ProcessInput(Delta);
	}

	private static float GetHorizontalInput(){
		return Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
	}

	private void ProcessInput(float delta){
		Vector2 direction = new();

		if ( // Check for jump
				Input.GetActionStrength("move_up") > 0
				&& _isJumping == false
			){
			_isJumping = true;
			_velocity.Y = -JumpForce;
		}

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
		direction.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		direction = direction.Normalized() * Speed;
		_velocity.X = Mathf.Lerp(_velocity.X, direction.X, 0.1f);

		// Apply gravity
		if (!_isJumping || _velocity.Y < 0){
			_velocity.Y += Gravity * delta;
		}

		// Move and Slide
		// _velocity = MoveAndSlide(_velocity, Vector2.Up);

		Vector2 newPosition = Position + _velocity * delta;

		// Check if the new position is within the screen, if not clamp
		newPosition.X = Mathf.Clamp(newPosition.X, 0, ScreenSize.X);
		newPosition.Y = Mathf.Clamp(newPosition.Y, 0, ScreenSize.Y);

		// Move the player
		Position = newPosition;

				// Flip the sprite based on direction
		if (_velocity.X != 0){
			PlayerSprite.Play("walk");
			PlayerSprite.FlipH = _velocity.X < 0;
		} else if (_velocity.Y < 0 && _isJumping){
			PlayerSprite.Play("jump");
		}

		// If they stop moving, play the idle animation
		if (_velocity.X == 0 && _velocity.Y == 0){
			PlayerSprite.Play("idle");
		}
	}

	// Use this method to reset jumping state when the player lands
  private void _on_Player_body_entered(Node body){
		_isJumping = false;
  }

	private void OnBodyEntered(Node2D body){
		Hide(); // Disappear on being hit for now
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
    GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}
