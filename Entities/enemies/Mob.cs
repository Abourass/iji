using Godot;
using Iji.Utilities;
using System;

public partial class Mob : RigidBody2D {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		// Set them moving either left or right
		var direction = new Vector2(1, 0);

		// Set the velocity.
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Set("velocity", direction * 100);

		// Start the walk animation
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk");

		// 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
	}

	private void OnVisibleOnScreenNotifier2DScreenExited(){
    QueueFree();
	}
}
