using Godot;
using System;

public partial class SendItemToInventory : Node
{
	public static SendItemToInventory Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	[Signal]
	public delegate void SentItemEventHandler(Item item, int amount);


}
