using Godot;
using System;

public partial class AddItemButton : Button
{
	[Export] InventoryLogic2 inv;
	// [Export] int id;
	// [Export] string name;
	// [Export] Texture2D itemIcon;
	// [Export] int max_qty;
	[Export] int qty;
	[Export] public Item itemData;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += AddNewItem;
	}

	private void AddNewItem()
	{
		if(itemData is Item item)
		{
			//Make the copy of the item to not change the resource
			Item tempitem = item.shallowCopy();
			tempitem.qty = qty;
			inv.AddInventoryItem(tempitem);
		}
	}

}
