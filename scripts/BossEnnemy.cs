using Godot;
using System;

public partial class BossEnnemy : CharacterBody2D
{
	[Export] public PackedScene fireball_scene;
    [Export] public PackedScene magic_circle_scene;
    [Export] public float shoot_interval = 2.5f;
    [Export] public NodePath animated_sprite_path;
    [Export] public NodePath timer_path;
    [Export] public NodePath fire_point_path;
    [Export] private CharacterBody2D Player;
    private Node2D player;

    private AnimatedSprite2D anim;
    private Timer timer;
    private Marker2D fire_point;

    private int fireball_count = 0;
    private const int fireballs_before_circle = 2;

    private bool player_attack = false;
    private bool can_take_damage = true;
    private bool is_dead = false;
    private bool is_taking_damage = false;
    private ProgressBar healthbar;

    public float health = 2000f;

    private bool player_inattack_zone = false;

    public override void _Ready()
    {
        GetNode<Timer>("take_damage_cooldown").Timeout += OnTakeDamageCooldownTimeout;
        healthbar = GetNode<ProgressBar>("healthbar");
        anim = GetNode<AnimatedSprite2D>(animated_sprite_path);
        timer = GetNode<Timer>(timer_path);
        fire_point = GetNodeOrNull<Marker2D>(fire_point_path);

        timer.WaitTime = shoot_interval;
        timer.Timeout += OnAttackTimeout;
        timer.Stop();
    }
    public override void _PhysicsProcess(double delta)
    {
        update_health();
        deal_with_damage();
        //if (is_dead || is_taking_damage)
        //{
            //Velocity = Vector2.Zero;
            
            //return;
        //}
        if (player != null)
        {        
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
            
        }
    }
    private void OnAttackTimeout()
    {
        if (fireball_count < fireballs_before_circle)
        {
            ShootFireball();
            fireball_count++;
        }
        else
        {
            SummonMagicCircle();
            fireball_count = 0; 
        }
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
        anim.Play("shoot");
        GD.Print("Circle spawned at: ", circle.GlobalPosition);
    }
    private void _on_area_2d_body_entered(Node2D Body)
    {
        if (Body == Player)
        {
            GD.Print("player entered bossssss");
            player = Body;
            player_attack = true;
            timer.Start();
        }
    }
    private void _on_area_2d_body_exited(Node2D Body)
    {
        if (Body == Player)
        {
            GD.Print("player exited the boss");
            player = Body;
            player_attack = false;
            timer.Stop();
        }
    }
    private void _on_boss_hitbox_body_entered(Node2D Body)
    {
        if (Body == Player)
        {
            player_inattack_zone = true;
        }
    }
    private void _on_boss_hitbox_body_exited(Node2D Body)
    {
        if (Body == Player)
        {
            player_inattack_zone = false;
        }
    }
    private void OnTakeDamageCooldownTimeout()
    {
        GD.Print("Boss can take damage again");
        can_take_damage = true;
    }
    public override void _Process(double delta)
    {
        if(player!=null)
        {
            if (Player.Position.Y > Position.Y)
            {
                ZIndex = 0;
            }
            else
            {
                ZIndex = 6;
            }
        }
        
    }
    private void update_health()
    {
        healthbar.Value = health;
        if (health >= 2000)
        {
            healthbar.Visible = false;
        }
        else
        {
            healthbar.Visible = true;
        }
    }
    private void deal_with_damage()
    {
        var globalInstance = GetNode<globall>("/root/Globall");
        if (player_inattack_zone && globalInstance.player_current_attack)
        {
            if (can_take_damage == true)
            {
                health -= globalInstance.player_damage;

                var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
                anim.Play("take_damage"); 
                is_taking_damage = true;
                anim.Play("IDLE");
                
                GetNode<Timer>("take_damage_cooldown").Start();
                can_take_damage = false;
                GD.Print("boss health = ", health);

                if (health <= 0 && !is_dead)
                {
                    is_dead = true;
                    var anim_death = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
                    anim_death.Play("death");
                    
                    globalInstance.AddXP(1000);                                      
                }
            }
        }
    }
    private void OnAnimationFinished()
    {
        var anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (anim.Animation == "knockback")
        {
            is_taking_damage = false;
            
            // remrt animation walk
            if (!is_dead)
            {
                anim.Play("IDLE");
            }
        }
        else if (anim.Animation == "death")
        {
            QueueFree();
        }
    }
}   