using Godot;
using System;

public partial class globall : Node
{
	public bool player_current_attack = false;

	//systeme xp
    public int player_xp = 0;
    public int player_xp_max = 100;
    public int player_level = 1;
    public int player_damage = 20;   
    public int damage_per_level = 5; // a changer ?

    public void AddXP(int amount)
    {
        player_xp += amount;

        if (player_xp >= player_xp_max)
        {
            player_xp -= player_xp_max; // on garde le reste pouir apres
            LevelUp();
        }
    }

    private void LevelUp()
    {
        player_level++;
        player_damage += damage_per_level;
        GD.Print($"[LEVEL UP] Niveau {player_level}, Dégâts = {player_damage}");
    }

}
