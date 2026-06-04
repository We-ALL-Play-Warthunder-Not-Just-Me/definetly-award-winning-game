using Godot;
using System;

public partial class AddItemTest : Button
{
	[Export] Mats inv;
	[Export] int id;
	[Export] string name;
	[Export] Texture2D itemIcon;
	[Export] int max_qty;
	[Export] int qty;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += AddNewItem;
	}

	private void AddNewItem()
	{
		Item item = new Item()
		{
			ID = id,
			Name = name,
			icon = itemIcon,
			max_qty = max_qty,
			qty = qty
		};

		inv.AddInventoryItem(item);
	}

}
