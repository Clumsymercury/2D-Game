using Godot;
using System;

public partial class Ennemy : CharacterBody2D
{   
    
    [Export]
    public float Speed = 40f;

    private bool playerChase = false;
    private Node2D player;

    public float health = 100f;
    private bool player_inattack_zone = false;
    
    

    public override void _PhysicsProcess(double delta)
    {

        deal_with_damage();

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
    public void ennemy()
    {
    }

    public void _on_ennemy_hitbox_body_entered(Node2D body)
    {
        if (body.HasMethod("player"))
        {
            player_inattack_zone = true;
        }
    }
    
    public void _on_ennemy_hitbox_body_exited(Node2D body)
    {
        if (body.HasMethod("player"))
        {
            player_inattack_zone = false;
        }
    }
    public void deal_with_damage()
    {
        var globalInstance = GetNode<globall>("/root/Globall");


        if (player_inattack_zone && globalInstance.player_current_attack)
        {
            health = health - 20;
            GD.Print("slime health = ", health);
            if (health <= 0)
            {
                QueueFree();
            }
        }
    }
}


