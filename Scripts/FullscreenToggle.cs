using Godot;
using System;

public partial class FullscreenToggle : CheckBox
{


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Toggled += ToggleFullScreen;
		if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
		{
			SetPressedNoSignal(true);
		}
		else
		{
			SetPressedNoSignal(false);
		}
	}

	private void ToggleFullScreen(bool toggle_on)
	{
		if (toggle_on)
		{
			//DisplayServer.WindowMode.Fullscreen
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else if (!toggle_on)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}

}
