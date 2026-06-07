using Godot;
using System;

public partial class CraftingLogic : ItemList
{
	[Export] ItemDictionary itemDatabase;
	[Export] InventoryStorage materialInventory;
	[Export] InventoryStorage keyItemInventory;
	[Export] RecipesList recipeList;
	[Export] CraftingMaterial materialList;
	private Godot.Collections.Dictionary<int,int> craftingMenu = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		updateItemlist();
		materialList.SendOverItem += SentItem;
	}

	private void SentItem(Item item)
	{
		//why does this work and not making a shallowcopy doesn't????? 
		//Item tempitem = item.shallowCopy();
		addToCraftingMenu(item);
		//updateItemlist();
	}

	//Copied from InventoryLogic and modified
	public bool addToCraftingMenu(Item item)
	{
		if (item == null || item.qty <= 0)
		{
			return false;
		}

		//if the item is stackable
		bool couldPickup = AddStackableItem(item);

		// There is an item there, it used to be there soooo true (because it was stackable)
		if (item.qty == 0) return true;

		if (craftingMenu.ContainsKey(item.ID))
		{	// we kinda are already checking for this in AddStackable
			if (craftingMenu[item.ID] >= materialInventory.inventory[item.ID])
			{
				GD.Print($"You can't put more of {item.Name}!!!");
				return couldPickup;
			}
		}
		// if there is no item in the bag yet
		if (!craftingMenu.ContainsKey(item.ID))
		{
			craftingMenu.Add(item.ID,item.qty);
			updateItemlist();
			return true;
		}

		return couldPickup;
	}
	public void updateItemlist()
	{
		Clear();
		foreach (var (id,amount) in craftingMenu)
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
	}

	private bool AddStackableItem(Item item)
	{
		bool couldPickup = false;
		if (craftingMenu.ContainsKey(item.ID))
		{
			// When you try to add more than you actually have
			if (craftingMenu[item.ID] >= materialInventory.inventory[item.ID])
			{
				GD.Print($"You can't put more {item.Name}!!!");
				return couldPickup;
			}
			//Pick up ALL the item and add it
			if (craftingMenu[item.ID] + item.qty <= materialInventory.inventory[item.ID])
			{
				craftingMenu[item.ID] = item.qty + craftingMenu[item.ID];
				couldPickup = true;
			}
		}
		updateItemlist();
		return couldPickup;
	}


}
