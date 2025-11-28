/*using Godot;
using System;
using System.Collections.Generic;

public partial class InvUi : Control
{
	[Export]
    public Resource inv_resource; // assign playerinv.tres here

    private Node[] slots;
	private bool is_open = false;
	
	public override void _Ready()
	{
		slots = GetNode("NinePatchRect/GridContainer").GetChildren().ToArray<Node>();
        update_slots();
		close();
	}
    public override void _Process(double delta)
    {
		if (Input.IsActionJustPressed("inventory"))
		{
			GD.Print("inventory opened/closed");
			if (is_open)
			{
				close();
			}
			else
            {
				open();
            }
        }

    }

	public void open()
	{
		Visible = true;
		is_open = true;
	}
	public void close()
	{
		Visible = false;
		is_open = false;
	}
	public void update_slots()
    {
        if (inv_resource == null)
            return;

        // Cast the resource at runtime
        var inv = inv_resource as Godot.Object;

        if (inv == null)
            return;

        // Try to get the item array from the resource
        // Using Call() to access GDScript property
        var items = inv.Call("get", "item") as Godot.Collections.Array;

        if (items == null)
            return;

        int count = Math.Min(items.Count, slots.Length);

        for (int i = 0; i < count; i++)
        {
            // Call the "update" method on each slot node
            slots[i].Call("update_item", items[i]);
        }
    }
}*/
