using Godot;
using System;

public partial class CraftedItemPicture : TextureRect
{
	[Export] CraftingLogic crafting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		crafting.recipePicture += changePicture;
	}

	private void changePicture(Texture2D icon)
	{
		Texture = icon;
	}
}
