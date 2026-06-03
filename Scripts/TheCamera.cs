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
		//Deprecated for now. Might just get rid of it.
		//Restraints = GetNode<Node2D>("/root/GameScene/"+Map+"/CameraRestraints");
		//Marker2D TopLeft = Restraints.GetNode<Marker2D>("TopLeft");
		//Marker2D BottomRight = Restraints.GetNode<Marker2D>("BottomRight");
		//this.LimitTop = (int)TopLeft.Position.Y;
		//this.LimitBottom = (int)BottomRight.Position.Y;
		//this.LimitLeft = (int)TopLeft.Position.X;
		//this.LimitRight = (int)BottomRight.Position.X;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position = Player.Position;
	}
}
