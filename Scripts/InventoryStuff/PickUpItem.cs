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
	
	//Fancy stuff for making them a little more appealing
	AudioStreamPlayer PickUpSound;
	float FrameTime = 48;
	Vector2 NewOffset;
	float CountDown;
	
	//Item checks...
	//ItemChecker CheckItem;
	//bool PickedUp;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += ItemTouched;
		sprite = GetNode<Sprite2D>("Sprite2D");
		PickUpSound = GetNode<AudioStreamPlayer>("/root/GameScene/SFX");
		//CheckItem = GetNode<ItemChecker>("/root/ItemChecker");
		if (replacementPicture != null)
		{
			sprite.Texture = replacementPicture;
		}
		else
		{
			sprite.Texture = baseItem.icon;
			//sprite.Scale = new Vector2((float)0.25,(float)0.25);
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
		//This will be figured out later maybe
		//if (PickedUp == true)
		//{
			//this.QueueFree();
		//}

	}
	
	public override void _Process(double delta)
	{
		// MANUAL ANIMATIONS! (This is so dumb... But it works)
		CountDown -= 1;
		if (CountDown > (FrameTime/2))
		{
			// Resetting the Sprite to it's original Offset
			NewOffset.Y = 0;
			sprite.SetOffset(NewOffset);
		}
		else if (CountDown > 0)
		{
			// Moving the Sprite down 2 Pixels
			NewOffset.Y = 2;
			sprite.SetOffset(NewOffset);
		}
		else
		{
			// Resetting the FrameTime
			CountDown = FrameTime;
		}
	}

	private void ItemTouched(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			if (AddNewItem(amount))
			{
				RandomNoise();
				//GD.Print(this);
				//CheckItem.AddItemToCollected(this);
				PickUpSound.Play();
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
	
	private void RandomNoise()
	{
		int RandomNumber = (int)(GD.Randi() % 3);
		if (RandomNumber == 0)
		{
			PickUpSound.PitchScale = 1f;
		}
		if (RandomNumber == 1)
		{
			PickUpSound.PitchScale = 1.2f;
		}
		if (RandomNumber == 2)
		{
			PickUpSound.PitchScale = 0.8f;
		}
	}

}
