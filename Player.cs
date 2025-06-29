using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private Vector2 Direction;
	private float Speed = 100;

	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		// Get the AnimationPlayer node (make sure it's named exactly "AnimationPlayer")
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		Direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Velocity = Direction * Speed;
		MoveAndSlide();

		// Handle animation
		if (Direction != Vector2.Zero)
		{
			if (Direction.X != 0) // Prioritize horizontal movement
			{
				if (Direction.X > 0)
					animationPlayer.Play("walk right");
				else
					animationPlayer.Play("walk left");
			}
			else if (Direction.Y > 0)
				animationPlayer.Play("walk down");
			else
				animationPlayer.Play("walk up");
		}
		else
		{
			animationPlayer.Stop();
		}
	}
}

