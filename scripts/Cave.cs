using Godot;
using System;

public partial class Cave : Node2D
{
    private AnimationPlayer _Scene_transition_animation;

    [Export] private CharacterBody2D Player;
    private Node2D player;
    

    public async override void _Ready()
    {
        
        
        _Scene_transition_animation = GetNodeOrNull<AnimationPlayer>("Scene_transition_animation/AnimationPlayer");

        if (_Scene_transition_animation == null)
        {
            GD.PrintErr("AnimationPlayer not found at 'Scene_transition_animation/AnimationPlayer'");
            return;
        }

        var colorRect = GetNodeOrNull<ColorRect>("Scene_transition_animation/ColorRect");
        if (colorRect == null)
        {
            GD.PrintErr("ColorRect not found at 'Scene_transition_animation/ColorRect'");
            return;
        }

        var color = colorRect.Color;
        color.A = 1.0f;
        colorRect.Color = color;

        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        _Scene_transition_animation.Play("fade_out");
    }
    public async void _on_cave_exit_body_entered(Node2D body)
    {
        var globalInstance = GetNode<globall>("/root/Globall");
        if (body == Player)
        {
            player = body;
            globalInstance.cave_exited = true;
            GD.Print("joueur reviens dans playground");

            _Scene_transition_animation.Play("fade_in");     
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");       
            GetTree().ChangeSceneToFile("res://scenes/playground.tscn");
        }
    }
}
