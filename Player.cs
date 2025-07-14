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
		_sword = GetNode<Sword>("Sword");
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
	//test depee
	private Sword _sword;

    

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("attack"))
            _sword.Attack();

        Vector2 dir = new Vector2();

		if (Input.IsActionPressed("ui_right"))
    		dir.X += 1;
		if (Input.IsActionPressed("ui_left"))
    		dir.X -= 1;
		if (Input.IsActionPressed("ui_down"))
   			dir.Y += 1;
		if (Input.IsActionPressed("ui_up"))
    		dir.Y -= 1;

        if (dir != Vector2.Zero)
        {
            _sword.Rotation = dir.Angle();
            _sword.Position = dir.Normalized() * 16;
        }
    }
}

