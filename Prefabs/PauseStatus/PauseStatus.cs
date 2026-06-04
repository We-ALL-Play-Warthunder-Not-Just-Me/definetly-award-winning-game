using Godot;
using System;

public partial class PauseStatus : CanvasLayer
{
	private static Control pause;
	private static Control inventory;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pause = GetNode<Control>("PauseScreen");
		inventory = GetNode<Control>("InventoryScreen");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("escape"))
		{
			OnPauseButtonPressed();
			pause.Show();
		}
		else if (@event.IsActionPressed("InventoryOpen"))
		{
			OnPauseButtonPressed();
			inventory.Show();
		}
	}

	private void pauseToggle()
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
