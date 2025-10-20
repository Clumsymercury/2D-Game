using Godot;
using System;

public partial class Npc2 : CharacterBody2D
{
	private bool in_chat_area = false;


	[Export] private CharacterBody2D Player;
	private bool dialogue_finished = false;

	public override void _Process(double delta)
	{
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("idle");
		if (Input.IsActionPressed("chat") && in_chat_area)
		{
			GD.Print("chatting");
			var dialogue = GetNode<Dialogue>("Dialogue");
			dialogue.Start();

			

		}

		if (Player.Position.Y > Position.Y)
		{
			ZIndex = 0;
		}
		else
		{
			ZIndex = 6;
		}
	}
	public void _on_area_2d_mouse_entered()
	{
		in_chat_area = true;
	}
	public void _on_area_2d_mouse_exited()
	{
		in_chat_area = false;
	}
	public void _on_dialogue_dialogue_finished()
	{
		dialogue_finished = true;
	}
}
