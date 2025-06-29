using Godot;
using System;

[GlobalClass]
public partial class InventoryItem : Resource
{
	[Export] public int Stacks = 1;

	public enum SlotType
	{
		RightHand,
		LeftHand,
		Potions,
		NotEquipable
	}

	[Export] public SlotType slotType = SlotType.NotEquipable;

	[Export] public RectangleShape2D GroundCollisionShape;
	[Export] public string Name = "";
	[Export] public Texture2D Texture;
	[Export] public Texture2D SideTexture;
	[Export] public int MaxStacks;
	[Export] public int Price;
}
