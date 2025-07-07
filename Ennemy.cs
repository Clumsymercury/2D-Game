using Godot;
using System;

public partial class Ennemy : CharacterBody2D
{
    [Export]
    public float Speed = 40f;

    private bool playerChase = false;
    private Node2D player;

    public override void _PhysicsProcess(double delta)
    {
        if (playerChase && player != null)
        {
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("walk");
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            Velocity = direction * Speed;
            MoveAndSlide();
        
			if (player.Position.X - Position.X < 0)
			{
    			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
			}
			else
			{
    			GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
			}
		}
        else
		{
			GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("IDLE");
			Velocity = Vector2.Zero;
		}
    }

    public void _on_detection_area_body_entered(Node2D body)
    {
        
        
        GD.Print("Player detected!");
        player = body;
        playerChase = true;
        
    }

    public void _on_detection_area_body_exited(Node2D body)
    {
        
        
        GD.Print("Player left detection!");
        player = null;
        playerChase = false;
        
    }
}


