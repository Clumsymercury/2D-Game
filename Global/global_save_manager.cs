// File: Scripts/GlobalSaveManager.cs
/*
using Godot;
using System;
using GDict = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;

[GlobalClass]
public partial class GlobalSaveManager : Node
{
    private const string SAVE_DIR = "user://";
    private const string SAVE_FILE = "save.sav";

    [Signal] public delegate void GameLoadedEventHandler();
    [Signal] public delegate void GameSavedEventHandler();

    private GDict currentSave = new GDict
    {
        { "scene_path", "" },
        { "player", new GDict
            {
                { "hp", 1 },
                { "max_hp", 1 },
                { "pos_x", 0 },
                { "pos_y", 0 },
            }
        },
        { "items", new GArray() },
        { "persistence", new GArray() },
        { "quests", new GArray() },
    };

    public void SaveGame()
    {
        UpdatePlayerData();
        UpdateScenePath();

        var path = SAVE_DIR + SAVE_FILE;
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PushError($"SaveGame: cannot open {path} for writing.");
            return;
        }

        var json = Json.Stringify(currentSave);
        file.StoreLine(json);
        EmitSignal(SignalName.GameSaved);
    }

    public async void LoadGame()
    {
        var path = SAVE_DIR + SAVE_FILE;
        if (!FileAccess.FileExists(path))
        {
            GD.PushWarning($"LoadGame: save not found at {path}. Creating default save.");
            SaveGame();
            return;
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PushError($"LoadGame: cannot open {path} for reading.");
            return;
        }

        var json = new Json();
        var parseErr = json.Parse(file.GetLine());
        if (parseErr != Error.Ok)
        {
            GD.PushError($"LoadGame: JSON parse error: {parseErr}");
            return;
        }

        currentSave = (GDict)json.Data;

        var levelManager = GetNode("/root/LevelManager");
        levelManager.Call("load_new_level",
            (string)currentSave["scene_path"],
            "",
            Vector2.Zero
        );

        await ToSignal(levelManager, "level_load_started");

        var playerManager = GetNode("/root/PlayerManager");
        var pDict = (GDict)currentSave["player"];
        var pos = new Vector2(
            Convert.ToSingle(pDict["pos_x"]),
            Convert.ToSingle(pDict["pos_y"])
        );
        playerManager.Call("set_player_position", pos);
        playerManager.Call("set_health",
            Convert.ToInt32(pDict["hp"]),
            Convert.ToInt32(pDict["max_hp"])
        );

        await ToSignal(levelManager, "level_loaded");
        EmitSignal(SignalName.GameLoaded);
    }

    private void UpdatePlayerData()
    {
        var playerManager = GetNode("/root/PlayerManager");

        Variant v = playerManager.Get("player");
        Node player = null;
        if (v.VariantType == Variant.Type.Object)
        {
            // why: Variant generic helper may not exist in your build; cast from GodotObject instead
            var gobj = v.AsGodotObject();
            player = gobj as Node;
        }

        if (player == null)
        {
            GD.PushWarning("UpdatePlayerData: PlayerManager.player not available.");
            return;
        }

        var playerDict = (GDict)currentSave["player"];
        playerDict["hp"] = Convert.ToInt32(player.Get("hp"));
        playerDict["max_hp"] = Convert.ToInt32(player.Get("max_hp"));

        var gpos = (Vector2)player.Get("global_position");
        playerDict["pos_x"] = gpos.X;
        playerDict["pos_y"] = gpos.Y;
    }

    private void UpdateScenePath()
    {
        currentSave["scene_path"] = GetTree().CurrentScene?.SceneFilePath ?? "";
    }
}
*/