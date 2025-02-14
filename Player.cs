using System;
using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
	private Vector2 Direction;
	private float Speed = 100;

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
    }
}


