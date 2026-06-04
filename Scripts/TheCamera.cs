using Godot;
using System;

public partial class TheCamera : Camera2D
{
	[Export]
	public string Map;
	Node2D Restraints;
	Node2D Player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<Node2D>("/root/GameScene/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//I probably could've left this on the player...
		this.Position = Player.Position;
	}
}
