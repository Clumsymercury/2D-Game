using Godot;

[Tool]
public partial class LevelTransition : Area2D
{
    // Exports a file picker for *.tscn files in the Inspector
    [Export(PropertyHint.File, "*.tscn")]
    public string Level { get; set; } = "";

    // Exports a string with default value "LevelTransition"
    [Export]
    public string TargetTransitionArea { get; set; } = "LevelTransition";

	// ---------- Collision Area Settings ----------
	// How many tiles long the gate is (min 1)
	private int _size = 2;

	[Export(PropertyHint.Range, "1,256,1,or_greater")]
	public int Size
	{
		get => _size;
		set
		{
			_size = Mathf.Max(1, value);
			UpdateArea();
		}
	}

	private Side _side = Side.Left;

	[Export]
	public Side TransitionSide
	{
		get => _side;
		set
		{
			_side = value;
			UpdateArea();
		}
	}

	private bool _snapToGrid = false;

	[Export]
	public bool SnapToGrid
	{
		get => _snapToGrid;
		set
		{
			_snapToGrid = value;
			if (_snapToGrid) SnapToGridNow();
		}
	}

	// Grid/tile constants (adjust if your tiles aren’t 16/32)
	private const int Grid = 16;  // grid step for snapping
	private const int Tile = 16;  // base tile size for the trigger rectangle

	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (_collisionShape == null)
		{
			// Create one if missing (helps when adding the script to a fresh node)
			_collisionShape = new CollisionShape2D();
			AddChild(_collisionShape);
		}

		// Ensure it has a RectangleShape2D
		if (_collisionShape.Shape == null || _collisionShape.Shape is not RectangleShape2D)
			_collisionShape.Shape = new RectangleShape2D();

		UpdateArea();

		// Connect body-entered for runtime (don’t run in editor)
		if (!Engine.IsEditorHint())
			BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// TODO: Replace with your own player check / level manager call
		// Example:
		// if (body.IsInGroup("player")) { GetNode<LevelManager>("/root/LevelManager").GoTo(LevelPath, TargetTransitionArea); }
		// For now, do nothing:
		// GD.Print("Player entered transition: ", Name);
	}

	private void UpdateArea()
	{
		// Make sure we have a RectangleShape2D
		if (_collisionShape == null)
			_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (_collisionShape == null)
			return;

		if (_collisionShape.Shape == null || _collisionShape.Shape is not RectangleShape2D)
			_collisionShape.Shape = new RectangleShape2D();

		var rect = (RectangleShape2D)_collisionShape.Shape;

		// Build a rectangle centered on the shape position. Godot’s RectangleShape2D.Size is full width/height.
		Vector2 newSize = new Vector2(Tile, Tile);
		Vector2 newLocalPos = Vector2.Zero;

		switch (_side)
		{
			case Side.Top:
				newSize.X *= _size;     // width spans multiple tiles horizontally
				newLocalPos.Y -= Grid;  // nudge 1/2 tile up
				break;

			case Side.Bottom:
				newSize.X *= _size;
				newLocalPos.Y += Grid;  // nudge 1/2 tile down
				break;

			case Side.Left:
				newSize.Y *= _size;     // height spans multiple tiles vertically
				newLocalPos.X -= Grid;  // nudge 1/2 tile left
				break;

			case Side.Right:
				newSize.Y *= _size;
				newLocalPos.X += Grid;  // nudge 1/2 tile right
				break;
		}

		rect.Size = newSize;
		_collisionShape.Position = newLocalPos;

		// Editor refresh
		if (Engine.IsEditorHint())
			QueueRedraw();
	}

	private void SnapToGridNow()
	{
		var pos = Position;
		pos.X = Mathf.Round(pos.X / Grid) * Grid;
		pos.Y = Mathf.Round(pos.Y / Grid) * Grid;
		Position = pos;
	}

	// Useful if you tweak values live in the Inspector in editor
	public override void _Notification(int what)
	{
		// Only in editor, refresh when an exported property changes
		
	}
}