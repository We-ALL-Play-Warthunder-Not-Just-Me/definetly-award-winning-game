using Godot;
using System;

public partial class RecipeListDropDown : MenuButton
{

	[Export] ItemDictionary itemDatabase;
	[Export] RecipesList recipesList;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FillMenu();
		GetPopup().IdPressed += sendItems;
	}

	private void FillMenu()
	{
		GetPopup().Clear();
		int i = 0;
		foreach (var(id,ingredients) in recipesList.recipes)
		{
			GetPopup().AddIconItem(itemDatabase.items[id].icon,$"{itemDatabase.items[id].Name}");
			GetPopup().SetItemIconMaxWidth(i, 50);
			GetPopup().SetItemMetadata(i,id);
			i++;
		}
	}

	private void sendItems(long id)
	{
		GD.Print($"You clicked on {GetPopup().GetItemText((int)id)} id: {GetPopup().GetItemId((int)id)} itemID: {GetPopup().GetItemMetadata((int)id)}");
		int itemID = (int)GetPopup().GetItemMetadata((int)id);
		foreach (var (item,amount) in recipesList.recipes[itemID].ingredients)
		{
			Item tempItem = item.shallowCopy(amount);
			EmitSignal(SignalName.SentItem, tempItem, amount);
		}
	}

	[Signal]
	public delegate void SentItemEventHandler(Item item, int amount);

	
}
