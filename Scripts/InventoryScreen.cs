using Godot;
using System;

public partial class InventoryScreen : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("InventoryOpen") )
		{
			inventoryToggle();
		}

		inventoryClose();
	}

	private void inventoryToggle()
	{
		if (GetTree().Paused == false)
		{
			OnPauseButtonPressed();
		}
		else
		{
			OnCloseButtonPressed();
		}
	}

	private void inventoryClose()
	{
		if (Input.IsActionJustPressed("ui_cancel"))
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
