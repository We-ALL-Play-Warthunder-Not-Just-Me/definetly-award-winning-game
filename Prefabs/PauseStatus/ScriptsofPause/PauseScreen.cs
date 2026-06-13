using Godot;
using System;
using System.Diagnostics;

public partial class PauseScreen : Control
{
	[Export] private Button resumeButton;
	[Export] private Button settingsButton;
	[Export] private Button quitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resumeButton.Pressed += OnResumePressed;
		settingsButton.Pressed += OnSettingsPressed;
		quitButton.Pressed += OnQuitButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print("apple");
		// if (Input.IsActionJustPressed("escape"))
		// {
		// 	pauseToggle();
		// }
	}


	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("escape") && Visible == true)
		{
			//GetViewport().SetInputAsHandled();
			//GD.Print("apple");
			AcceptEvent();
			OnCloseButtonPressed();
		}
	}

	private void OnResumePressed()
	{
		OnCloseButtonPressed();
	}

	private void OnSettingsPressed()
	{
		try
		{
			GetNode<Control>("Settings").Show();
		}
		catch
		{
			GD.Print("something went wrong with opening settings");
		}
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
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
