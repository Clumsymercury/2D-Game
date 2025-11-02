using Godot;
using System;

public partial class HUD : CanvasLayer
{
	private TextureRect[] hearts;
	private Texture2D full_heart;
	private Texture2D half_heart;
	private Texture2D empty_heart;

	private ProgressBar xp_bar;
	private Godot.Label xp_label;
	
	// _Ready est appelé quand le node rentre dans scene our la prmiere fois
	public override void _Ready()
	{
		var container = GetNode<HBoxContainer>("Control/Hearts");
		hearts = new TextureRect[container.GetChildCount()];

		for (int i = 0; i < container.GetChildCount(); i++)
		{
			hearts[i] = container.GetChild<TextureRect>(i);
		}


		full_heart = GD.Load<Texture2D>("res://Assets/gfx/Pixel Heart Sprite full.png");
		half_heart = GD.Load<Texture2D>("res://Assets/gfx/Pixel Heart Sprite half.png");
		empty_heart = GD.Load<Texture2D>("res://Assets/gfx/Pixel Heart Sprite empty.png");

		xp_bar = GetNode<ProgressBar>("XPBar");
		xp_label = xp_bar.GetNode<Godot.Label>("Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var globalInstance = GetNode<globall>("/root/Globall");

        xp_bar.MaxValue = globalInstance.player_xp_max;
        xp_bar.Value = globalInstance.player_xp;

        xp_label.Text = $"XP: {globalInstance.player_level}";
	}
	public void UpdateHearts(int currentHealth)
	{
		for (int i = 0; i < hearts.Length; i++)
		{
			int heartHealth = currentHealth - (i * 2);

			if (heartHealth >= 2)
				hearts[i].Texture = full_heart;
			else if (heartHealth == 1)
				hearts[i].Texture = half_heart;
			else
				hearts[i].Texture = empty_heart;
		}
	
	}
	
}
