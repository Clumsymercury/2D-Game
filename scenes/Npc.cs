using Godot;
using System;

public partial class Npc : CharacterBody2D
{
	private const float speed = 30f;
	private state current_state = state.idle;

	private Vector2 dir = Vector2.Right;
	private Vector2 start_pos;

	private bool is_roaming = true;
	private bool is_chatting = false;

	private Node2D player;
	private bool player_in_chat_zone = false;

	private enum state
	{
		idle,
		new_dir,
		move
	}

	public override void _Ready()
	{
		GD.Randomize();
		start_pos = Position;
	}

	public override void _Process(double delta)
	{
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (current_state == state.idle)
		{
			sprite.Play("idle");
		}
		else if (current_state == state.move && !is_chatting)
		{
			if (dir.X == -1)
				sprite.Play("walk_w");
			if (dir.X == 1)
				sprite.Play("walk_e");
			if (dir.Y == -1)
				sprite.Play("walk_n");
			if (dir.Y == 1)
				sprite.Play("walk_s");
		}

		if (is_roaming)
		{
			switch (current_state)
			{
				case state.idle:
					break;

				case state.new_dir:
					dir = choose(new Vector2[] { Vector2.Right, Vector2.Up, Vector2.Left, Vector2.Down });
					break;

				case state.move:
					move(delta);
					break;
			}
		}
		if (Input.IsActionPressed("chat"))
		{
			GD.Print("chatting");
			is_roaming = false;
			is_chatting = true;
			sprite.Play("idle");
		}
	}

	private T choose<T>(T[] array)//le T rends possible pour que on puisse utilisr choose pour nimporte quel classe
	{
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		return array[rng.RandiRange(0, array.Length - 1)];
	}

	private void move(double delta)
	{
		if (!is_chatting)
		{
			Position += dir * speed * (float)delta;
		}
	}

	public void _on_chat_detection_area_body_entered(Node2D body)
	{
		player = body;
		player_in_chat_zone = true;
	}
	public void _on_chat_detection_area_body_exited(Node2D body)
	{
		player = body;
		player_in_chat_zone = false;
	}
	public void _on_timer_timeout()
	{
		var timer = GetNode<Timer>("Timer");
    	timer.WaitTime = choose(new float[] { 0.5f, 1f, 1.5f });

    	current_state = choose(new state[] { state.idle, state.new_dir, state.move });
	}
}