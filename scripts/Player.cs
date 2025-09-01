using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody2D
{
    [Export]
    private AnimatedSprite2D anim;

    private Vector2 Direction;
    private float Speed = 100;

    private AnimationPlayer animationPlayer;

    private bool enemy_inattack_range = false;
    private bool enemy_attack_cooldown = false;
    public int health = 6;
    private int max_health = 6;
    private bool player_alive = true;

    private bool attack_ip = false; //ip = in progress (pour alex :) )

    private string lastAnimation = "walk_down_right";
    private AnimatedSprite2D slashAnim;//

    private globall globalInstance;//

    private HUD _hud;
    public override void _Ready()
    {


        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GetNode<Timer>("attack_cooldown").Start();

        slashAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D2");//
        slashAnim.Visible = false;  // Start hidden

        // Connect the signal
        slashAnim.AnimationFinished += OnSlashAnimationFinished;

        globalInstance = GetNode<globall>("/root/Globall");//
        UpdateHealth();
        _hud = GetTree().Root.GetNode<HUD>("Playground/HUD");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        Direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    }

    public override void _PhysicsProcess(double delta)
    {

        UpdateHealth();
        base._PhysicsProcess(delta);

        Direction = Vector2.Zero;

        if (Input.IsActionPressed("ui_right"))
            Direction.X += 1;
        if (Input.IsActionPressed("ui_left"))
            Direction.X -= 1;
        if (Input.IsActionPressed("ui_down"))
            Direction.Y += 1;
        if (Input.IsActionPressed("ui_up"))
            Direction.Y -= 1;

        Direction = Direction.Normalized();
        Velocity = Direction * Speed;
        MoveAndSlide();

        UpdateAnimation(Direction);
        ennemy_attack();

        if (health <= 0)
        {
            player_alive = false;
            health = 0;
            GD.Print("player is dead");
            QueueFree();
        }
    }



    private void UpdateAnimation(Vector2 direction)
    {
        if (direction == Vector2.Zero)
        {
            anim.Play("idle");  // Ensure animation name is lowercase "idle"
            return;
        }

        string animName = "";

        if (direction.X < 0 && direction.Y < 0)
            animName = "walk_up_left";
        else if (direction.X > 0 && direction.Y < 0)
            animName = "walk_up_right";
        else if (direction.X < 0 && direction.Y > 0)
            animName = "walk_down_left";
        else if (direction.X > 0 && direction.Y > 0)
            animName = "walk_down_right";
        else
        {
            animName = lastAnimation; // fallback to last facing direction
        }

        if (anim.Animation != animName)
        {
            anim.Play(animName);
            lastAnimation = animName;
        }
    }

    public override void _Process(double delta)
    {


        if (Input.IsActionJustPressed("attack")) // Replace with your input action
        {
            slash();
        }
    }

    private void slash()
    {
        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 direction = mousePosition - GlobalPosition;
        float angle = direction.Angle();  // en radians

        angle += Mathf.Pi / 4; //tourne de 45 degre a droite 
        slashAnim.Rotation = angle;

        globalInstance.player_current_attack = true;//
        GD.Print("attash slash");
        attack_ip = true;//

        slashAnim.Visible = true;
        slashAnim.Play("slash");
    }

    private void OnSlashAnimationFinished()
    {
        slashAnim.Visible = false;

        globalInstance.player_current_attack = false;//
        attack_ip = false;//
    }

    public void _on_player_hitbox_body_entered(Node2D body)
    {
        if (body.HasMethod("ennemy"))
        {
            GD.Print("inattack=true");
            enemy_inattack_range = true;
        }
    }

    public void _on_player_hitbox_body_exited(Node2D body)
    {
        if (body.HasMethod("ennemy"))
        {
            GD.Print("inattack=false");
            enemy_inattack_range = false;
        }
    }

    public void _on_attack_cooldown_timeout()
    {
        enemy_attack_cooldown = true;
    }



    public void ennemy_attack()
    {
        if (enemy_inattack_range && enemy_attack_cooldown)
        {
            health = health - 1;
            enemy_attack_cooldown = false;
            GetNode<Timer>("attack_cooldown").Start();
            GD.Print(health);

            UpdateHealth();
        }
    }

    public void player()
    {

    }

    public void UpdateHealth()
    {
        if (_hud == null) return;
        _hud.UpdateHearts(health);
    }

    private void _on_regin_timer_timeout()
    {
        if (health < 6)
        {
            health += 1;
            if (health > 6)
                health = 6;
        }

        if (health <= 0)
            health = 0;
    }
    
}

 