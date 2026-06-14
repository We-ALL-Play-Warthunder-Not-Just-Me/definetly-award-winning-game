using Godot;
using System;

[GlobalClass]
public partial class ItemDictionary : Resource
{
	[Export]
	public Godot.Collections.Dictionary<int,Item> items = [];


	public ItemDictionary() {}

	
}
