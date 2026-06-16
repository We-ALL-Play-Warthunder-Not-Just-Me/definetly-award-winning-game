using Godot;
using System;

public partial class ItemChecker : Node
{
	[Export]
	Godot.Collections.Array ItemsCollected;
	
	public void AddItemToCollected(Area2D item)
	{
		GD.Print("Before: " + ItemsCollected);
		GD.Print(item);
		ItemsCollected.Add(item);
		GD.Print("After: " + ItemsCollected);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ItemsCollected = [];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}
