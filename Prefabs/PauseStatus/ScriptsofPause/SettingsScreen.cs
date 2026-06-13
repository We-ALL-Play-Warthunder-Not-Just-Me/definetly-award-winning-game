using Godot;
using System;

public partial class SettingsScreen : Control
{
	[Export] private Button backButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backButton.Pressed += OnBackButtonPressed;
	}

	private void OnBackButtonPressed()
	{
		Hide();
	}

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("escape"))
		{
			AcceptEvent();
			Hide();
		}
    }
}
