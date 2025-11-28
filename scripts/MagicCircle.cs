using Godot;
using System;

public partial class MagicCircle : Area2D
{
    [Export] public float duration = 2.0f;
    [Export] public int damage = 1;
    [Export] public float delayBeforeDamage = 1.0f; 
    [Export] public string sprite_node_path = "AnimatedSprite2D";

    private AnimatedSprite2D anim_sprite;
    private bool hasDamaged = false;

    public override async void _Ready()
    {
        
        anim_sprite = GetNode<AnimatedSprite2D>(sprite_node_path);
        BodyEntered += _on_body_entered;

        anim_sprite.Play("magic_circle");

        
        Scale = Vector2.Zero;
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.5f);

       
        await ToSignal(GetTree().CreateTimer(delayBeforeDamage), "timeout");

        
        anim_sprite.Modulate = new Color(1, 0.3f, 0.3f); 

       
        foreach (var body in GetOverlappingBodies())
        {
            if (body is Player player && IsInstanceValid(player))
            {
                player.health -= damage;
                player.UpdateHealth();
                GD.Print("Player hit by magic circle (after delay)!");
            }
        }

        
        await ToSignal(GetTree().CreateTimer(duration - delayBeforeDamage), "timeout");

        var fadeTween = CreateTween();
        fadeTween.TweenProperty(this, "modulate:a", 0.0f, 0.4f);
        await ToSignal(fadeTween, "finished");

        QueueFree();
    }

    

    private void _on_body_entered(Node2D body)
    {
        if (body is Player player && IsInstanceValid(player))
        {
            player.health -= damage;
            player.UpdateHealth();
            GD.Print("Player hit by magic circle!");
        }
    }
}
