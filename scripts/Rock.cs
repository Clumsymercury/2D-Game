using Godot;
using System;

public partial class Rock : Node2D
{
    [Export] public int hits_to_break = 3; 
    private int current_hits = 0;
    private bool can_be_hit = false;
	[Export] private CharacterBody2D Player;
	private Node2D player;
	[Export] private bool dialogue_finished;//prends variable de NPC_2
	public override void _Process(double delta)
    {
        if (Player.Position.Y > Position.Y)
		{
			ZIndex = -1;
		}
		else
		{
			ZIndex = 6;
		}
    }
	private void _on_area_can_take_damage_body_entered(Node2D body)
	{
		if (body == Player)
		{
			GD.Print("player enterd ");
			player = body;
			can_be_hit = true;
		}
	}
	private void _on_area_can_take_damage_body_exited(Node2D body)
	{
		if (body == Player)
		{
			player = body;
			can_be_hit = false;
		}
	}

    public void hit()
	{
		if (!can_be_hit && !dialogue_finished)
			return;

		current_hits++;
		GD.Print($"Rock hit! {current_hits}/{hits_to_break}");

		if (current_hits >= hits_to_break)
		{
			GD.Print("Rock destroyed!");
			QueueFree();
		}
	}
}
