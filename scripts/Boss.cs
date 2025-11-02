using Godot;
using System;

public partial class Boss : CharacterBody2D
{
	[Export] public PackedScene fireball_scene;
    [Export] public PackedScene magic_circle_scene;
    [Export] public float shoot_interval = 2.5f;
    [Export] public NodePath animated_sprite_path;
    [Export] public NodePath timer_path;
    [Export] public NodePath fire_point_path;

    private AnimatedSprite2D anim;
    private Timer timer;
    private Marker2D fire_point;
    private bool use_fireball = true;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>(animated_sprite_path);
        timer = GetNode<Timer>(timer_path);
        fire_point = GetNodeOrNull<Marker2D>(fire_point_path);

        timer.WaitTime = shoot_interval;
        timer.Timeout += OnAttackTimeout;
        timer.Start();
    }
    private void OnAttackTimeout()
    {
        if (use_fireball)
            ShootFireball();
        else
            SummonMagicCircle();

        use_fireball = !use_fireball;
    }

    private void ShootFireball()
    {
        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        if (player == null)
            return;

        anim.Play("shoot");

        var fireball = fireball_scene.Instantiate<Fireball>();
        GetParent().AddChild(fireball);

        Vector2 spawn_pos = fire_point != null ? fire_point.GlobalPosition : GlobalPosition;
        fireball.GlobalPosition = spawn_pos;

        Vector2 dir = (player.GlobalPosition - spawn_pos).Normalized();
        fireball.direction = dir;

        //tourne vers le joueur
        fireball.Rotation = dir.Angle();
    }
    private void SummonMagicCircle()
    {
        var player = GetTree().GetFirstNodeInGroup("player") as Player;
        var circle = magic_circle_scene.Instantiate<MagicCircle>();
        GetParent().AddChild(circle);
        circle.GlobalPosition = player.GlobalPosition; // spawn at player’s feet
        GD.Print("Circle spawned at: ", circle.GlobalPosition);
    }
}
