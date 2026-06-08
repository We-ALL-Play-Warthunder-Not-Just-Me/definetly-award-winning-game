using Godot;
using System;
using System.Collections;
using System.Data.Common;

public partial class InventoryLogic : ItemList
{
	[Export] private ItemDictionary itemDatabase;
	[Export] public InventoryStorage inventory;
	// [Export] private Resource itemDatabase;
	// [Export] public Resource inventory;
	//[Export] int InventorySize = 20;
	//[Export] Texture2D blankicon;
	//private Item[] items;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateItemlist();
		ItemClicked += OnInventoryItemClicked;
		inventory.Changed += UpdateItemlist;
	}

	public void UpdateItemlist()
	{
		Clear();
		foreach (var (id,amount) in inventory.inventory)
		{
			if (itemDatabase.items[id].max_qty == 1)
			{
				int i = AddItem(itemDatabase.items[id].Name, itemDatabase.items[id].icon);
				SetItemMetadata(i, itemDatabase.items[id].ID);
			}
			else
			{
				int i = AddItem($"{itemDatabase.items[id].Name} : {amount}", itemDatabase.items[id].icon);
				SetItemMetadata(i, itemDatabase.items[id].ID);
			}
		}
		ResourceSaver.Save(inventory);
	}
	public bool AddInventoryItem(Item item)
	{
		if (item == null || item.qty <= 0)
		{
			return false;
		}

		//if the item is stackable
		bool couldPickup = AddStackableItem(item);

		// There is an item there, it used to be there soooo true (because it was stackable)
		if (item.qty == 0) return true;

		if (inventory.inventory.ContainsKey(item.ID))
		{	// we kinda are already checking for this in AddStackable
			if (inventory.inventory[item.ID] >= item.max_qty)
			{
				//GD.Print($"Your inventory is full of {item.Name}");
				return couldPickup;
			}
		}
		// if there is no item in the bag yet
		if (!inventory.inventory.ContainsKey(item.ID))
		{
			inventory.inventory.Add(item.ID,item.qty);
			//UpdateItemlist();
			inventory.EmitChanged();
			return true;
		}

		return couldPickup;
	}

	//Adds item to the stack, adds item to next inventory slot if the stack is full
	//Could change max stack behavior later on depending on what we want
	private bool AddStackableItem(Item item)
	{
		bool couldPickup = false;
		if (inventory.inventory.ContainsKey(item.ID))
		{
			// Your BAG IS TOO FULL GOOBER
			if (inventory.inventory[item.ID] >= item.max_qty)
			{
				GD.Print($"Your inventory is full of {item.Name}");
				return couldPickup;
			}
			// When you can't pick up ALL the item
			if (inventory.inventory[item.ID] + item.qty > item.max_qty)
			{
				int AmtToRemove = item.max_qty - inventory.inventory[item.ID];
				inventory.inventory[item.ID] = item.max_qty;
				item.qty -= AmtToRemove;
				couldPickup = true;
			}
			//Pick up ALL the item and add it to your bag
			if (inventory.inventory[item.ID] + item.qty <= item.max_qty)
			{
				inventory.inventory[item.ID] = item.qty + inventory.inventory[item.ID];
				item.qty = 0;
				couldPickup = true;
			}
		}
		//UpdateItemlist();
		inventory.EmitChanged();
		return couldPickup;
	}

	//YOU DROP THE ITEM
	public void RemoveInventoryItem(int index)
	{
		if (index < 0 || index >= inventory.inventory.Count) return;
		// When the item is important don't get rid of it!!!!!!
		int id = (int)GetItemMetadata(index);
		if (itemDatabase.items[id].important == true)
		{
			GD.Print("This is an important item!!");
			return;
		}
        inventory.inventory.Remove(id);
        RemoveItem(index);
		UpdateItemlist();
	}

	public Item GetInventoryItem(int index)
	{
		if (index < 0 || index >= inventory.inventory.Count) return null;
		int id = (int)GetItemMetadata(index);
		Item temp = itemDatabase.items[id].shallowCopy();
		if (inventory.inventory.ContainsKey(id))
		{
			temp.qty = inventory.inventory[id];
		}
		return temp;
	}

	private void OnInventoryItemClicked(long index, Vector2 pos, long mousebuttonindex)
	{
		if (mousebuttonindex == 2)
		{
			Item item = GetInventoryItem((int)index);

			if (item == null)
			{
				GD.Print("No item here");
				return;
			}

			RemoveInventoryItem((int)index);
			GD.Print($"You dropped {item.qty} of {item.Name}");
		}

		else if (mousebuttonindex == 1)
		{
			Item item = GetInventoryItem((int)index);
			if (item == null)
			{
				GD.Print("No item here");
				return;
			}
			GD.Print($"You Clicked {item.Name} there are {item.qty}");
		}
	}

	[Signal]
	public delegate void sentItemDataEventHandler(Item item);

	private void _on_item_selected(int index)
	{
		Item item = GetInventoryItem(index);
		EmitSignal(SignalName.sentItemData, item);
	}


}
