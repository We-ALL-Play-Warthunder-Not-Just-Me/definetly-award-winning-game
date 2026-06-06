using Godot;
using System;

public partial class InventoryPicture : TextureRect
{

	private void _on_item_list_sent_item_data(Item item)
	{
		Texture = item.icon;

	}
}
