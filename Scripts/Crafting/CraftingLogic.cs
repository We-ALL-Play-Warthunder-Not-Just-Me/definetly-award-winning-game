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
	private int recipeID = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateItemlist();
		materialList.SendOverItem += SentItem;
		ItemClicked += OnItemClicked;
	}

	private void SentItem(Item item)
	{
		//why does this work and not making a shallowcopy doesn't????? 
		//It was bc the addtostackingfunction was setting the item.qty to 0
		//Item tempitem = item.shallowCopy();
		AddToCraftingMenu(item);
		//updateItemlist();
	}


	//Evil recipe validation
	private bool CheckValidRecipe()
	{
		bool isValid = false;
		foreach (var (recipeid,ingredientsList) in recipeList.recipes)
		{
			foreach (var (item, amount) in ingredientsList.ingredients)
			{
				// You have not all the ingredients of the recipe
				if (!craftingMenu.ContainsKey(item.ID))
				{
					isValid = false;
					break;
				}
			}
			foreach (var (id, amount) in craftingMenu)
			{
				//there is an ingredient that is not part of the recipe
				if (!ingredientsList.ingredients.ContainsKey(itemDatabase.items[id]))
				{
					isValid = false;
					break;
				}
				if (ingredientsList.ingredients.ContainsKey(itemDatabase.items[id]))
				{
					if (amount >= ingredientsList.ingredients[itemDatabase.items[id]])
					{
						isValid = true;
					}
				}
			}
			if (isValid)
			{
				recipeID = recipeid;
				break;
			}
			else
			{
				recipeID = 0;
			}
		}
		GD.Print($"valid recipe: {isValid}");
		return isValid;
	}

	//Copied from InventoryLogic and modified
	public bool AddToCraftingMenu(Item item)
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
				CheckValidRecipe();
				return couldPickup;
			}
		}
		// if there is no item in the bag yet
		if (!craftingMenu.ContainsKey(item.ID))
		{
			craftingMenu.Add(item.ID,item.qty);
			UpdateItemlist();
			CheckValidRecipe();
			return true;
		}

		return couldPickup;
	}
	public void UpdateItemlist()
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
		if(couldPickup) CheckValidRecipe();
		UpdateItemlist();
		return couldPickup;
	}

	// we don't have to be as 'careful' when removing items from the menu on the left in the crafting
	public void RemoveItemFromCrafting(int index)
	{
		if (index < 0 || index >= craftingMenu.Count) return;
		int id = (int)GetItemMetadata(index);
        craftingMenu.Remove(id);
        //RemoveItem(index);
		UpdateItemlist();
		CheckValidRecipe();
	}

	public Item GetItemFromCrafting(int index)
	{
		if (index < 0 || index >= craftingMenu.Count) return null;
		int id = (int)GetItemMetadata(index);
		Item temp = itemDatabase.items[id].shallowCopy();
		if (craftingMenu.ContainsKey(id))
		{
			temp.qty = craftingMenu[id];
		}
		return temp;
	}

	private void OnItemClicked(long index, Vector2 pos, long mousebuttonindex)
	{
		//Right click to take out from the menu
		if (mousebuttonindex == 2)
		{
			RemoveItemFromCrafting((int) index);
			GD.Print($"You took out an item");
		}
		// Left Click to add more items....
		else if (mousebuttonindex == 1)
		{
			Item tempitem = GetItemFromCrafting((int) index);
			tempitem.qty = 1;
			AddToCraftingMenu(tempitem);
		}
	}
	

}
