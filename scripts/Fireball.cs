using Godot;
using System;

public partial class Fireball : Area2D
{
    [Export] public float speed = 200f;
    public Vector2 direction = Vector2.Zero;
    private AnimatedSprite2D anim;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        anim.Play("fly"); 
        BodyEntered += _on_body_entered;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += direction * speed * (float)delta;
    }

    private async void _on_body_entered(Node2D body)
    {
        if (body is Player player )
        {
            anim.Play("fireball_impact");
            speed = 0; 
            QueueFree();  
            player.health -= 1;
            player.UpdateHealth();
            GD.Print("player hit by fireball!");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            
        }
        
    }
}

