using Godot;
using System;

public partial class AppleCollectable : Area2D
{

    private AnimatedSprite2D animated_sprite_2d;
    private AnimationPlayer animation_2d;

    public override void _Ready()
    {
        fallfromtree();
        animated_sprite_2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animation_2d = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    private async void fallfromtree()
    {
        animation_2d.Play("fallingfromtree");

        
        await ToSignal(GetTree().CreateTimer(1.5), "timeout");

        animation_2d.Play("fade"); 

        GD.Print("+1 apples");

        await ToSignal(GetTree().CreateTimer(0.3), "timeout");
        QueueFree();
    }

}
