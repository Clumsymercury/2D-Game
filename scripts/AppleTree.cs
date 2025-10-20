using Godot;
using System;

public partial class AppleTree : Node2D
{
    string state = "no apples";
    private bool player_in_area = false;

    [Export] private CharacterBody2D Player;
    private Node2D player;

    private PackedScene apple = GD.Load<PackedScene>("res://scenes/apple_collectable.tscn");

    public override void _Ready()
    {
        var growth_timer = GetNodeOrNull<Timer>("growth_timer");
        if (state == "no apples")
        {
            growth_timer.Start();
        }
    }
    public override void _Process(double delta)
    {
        var tree_anim = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        var growth_timer = GetNodeOrNull<Timer>("growth_timer");
        if (state == "no apples")
        {
            tree_anim.Play("no_apples");
        }
        if (state == "apples")
        {
            tree_anim.Play("apples");
            if (player_in_area)
            {
                if (Input.IsActionJustPressed("e"))
                {
                    state = "no apples";
                    drop_apple();
                }
            }
        }
    }
    private void _on_pick_area_body_entered(Node2D body)
    {
        if (body == Player)
        {
            GD.Print("player enterd tree");
            player = body;
            player_in_area = true;
        }
    }
    private void _on_pick_area_body_exited(Node2D body)
    {
        if (body == Player)
        {
            player = body;
            player_in_area = false;
        }
    }
    private void _on_growth_timer_timeout()
    {
        if (state == "no apples")
        {
            state = "apples";
        }
    }
    private async void drop_apple()
    {
        var growth_timer = GetNodeOrNull<Timer>("growth_timer");

        var apple_instance = apple.Instantiate() as Node2D;
        var marker = GetNode<Node2D>("Marker2D"); 
        
        apple_instance.GlobalPosition = marker.GlobalPosition;

        AddChild(apple_instance);
        await ToSignal(GetTree().CreateTimer(3f), "timeout");
        growth_timer.Start();
    }
}
