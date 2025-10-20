using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody2D
{
    [Export]
    private AnimatedSprite2D anim;

    private Vector2 Direction;
    private float Speed = 60;

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

    [Export] private Rock rock_node;

    private bool dash_in_progress = false; // dash in progress
    private float dash_distance = 50f;    // how far the dash goes
    private float dash_speed = 110f;       // speed during dash
    private Vector2 dash_direction;
    private Vector2 dash_target;
    private bool dash_on_cooldown = false;

    public override void _Ready()
    {
        anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GetNode<Timer>("attack_cooldown").Start();

        slashAnim = GetNode<AnimatedSprite2D>("AnimatedSprite2D2");//
        slashAnim.Visible = false;  // Start hidden

        // Connect the signal
        slashAnim.AnimationFinished += OnSlashAnimationFinished;

        globalInstance = GetNode<globall>("/root/Globall");//
        //UpdateHealth();
        HUD hud = GetTree().Root.GetNode<HUD>("HUD"); 
        if (hud != null)
            hud.UpdateHearts(health);
        else
            GD.PrintErr("HUD singleton not found!");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        Direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("MoveUp"))
        GD.Print("MoveUp pressed");

        UpdateHealth();
        base._PhysicsProcess(delta);

        Direction = Vector2.Zero;

    if (!dash_in_progress)
    {
        
        Direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Velocity = Direction * Speed;
        MoveAndSlide();
    }
    else
    {
        KinematicCollision2D collision = MoveAndCollide(dash_direction * dash_speed * (float)delta);

        if (collision != null)
        {
            // s'arrete si ca touche un mur
            dash_in_progress = false;
            Velocity = Vector2.Zero;
            Speed = 60;
        }

        // ca s'arrete quand ca a atteint le dash_target
        if ((dash_target - GlobalPosition).Dot(dash_direction) <= 0f)
        {
            GlobalPosition = dash_target;
            dash_in_progress = false;
            Velocity = Vector2.Zero;
            Speed = 60;
        }
    }
        
        

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
        if (dash_in_progress) return; 
        if (direction == Vector2.Zero)
        {
            anim.Play("idle");  
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


        if (Input.IsActionJustPressed("attack"))
        {
            slash();
        }
        if (Input.IsActionJustPressed("dash") && !dash_in_progress && !dash_on_cooldown)
        {
            start_dash();
            dash_on_cooldown = true;
            GetNode<Timer>("dash_cooldown_timer").Start();
        }
    }

    private void slash()
    {

        Vector2 mousePosition = GetGlobalMousePosition();
        Vector2 direction = mousePosition - GlobalPosition;
        float angle = direction.Angle();  // en radians

        angle += Mathf.Pi / 4; //tourne de 45 degre a droite 
        slashAnim.Rotation = angle;
        GD.Print(angle);

        globalInstance.player_current_attack = true;//
        GD.Print("attash slash");
        attack_ip = true;//

        slashAnim.Visible = true;
        slashAnim.Play("slash");

        if (angle >= -2.0f && angle <= 0.4f)
        {
            // Derrière le joueur
            slashAnim.ZIndex = ZIndex - 1;
        }
        else
        {
            // Devant le joueur
            slashAnim.ZIndex = ZIndex + 2;
        }
        Speed = 10;
        rock_node.hit();
    }
    private void start_dash()
    {
        Vector2 mouse_position = GetGlobalMousePosition();
        dash_direction = (mouse_position - GlobalPosition).Normalized();
        dash_target = GlobalPosition + dash_direction * dash_distance;

        dash_in_progress = true;
        Speed = 0; // arrete le mouvement de base 

       
        if (dash_direction.X >= 0)
            anim.Play("dash_right");
        else
            anim.Play("dash_left");
    }
    private void OnSlashAnimationFinished()
    {
        slashAnim.Visible = false;

        globalInstance.player_current_attack = false;//
        attack_ip = false;//
        Speed = 60;
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
        HUD hud = GetTree().Root.GetNode<HUD>("HUD");
        if (hud != null)
            hud.UpdateHearts(health);
        else
            GD.PrintErr("HUD singleton not found!");
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
    private void _on_dash_cooldown_timer_timeout()
    {
        dash_on_cooldown = false;
    }
    
}

 