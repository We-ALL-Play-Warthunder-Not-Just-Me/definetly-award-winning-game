using Godot;
using System;

public partial class PickUpItem : Area2D
{

	[Export] Item baseItem;
	[Export] int amount;
	// Can be the Materials inventory itemlist or the KeyItems inventory Itemlist
	[Export] InventoryLogic invent;
	// [Export] bool MaterialInventory = true;
	[Export] bool KeyItemInventory = false;
	private Sprite2D sprite;
	[Export] Texture2D replacementPicture = null;
	// private Node globals;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += ItemTouched;
		sprite = GetNode<Sprite2D>("Sprite2D");
		if (replacementPicture != null)
		{
			sprite.Texture = replacementPicture;
		}
		else
		{
			sprite.Texture = baseItem.icon;
			sprite.Scale = new Vector2((float)0.25,(float)0.25);
		}
		// invent = GetNode<InventoryLogic>("../../../PauseStatus/InventoryScreen/Panel/TabContainer/Inventory/TabContainer/Materials/MarginContainer/ItemList");
		// invent = GetNode<InventoryLogic>("/root/GameScene/PauseStatus/InventoryScreen/Panel/TabContainer/Inventory/TabContainer/Materials/MarginContainer/ItemList");
		// invent = GetNode<InventoryLogic>("/root/GameScene/PauseStatus/InventoryScreen/Panel/TabContainer/Inventory/TabContainer/KeyItems/MarginContainer/ItemList");		
		// SendItemToMaterialInventory.Instance.EmitSignal(SignalName.SentItem,);
		// if (!KeyItemInventory)
		// {
		// 	globals = GetNode("/root/SendItemtoMaterialInventory") as SendItemToMaterialInventory;
		// 	// globals.EmitSignal(SendItemToMaterialInventory.SignalName.SentItem);			
		// }
		// else if (KeyItemInventory)
		// {
		// 	globals = GetNode("/root/SendItemtoKeyItemInventory") as SendItemToKeyItemInventory;
		// }
		if (!KeyItemInventory)
		{
			invent = GetNode<InventoryLogic>("/root/GameScene/PauseStatus/InventoryScreen/Panel/TabContainer/Inventory/TabContainer/Materials/MarginContainer/ItemList");
		}
		else if (KeyItemInventory)
		{
			invent = GetNode<InventoryLogic>("/root/GameScene/PauseStatus/InventoryScreen/Panel/TabContainer/Inventory/TabContainer/KeyItems/MarginContainer/ItemList");
		}

	}

	private void ItemTouched(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			if (AddNewItem(amount))
			{
				QueueFree();
			}
		}
	}

	private bool AddNewItem(int amount)
	{
		baseItem.qty = amount;
		Item item = baseItem;
		return invent.AddInventoryItem(item);
	}

}
