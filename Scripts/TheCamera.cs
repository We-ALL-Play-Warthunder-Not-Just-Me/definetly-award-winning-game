using Godot;
using System;

public partial class TheCamera : Camera2D
{
	Node2D Player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<Node2D>("/root/GameScene/Player");
		this.LimitTop = -90;
		this.LimitBottom = 90;
		this.LimitLeft = -160;
		this.LimitRight = 160;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position = Player.Position;
	}
}
