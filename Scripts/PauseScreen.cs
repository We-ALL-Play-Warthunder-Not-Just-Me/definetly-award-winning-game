using Godot;
using System;
using System.Diagnostics;

public partial class PauseScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print("apple");
		if (Input.IsActionJustPressed("escape"))
		{
			pauseToggle();
		}
	}

	private void _on_resume_pressed()
	{
		OnCloseButtonPressed();
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
