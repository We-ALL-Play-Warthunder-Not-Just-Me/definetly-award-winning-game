using Godot;
using System;

public partial class BoundingBox : Sprite2D
{
	Node2D BoundBox;
	Marker2D TopLeft;
	Marker2D BottomRight;
	Node2D SpawnLeft;
	Node2D SpawnRight;
	Node2D SpawnTop;
	Node2D SpawnBottom;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoundBox = this;
		TopLeft = GetNode<Marker2D>("TopLeft");
		BottomRight = GetNode<Marker2D>("BottomRight");
		SpawnLeft = GetNode<Node2D>("SpawnLeft");
		SpawnRight = GetNode<Node2D>("SpawnRight");
		SpawnTop = GetNode<Node2D>("SpawnTop");
		SpawnBottom = GetNode<Node2D>("SpawnBottom");
		
		SetUpRestraints();
		SetUpSpawns();
	}
	
	private void SetUpRestraints()
	{
		//All of this just calculates where to put each marker on the
		//map automatically so you don't have to do it manually.
		var BBEdge = new Vector2(BoundBox.Scale.X/2, BoundBox.Scale.Y/2);
		TopLeft.Position = new Vector2(-BBEdge.X, -BBEdge.Y);
		BottomRight.Position = new Vector2(BBEdge.X, BBEdge.Y);
	}
	
	private void SetUpSpawns()
	{
		//All of this just calculates where to put each point on the
		//map automatically so you don't have to do it manually.
		var BBEdge = new Vector2(BoundBox.Scale.X/2, BoundBox.Scale.Y/2);
		SpawnLeft.Position = new Vector2(-(BBEdge.X-10), 0);
		SpawnRight.Position = new Vector2((BBEdge.X-10), 0);
		SpawnTop.Position = new Vector2(0, -(BBEdge.Y-20));
		SpawnBottom.Position = new Vector2(0, (BBEdge.Y-20));
	}
}
