using Godot;
using System;


public partial class Npc : CharacterBody2D
{
	private float speed = 15;
	private state current_state = state.idle;

	private Vector2 dir = Vector2.Right;
	private Vector2 start_pos;

	private bool is_roaming = true;
	private bool is_chatting = false;

	private Node2D player;
	private bool player_in_chat_zone = false;

	private float distance_max = 50;

	private int enemy_kill_i = 0;
	private int enemy_kill_f = 5;

	private bool in_chat_area = false;
	private bool has_give_mission = false;

	[Export] private CharacterBody2D Player;




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

		var dialogue = GetNode<Dialogue>("Dialogue");
		//dialogue.Connect("dialogue_finished", new Callable(this, nameof(_on_dialogue_dialogue_finished)));// enft pas besoin car deja connecte dans godot :)

		// ca connecte le ennemy dans le jeux 
		foreach (Ennemy enemy in GetTree().GetNodesInGroup("Enemies"))
		{
			enemy.Connect("Enemy_died", new Callable(this, nameof(_on_Enemy_died)));
		}
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
		if (Input.IsActionPressed("chat") && in_chat_area)
		{
			GD.Print("chatting");
			var dialogue = GetNode<Dialogue>("Dialogue");
			dialogue.Start();
			is_roaming = false;
			is_chatting = true;
			sprite.Play("idle");
			has_give_mission = true;
		}
		
		if (Player.Position.Y > Position.Y)
		{
			ZIndex = 1;
		}
		else
		{
			ZIndex = 6;
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
			var new_position = Position += dir * speed * (float)delta;

			// in compare toujour par rapport à start_pos qui est fixe
			float minX = start_pos.X - distance_max;
			float maxX = start_pos.X + distance_max;
			float minY = start_pos.Y - distance_max;
			float maxY = start_pos.Y + distance_max;

			// Limite sur x
			if (new_position.X < minX || new_position.X > maxX)
			{
				dir.X = -dir.X; // rebond
				new_position.X = Mathf.Clamp(new_position.X, minX, maxX);
			}

			// Limite sur y
			if (new_position.Y < minY || new_position.Y > maxY)
			{
				dir.Y = -dir.Y; // rebond
				new_position.Y = Mathf.Clamp(new_position.Y, minY, maxY);
			}

			Position = new_position;
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
	public void _on_dialogue_dialogue_finished()
	{
		is_chatting = false;
		is_roaming = true;

		var hud = GetTree().CurrentScene.GetNode<Godot.CanvasLayer>("HUD");
		var quest_label = hud.GetNode<Godot.Label>("quest_label");

		quest_label.Visible = true;
		enemy_kill_i = 0;
		quest_label.Text = $"Tuer 5 monstres {enemy_kill_i}/{enemy_kill_f}";
	}
	public void _on_Enemy_died()
	{
		var globalInstance = GetNode<globall>("/root/Globall");
		

		var hud = GetTree().CurrentScene.GetNode<Godot.CanvasLayer>("HUD");
		var quest_label = hud.GetNode<Godot.Label>("quest_label");
		if (has_give_mission)
		{
			enemy_kill_i++;
			quest_label.Text = $"Tuer {enemy_kill_f} monstres ({enemy_kill_i}/{enemy_kill_f})";
		}
		

		if (enemy_kill_i >= enemy_kill_f)
		{
			quest_label.Text = "Quête terminée ! Tu as merité une récompense (+25XP)";
			globalInstance.AddXP(25);

		}
	}
	public void _on_chat_detection_area_mouse_entered()
	{
		in_chat_area = true;
	}
	public void _on_chat_detection_area_mouse_exited()
	{
		in_chat_area = false;
	}
}