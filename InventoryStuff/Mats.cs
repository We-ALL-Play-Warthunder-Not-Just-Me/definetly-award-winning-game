using Godot;
using System;

public partial class Mats : ItemList
{
	[Export] int InventorySize = 20;
	[Export] Texture2D blankicon;
	private Item[] items;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		items = new Item[InventorySize];
		for (int i = 0; i < InventorySize; i++)
		{
			AddItem(" ", blankicon);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
