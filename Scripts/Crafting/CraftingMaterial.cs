using Godot;
using System;

public partial class CraftingMaterial : ItemList
{
	[Export] ItemDictionary itemDatabase;
	[Export] RecipesList recipesList;
	[Export] InventoryStorage materialInventory;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		updateItemlist();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void updateItemlist()
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
		ResourceSaver.Save(materialInventory);
	}

}
