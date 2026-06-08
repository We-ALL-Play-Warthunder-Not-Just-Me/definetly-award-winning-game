using Godot;
using System;

public partial class CraftingMaterial : ItemList
{
	[Export] ItemDictionary itemDatabase;
	[Export] RecipesList recipesList;
	[Export] InventoryStorage materialInventory;
	[Export] LineEdit searchBar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateItemlist();
		materialInventory.Changed += UpdateItemlist;
		ItemActivated += SelectItem;
		ItemClicked += OnInventoryItemClicked;
		searchBar.TextChanged += SearchItemList;
	}

	private void SearchItemList(String text)
	{
		Clear();
		if(text == null || text == "")
		{
			UpdateItemlist();
			return;
		}
		
		foreach (var (id,amount) in materialInventory.inventory)
		{
			if (itemDatabase.items[id].max_qty == 1)
			{
				if (itemDatabase.items[id].Name.Contains(text,StringComparison.OrdinalIgnoreCase))
				{
					int i = AddItem(itemDatabase.items[id].Name, itemDatabase.items[id].icon);
					SetItemMetadata(i, itemDatabase.items[id].ID);
				}
				
			}
			else
			{
				if (itemDatabase.items[id].Name.Contains(text,StringComparison.OrdinalIgnoreCase))
				{
					int i = AddItem($"{itemDatabase.items[id].Name} : {amount}", itemDatabase.items[id].icon);
					SetItemMetadata(i, itemDatabase.items[id].ID);
				}
				
			}
		}
	}

	public void UpdateItemlist()
	{
		Clear();
		foreach (var (id,amount) in materialInventory.inventory)
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
		//ResourceSaver.Save(materialInventory);
	}

	[Signal]
	public delegate void SendOverItemEventHandler(Item item);

	private void SelectItem(long index)
	{
		Item sendingItem = GetInventoryItem((int) index);
		sendingItem.qty = 1;
		GD.Print($"You selected {sendingItem.Name}");

		EmitSignal(SignalName.SendOverItem, sendingItem);
	}

	//Waow
	public Item GetInventoryItem(int index)
	{
		if (index < 0 || index >= materialInventory.inventory.Count) return null;
		int id = (int)GetItemMetadata(index);
		Item temp = itemDatabase.items[id].shallowCopy();
		if (materialInventory.inventory.ContainsKey(id))
		{
			temp.qty = materialInventory.inventory[id];
		}
		return temp;
	}

	private void OnInventoryItemClicked(long index, Vector2 pos, long mousebuttonindex)
	{
		if (mousebuttonindex == 1)
		{
			Item sendingItem = GetInventoryItem((int) index);
			sendingItem.qty = 1;
			GD.Print($"You selected {sendingItem.Name}");
			EmitSignal(SignalName.SendOverItem, sendingItem);
		}
	}

}
