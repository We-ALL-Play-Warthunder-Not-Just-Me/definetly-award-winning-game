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
		// Initalizes the inventory of Size 20 filled with spaces and text so that it looks even
		items = new Item[InventorySize];
		for (int i = 0; i < InventorySize; i++)
		{
			AddItem(" ", blankicon);
		}
		
		ItemClicked += OnInventoryItemClicked;
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

		for (int i = 0; i < InventorySize; i++)
		{
			//Basically checks for an empty slot in the array to add an item
			if(items[i] != null) continue;

			items[i] = item;
			SetItemIcon(i,item.icon);

			if(item.max_qty > 1)
			{
				SetItemText(i, items[i].Name + " " + items[i].qty.ToString());
			}

			return true;
		}

		return couldPickup;
	}

	//Adds item to the stack, adds item to next inventory slot if the stack is full
	//Could change max stack behavior later on depending on what we want
	private bool AddStackableItem(Item item)
	{
		bool couldPickup = false;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] == null) continue;
			
			if (items[i].ID != item.ID || items[i].qty >= items[i].max_qty) continue;
		
			if (items[i].qty + item.qty > items[i].max_qty)
			{
				int AmtToRemove = items[i].max_qty - items[i].qty;

				items[i].qty = items[i].max_qty;
				item.qty = item.qty - AmtToRemove;

				couldPickup = true;
				SetItemText(i, items[i].Name + " " + items[i].qty.ToString());
				continue;
			}

			items[i].qty = item.qty + items[i].qty;
			item.qty = 0;
			SetItemText(i, items[i].Name + " " + items[i].qty.ToString());
			return true;
		}

		return couldPickup;
	}

	//YOU DROP THE ITEM
	public void RemoveInventoryItem(int index)
	{
		if (index < 0 || index >= InventorySize) return;

		items[index] = null;
		SetItemIcon(index, blankicon);
		SetItemText(index, " ");
	}

	public Item GetInventoryItem(int index)
	{
		if (index < 0 || index >= InventorySize) return null;

		return items[index];
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

}
