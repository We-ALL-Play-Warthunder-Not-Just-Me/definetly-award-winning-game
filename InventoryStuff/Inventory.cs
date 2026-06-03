using Godot;
using System;

public partial class Inventory : Control
{

	public Item apple = new Item();

	//private TabContainer invi;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//invi = GetNode<TabContainer>("TabContainer");
		// Callable.From(invi.GrabFocus).CallDeferred();

		//Callable.From(GrabFocus).CallDeferred();
		//GetNode<TabBar>("Inventory");
		//Callable.From(GetNode<TabBar>("Inventory").GrabFocus).CallDeferred();
		//GrabFocus();
		//GD.Print(HasFocus());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("InventoryOpen") )
		{
			//invi.GrabFocus();
			//GrabFocus();
			//GD.Print(HasFocus());
		}
	}
}
