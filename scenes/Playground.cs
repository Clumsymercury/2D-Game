using Godot;
using System; 

//classe principale de la scene Playground
public partial class Playground : Node2D
{
    
    [Export] private CharacterBody2D Player;
    private Node2D player;
    private AnimationPlayer _Scene_transition_animation;
    
//Méthode appelée authomatiquement au chargement de la scène
    public async override void _Ready()
    {
        var globalInstance = GetNode<globall>("/root/Globall");
        

        _Scene_transition_animation = GetNode<AnimationPlayer>("Scene_transition_animation/AnimationPlayer");

        if (globalInstance.castle1_exited)//fonction qui s'execute lorsque le joueur rentre en colision avec la collision 2D de la scene castle
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");//trasnition de scene pour rendre un effet plus fluide

            var castle_exit_spawn_point = GetNode<Marker2D>("castle_exit_spawn");//prend le marker castle exit spawn pour savoir l'endroit ou le joueur va apparaitre
            Player.Position = castle_exit_spawn_point.GlobalPosition;


            globalInstance.castle1_exited = false;
        }
        if (globalInstance.cave_exited) //fonction qui s'execute lorsque le joueur rentre en colision avec la collision 2D de la scene cave
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");//trasnition de scene pour rendre un effet plus fluide

            var cave_exit_spawn_point = GetNode<Marker2D>("cave_exit_spawn");//prend le marker castle exit spawn pour savoir l'endroit ou le joueur va apparaitre
            Player.Position = cave_exit_spawn_point.GlobalPosition;


            globalInstance.cave_exited = false;
        }
        if (globalInstance.ocean_exited) //fonction qui s'execute lorsque le joueur rentre en colision avec la collision 2D de la scene ocean
        {
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            _Scene_transition_animation.Play("fade_out");//trasnition de scene pour rendre un effet plus fluide
            GD.Print("this is not an error");
            var castle_exit_spawn_point = GetNode<Marker2D>("ocean_exit_spawn");//prend le marker castle exit spawn pour savoir l'endroit ou le joueur va apparaitre
            Player.Position = castle_exit_spawn_point.GlobalPosition;

            
            globalInstance.ocean_exited = false;
        }
        
    }
    public async void _on_castle_entrance_1_body_entered(Node2D body)//fonction qui s'execute lorsque le joueur rentre en colision avec la collision 2D de la scene castle pour rentrer dans cette scene
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans castle zone");

            _Scene_transition_animation.Play("fade_in");//trasnition de scene pour rendre un effet plus fluide

            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
            GetTree().ChangeSceneToFile("res://scenes/castle_scene1.tscn");//changement de scene, le character passe de la scene Playground a castle
        }
    }

    public async void _on_cave_entrance_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans cave zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");//trasnition de scene pour rendre un effet plus fluide

            GetTree().ChangeSceneToFile("res://scenes/cave.tscn");//changement de scene, le character passe de la scene Playground a cave
        }
    }

    public async void _on_ocean_entrance_body_entered(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            GD.Print("joueur dans ocean zone");

            _Scene_transition_animation.Play("fade_in");
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");//trasnition de scene pour rendre un effet plus fluide

            GetTree().ChangeSceneToFile("res://scenes/ocean.tscn");//changement de scene, le character passe de la scene Playground a ocean
        }
    } 
}
