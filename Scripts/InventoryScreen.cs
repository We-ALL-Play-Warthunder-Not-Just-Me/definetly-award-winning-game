using Godot;
using System;

public partial class InventoryScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (Input.IsActionJustPressed("InventoryOpen") )
		// {
		// 	inventoryToggle();
		// }

	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("InventoryOpen"))
		{
			AcceptEvent();
			inventoryToggle();
		}
		else if (@event.IsActionPressed("escape"))
		{
			AcceptEvent();
			//GD.Print("apple");
			OnCloseButtonPressed();
		}
	}

	private void inventoryToggle()
	{
		if (GetTree().Paused == false)
		{
			OnPauseButtonPressed();
		}
		else if (GetTree().Paused == true && Visible == false)
		{
			GetNode<Control>("../PauseScreen").Hide();
			Show();
		}
		else
		{
			OnCloseButtonPressed();
		}
	}

	private void inventoryClose()
	{
		if (Input.IsActionJustPressed("escape"))
		{
			OnCloseButtonPressed();
		}
	}

	private void OnPauseButtonPressed()
	{
		GetTree().Paused = true;
		Show();
	}

	private void OnCloseButtonPressed()
	{
		Hide();
		GetTree().Paused = false;
	}


}
