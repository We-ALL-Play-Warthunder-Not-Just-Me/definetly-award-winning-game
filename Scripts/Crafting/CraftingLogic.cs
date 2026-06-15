using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class CraftingLogic : ItemList
{
	[Export] ItemDictionary itemDatabase;
	[Export] InventoryLogic materialInventory;
	[Export] InventoryLogic keyItemInventory;
	[Export] RecipesList recipeList;
	[Export] CraftingMaterial materialList;
	[Export] Texture2D BlankIcon;
	[Export] TextureButton craftButton;
	private Godot.Collections.Dictionary<int, int> craftingMenu = [];
	private int recipeID = 0;
	[Export] RecipeListDropDown RecipeListdropdown;

	[Signal]
	public delegate void recipePictureEventHandler(Texture2D icon);

	private void RecipePicture()
	{
		Texture2D icon;
		if (recipeID == 0) icon = BlankIcon;
		else icon = itemDatabase.items[recipeID].icon;
		EmitSignal(SignalName.recipePicture, icon);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateItemlist();
		materialList.SendOverItem += SentItem;
		ItemClicked += OnItemClicked;
		craftButton.Pressed += CraftItem;
		RecipeListdropdown.SentItem += dropdownRecipe;
	}

	private void dropdownRecipe(Item item, int amount)
	{
		//Item temp = item.shallowCopy(amount);
		item.qty = amount;
		AddToCraftingMenu(item);
	}

	private void CraftItem()
	{
		if (!CheckValidRecipe()) return;
		Item temp = itemDatabase.items[recipeID].Copy(1);
		// temp.qty = 1;
		materialInventory.AddInventoryItem(temp);
		ConsumeIngredients();
	}

	private void ConsumeIngredients()
	{
		if (HasCatalyst() != 0)
		{
			CatalystConsumeIngredient();
			return;
		}
		foreach (var (item, amount) in recipeList.recipes[recipeID].ingredients)
		{
			//Non-consumable items are reusable, if it's consumable we use them up
			if (itemDatabase.items[item.ID].consumable)
			{
				materialInventory.RemoveAmountofItem(item.ID, amount);
				craftingMenu[item.ID] -= amount;
			}
			if (craftingMenu[item.ID] == 0) craftingMenu.Remove(item.ID);
		}

		UpdateItemlist();
		CheckValidRecipe();
	}

	private void SentItem(Item item, int amount)
	{
		Item tempitem = item.Copy(amount);
		AddToCraftingMenu(tempitem);
		//updateItemlist();
	}

	private int HasCatalyst()
	{
		int HasCatalyst = 0;
		if (ItemCount > 2) return HasCatalyst;
		for (int i = 0; i < ItemCount; i++)
		{
			// The Item ID is stored in the metadata
			int itemID = (int)GetItemMetadata(i);
			switch (itemID)
			{
				case 4: //purple catalyst
					HasCatalyst = 4;
					break;
				case 9: //blue catalyst
					HasCatalyst = 9;
					break;
				case 15: //red catalyst
					HasCatalyst = 15;
					break;
				case 21: //yellow catalyst
					HasCatalyst = 21;
					break;
			}
		}
		return HasCatalyst;
	}

	private void CatalystConsumeIngredient()
	{
		for (int i = 0; i < ItemCount; i++)
		{
			int itemID = (int)GetItemMetadata(i);
			if (itemDatabase.items[itemID].consumable)
			{
				materialInventory.RemoveAmountofItem(itemID, 1);
				craftingMenu[itemID] -= 1;
			}
			if (craftingMenu[itemID] == 0) craftingMenu.Remove(itemID);
		}
		UpdateItemlist();
		CheckValidRecipe();
	}

	private void CatalystConvert(int catalystID)
	{
		if (craftingMenu.ContainsKey(catalystID))
		{
			switch (catalystID)
			{
				case 4: //purple catalyst
					if (craftingMenu.ContainsKey(10) || craftingMenu.ContainsKey(16) || craftingMenu.ContainsKey(22))
					{
						recipeID = 5;
					}
					else if (craftingMenu.ContainsKey(11) || craftingMenu.ContainsKey(17) || craftingMenu.ContainsKey(23))
					{
						recipeID = 6;
					}
					else if (craftingMenu.ContainsKey(12) || craftingMenu.ContainsKey(18) || craftingMenu.ContainsKey(24))
					{
						recipeID = 7;
					}
					else if (craftingMenu.ContainsKey(13) || craftingMenu.ContainsKey(19) || craftingMenu.ContainsKey(25))
					{
						recipeID = 8;
					}
					else
					{
						recipeID = 0;
					}
					break;
				case 9: //blue catalyst
					if (craftingMenu.ContainsKey(5) || craftingMenu.ContainsKey(16) || craftingMenu.ContainsKey(22))
					{
						recipeID = 10;
					}
					else if (craftingMenu.ContainsKey(6) || craftingMenu.ContainsKey(17) || craftingMenu.ContainsKey(23))
					{
						recipeID = 11;
					}
					else if (craftingMenu.ContainsKey(7) || craftingMenu.ContainsKey(18) || craftingMenu.ContainsKey(24))
					{
						recipeID = 12;
					}
					else if (craftingMenu.ContainsKey(8) || craftingMenu.ContainsKey(19) || craftingMenu.ContainsKey(25))
					{
						recipeID = 13;
					}
					else
					{
						recipeID = 0;
					}
					break;
				case 15: //red catalyst
					if (craftingMenu.ContainsKey(10) || craftingMenu.ContainsKey(5) || craftingMenu.ContainsKey(22))
					{
						recipeID = 16;
					}
					else if (craftingMenu.ContainsKey(11) || craftingMenu.ContainsKey(6) || craftingMenu.ContainsKey(23))
					{
						recipeID = 17;
					}
					else if (craftingMenu.ContainsKey(12) || craftingMenu.ContainsKey(7) || craftingMenu.ContainsKey(24))
					{
						recipeID = 18;
					}
					else if (craftingMenu.ContainsKey(13) || craftingMenu.ContainsKey(8) || craftingMenu.ContainsKey(25))
					{
						recipeID = 19;
					}
					else
					{
						recipeID = 0;
					}
					break;
				case 21: //yellow catalyst		
					if (craftingMenu.ContainsKey(10) || craftingMenu.ContainsKey(16) || craftingMenu.ContainsKey(5))
					{
						recipeID = 22;
					}
					else if (craftingMenu.ContainsKey(11) || craftingMenu.ContainsKey(17) || craftingMenu.ContainsKey(6))
					{
						recipeID = 23;
					}
					else if (craftingMenu.ContainsKey(12) || craftingMenu.ContainsKey(18) || craftingMenu.ContainsKey(7))
					{
						recipeID = 24;
					}
					else if (craftingMenu.ContainsKey(13) || craftingMenu.ContainsKey(19) || craftingMenu.ContainsKey(8))
					{
						recipeID = 25;
					}
					else
					{
						recipeID = 0;
					}
					break;
			}
		}

	}

	//Evil recipe validation
	private bool CheckValidRecipe()
	{
		bool isValid = false;
		int CatalystID = HasCatalyst();
		if (CatalystID != 0)
		{
			CatalystConvert(CatalystID);
			if (recipeID != 0)
			{
				isValid = true;
			}
			RecipePicture();
			return isValid;
		}
		foreach (var (recipeid, ingredientsList) in recipeList.recipes)
		{
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
			foreach (var (item, amount) in ingredientsList.ingredients)
			{
				// You have not all the ingredients of the recipe
				if (!craftingMenu.ContainsKey(item.ID))
				{
					isValid = false;
					break;
				}
			}
			if (isValid)
			{
				recipeID = recipeid;
				RecipePicture();
				break;
			}
			else
			{
				recipeID = 0;
				RecipePicture();
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
		{   // we kinda are already checking for this in AddStackable
			if (craftingMenu[item.ID] >= materialInventory.inventory.inventory[item.ID])
			{
				GD.Print($"You can't put more of {item.Name}!!!");
				CheckValidRecipe();
				return couldPickup;
			}
		}
		// if there is no item in the bag yet
		if (!craftingMenu.ContainsKey(item.ID))
		{
			craftingMenu.Add(item.ID, item.qty);
			UpdateItemlist();
			CheckValidRecipe();
			return true;
		}

		return couldPickup;
	}
	public void UpdateItemlist()
	{
		Clear();
		foreach (var (id, amount) in craftingMenu)
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
			if (craftingMenu[item.ID] >= materialInventory.inventory.inventory[item.ID])
			{
				GD.Print($"You can't put more {item.Name}!!!");
				return couldPickup;
			}
			//Pick up ALL the item and add it
			if (craftingMenu[item.ID] + item.qty <= materialInventory.inventory.inventory[item.ID])
			{
				craftingMenu[item.ID] = item.qty + craftingMenu[item.ID];
				couldPickup = true;
			}
		}
		if (couldPickup) CheckValidRecipe();
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
		Item temp = itemDatabase.items[id].Copy();
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
			RemoveItemFromCrafting((int)index);
			GD.Print($"You took out an item");
		}
		// Left Click to add more items....
		else if (mousebuttonindex == 1)
		{
			Item tempitem = GetItemFromCrafting((int)index);
			tempitem.qty = 1;
			AddToCraftingMenu(tempitem);
		}
	}


}
