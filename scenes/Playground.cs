using Godot;
using System; 
public partial class Playground : Node2D
{
    
    [Export] private CharacterBody2D Player;
    private Node2D player;
    private AnimationPlayer _Scene_transition_animation;
    

    public async override void _Ready()
    {
        var globalInstance = GetNode<globall>("/root/Globall");
        

        _Scene_transition_animation = GetNode<AnimationPlayer>("Scene_transition_animation/AnimationPlayer");

        if (globalInstance.castle1_exited)
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");

            var castle_exit_spawn_point = GetNode<Marker2D>("castle_exit_spawn");
            Player.Position = castle_exit_spawn_point.GlobalPosition;


            globalInstance.castle1_exited = false;
        }
        if (globalInstance.cave_exited)
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");

            var cave_exit_spawn_point = GetNode<Marker2D>("cave_exit_spawn");
            Player.Position = cave_exit_spawn_point.GlobalPosition;


            globalInstance.cave_exited = false;
        }
        if (globalInstance.ocean_exited)
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");
            GD.Print("this is not an error");
            var castle_exit_spawn_point = GetNode<Marker2D>("ocean_exit_spawn");
            Player.Position = castle_exit_spawn_point.GlobalPosition;

            //ca me parait faux ca
            globalInstance.ocean_exited = false;
        }
        
    }
    public async void _on_castle_entrance_1_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans castle zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/castle_scene1.tscn");
        }
    }

    public async void _on_cave_entrance_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans cave zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/cave.tscn");
        }
    }

    public async void _on_ocean_entrance_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans ocean zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/ocean.tscn");
        }
    } 
}
