using Godot;
using System;
using Iji.Utilities;

public partial class Player: CharacterBody2D {
	[Export]
	public float Speed { get; set; } = 300f; // How fast the player will move

	[Export]
	public float Gravity { get; set; } = 200f; // How fast the player will fall

	public Vector2 _velocity = new Vector2(0, 0); // Current Velocity
	private bool _isJumping = false; 
	public Vector2 ScreenSize; // Size of the game window
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenSize = GetViewportRect().Size;
	}

	public override void _PhysicsProcess(double delta){
		var Delta = ConversionHelper.toFloat(delta);
		ProcessInput();
		ApplyGravity(ConversionHelper.toFloat(Delta));
		MoveCharacter(Delta);
	}
	
	private void ProcessInput(){
		var direction = new Vector2();

		// if keys are pressed it will return 1 for ui_right, -1 for ui_left, and 0 for neither
		direction.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		direction.Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

		// Normalize direction and scale by speed
		direction = direction.Normalized() * Speed;
		// Update velocity based on input
		_velocity.X = Mathf.Lerp(_velocity.X, direction.X, 0.1f);
		_velocity.Y = Mathf.Lerp(_velocity.Y, direction.Y, 0.1f);
		
		// Change animation based on input
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (direction.Length() > 0){
			animatedSprite2D.Play("walk"); // Play walk animation if moving
		} else {
			animatedSprite2D.Stop(); // Stop animation if not moving
		}

		// Flip the sprite based on direction
		if (_velocity.X != 0){
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = _velocity.X < 0;
		} else if (_velocity.Y != 0){
			animatedSprite2D.Animation = "jump";
			animatedSprite2D.FlipV = _velocity.Y > 0;
		} else {
			animatedSprite2D.Animation = "idle";
		}
	}

	private void ApplyGravity(float delta){
		// Apply gravity
		_velocity.Y += 9.8f * delta;
	}

	private void MoveCharacter(float delta){
		// Move the player
		// Manually move the player
		Vector2 newPosition = Position + _velocity * delta;
		CollisionShape2D collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		// Check if the new position is within the screen, if not clamp
		newPosition.X = Mathf.Clamp(newPosition.X, 0, ScreenSize.X);
		newPosition.Y = Mathf.Clamp(newPosition.Y, 0, ScreenSize.Y);


		// Set the new position
		Position = newPosition;
	}

	private void _on_Player_jump(){
		if (!_isJumping){
			_velocity.Y = -500;
			_isJumping = true;
		}
	}

	private void HorizontalMovement(float delta){
		// Move

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
