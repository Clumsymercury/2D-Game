// File: Scripts/PauseController.cs
using Godot;

public partial class PauseController : Node
{
    /// <summary>Path to your PauseMenu scene (CanvasLayer or Control root).</summary>
    [Export(PropertyHint.File, "*.tscn")]
    public string PauseMenuScenePath { get; set; } = "res://UI/PauseMenu.tscn";

    private CanvasLayer _pauseMenu; // instance kept while open

    public override void _Ready()
    {
        // Must process even when game is paused so Esc can close the menu.
        ProcessMode = Node.ProcessModeEnum.Always;
        GD.Print("[PauseController] Ready. Press ESC to toggle PauseMenu.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause")) // Input Map must contain "pause" → Esc
        {
            if (_pauseMenu == null) ShowMenu();
            else HideMenu();

            GetViewport().SetInputAsHandled();
        }
    }

    private void ShowMenu()
    {
        var packed = ResourceLoader.Load<PackedScene>(PauseMenuScenePath);
        if (packed == null)
        {
            GD.PushError($"PauseController: Couldn't load scene at {PauseMenuScenePath}");
            return;
        }

        _pauseMenu = packed.Instantiate<CanvasLayer>();

        // Ensure the UI still runs while paused (so Esc and UI can work).
        SetProcessModeRecursive(_pauseMenu, Node.ProcessModeEnum.Always);

        // Put it at the top of the scene tree (overlay).
        GetTree().Root.AddChild(_pauseMenu);

        GetTree().Paused = true;
        GD.Print("[PauseController] Menu shown, tree paused.");
    }

    private void HideMenu()
    {
        GetTree().Paused = false;

        if (_pauseMenu != null)
        {
            _pauseMenu.QueueFree();
            _pauseMenu = null;
        }

        GD.Print("[PauseController] Menu hidden, tree unpaused.");
    }

    private static void SetProcessModeRecursive(Node node, Node.ProcessModeEnum mode)
    {
        node.ProcessMode = mode;
        foreach (var child in node.GetChildren())
            if (child is Node n) SetProcessModeRecursive(n, mode);
    }
}
