using Godot;
using System;

public partial class Sprite2d : Node2D // Class name matches the file name
{
    public override void _Ready()
    {
        GD.Print("Sprite2d is ready.");
    }
}
