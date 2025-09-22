using Godot;
using System.Collections.Generic;

public partial class Dialogue : Control
{
    [Signal]
    public delegate void dialogue_finishedEventHandler();


    [Export(PropertyHint.File, "*.json")]
    private string d_file; 

    private List<Godot.Collections.Dictionary> dialogue;
    private int current_dialogue_id = -1;

    public bool d_active = false;

    public override void _Ready()
    {
        GetNode<NinePatchRect>("NinePatchRect").Visible = false;
    }

    public void Start()
    {
        if (d_active)
        {
            return;
        }
        GD.Print("oui");
        d_active = true;//si active 
        GetNode<NinePatchRect>("NinePatchRect").Visible = true;
        dialogue = LoadDialogue();
        current_dialogue_id = -1;
        NextScript();
    }

    private List<Godot.Collections.Dictionary> LoadDialogue()
    {
        using var file = FileAccess.Open(d_file, FileAccess.ModeFlags.Read);
        var content = file.GetAsText();

        var json = new Json();
        var error = json.Parse(content);

        if (error != Error.Ok)
        {
            GD.PrintErr("Erreur parsing JSON: ", json.GetErrorMessage());
            return new List<Godot.Collections.Dictionary>();
        }

        // Le JSON est un Array → on le convertit en liste de Dictionary
        var array = (Godot.Collections.Array)json.Data;
        var list = new List<Godot.Collections.Dictionary>();

        foreach (Godot.Collections.Dictionary entry in array)
        {
            list.Add(entry);
        }

        return list;
    }

    public override void _Input(InputEvent @event)
    {
        if (!d_active)
        {
            return;
        }
        if (@event.IsActionPressed("ui_accept"))
            {
                NextScript();
            }
    }

    private void NextScript()
    {
        current_dialogue_id++;

        if (current_dialogue_id >= dialogue.Count)
        {
            GetNode<NinePatchRect>("NinePatchRect").Visible = false;
            d_active = false;
            EmitSignal("dialogue_finished");
            return;
            
        }
            

        var line = dialogue[current_dialogue_id];

        // Mise à jour des RichTextLabel
        GetNode<RichTextLabel>("NinePatchRect/Name").Text = line["name"].ToString();
        GetNode<RichTextLabel>("NinePatchRect/Text").Text = line["text"].ToString();
    }
}