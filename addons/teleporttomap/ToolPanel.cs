using Godot;
using System;

[Tool]
public partial class ToolPanel : VBoxContainer
{
	[Export] SpinBox spinBoxX;
	[Export] SpinBox spinBoxY;
	[Export] Button goButton;
	// [Export] CharacterBody2D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		goButton.Pressed += ButtonPressed;
	}

	private void ButtonPressed()
	{
		Vector2 awawawawa = new Vector2((float)spinBoxX.Value,(float)spinBoxY.Value);
		var selection = EditorInterface.Singleton.GetSelection();
		
		foreach (Node awa in selection.GetSelectedNodes())
		{
			if (awa is CharacterBody2D player)
			{
				GD.Print(awawawawa);
				player.GlobalPosition = awawawawa;
			}
		}
	}
}
