using Godot; 
using System;

public partial class CastleScene1 : Node2D
{
    private AnimationPlayer _Scene_transition_animation;

    [Export] private CharacterBody2D Player;
    private Node2D player;
    

    public async override void _Ready()
    {
        
        
        _Scene_transition_animation = GetNodeOrNull<AnimationPlayer>("Scene_transition_animation/AnimationPlayer");

        if (_Scene_transition_animation == null)
        {
            GD.PrintErr("❌ AnimationPlayer not found at 'Scene_transition_animation/AnimationPlayer'");
            return;
        }

        var colorRect = GetNodeOrNull<ColorRect>("Scene_transition_animation/ColorRect");
        if (colorRect == null)
        {
            GD.PrintErr("❌ ColorRect not found at 'Scene_transition_animation/ColorRect'");
            return;
        }

        var color = colorRect.Color;
        color.A = 1.0f;
        colorRect.Color = color;

        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        _Scene_transition_animation.Play("fade_out");

                // problem avec ca if (globalInstance.boss_exited)
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");

            var cave_exit_spawn_point = GetNode<Marker2D>("cave_exit_spawn");
            Player.Position = cave_exit_spawn_point.GlobalPosition;


            // il y a un problem avec ca globalInstance.boss_exited = false;
        }
    }
    public async void _on_castle_exit_body_entered(Node2D body)
    {
        var globalInstance = GetNode<globall>("/root/Globall");
        if (body == Player)
        {
            player = body;
            globalInstance.castle1_exited = true;
            GD.Print("joueur reviens dans playground");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/playground.tscn");
        }
    }

    public async void _on_boss_entrance_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans boss zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/boss.tscn");
        }
    }
    
    
}

