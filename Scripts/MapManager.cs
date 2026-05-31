using Godot;
using System;

public partial class MapManager : Node2D
{
	public int TestSpawn;
	Node2D Player;
	Node2D SpawnPoint;
	Node2D GameScene;
	Camera2D Camera;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<Node2D>("/root/GameScene/Player");
		GameScene = GetNode<Node2D>("/root/GameScene");
		Camera = GetNode<Camera2D>("/root/GameScene/TheCamera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ChangeMap(int S, string NM, string CM, bool VT, Node2D P)
	{
		//This takes the Old Map data and the New Map data and swaps
		//the two by Loading in the New Map and killing the Old Map
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/" + CM);
		OldMap.QueueFree();
		var NewMap = GD.Load<PackedScene>("res://Scenes/" + NM + ".tscn");
		var SpawnNewMap = NewMap.Instantiate();
		GameScene.AddChild(SpawnNewMap);
		
		//This finds the corresponding SpawnPoint needed and moves the
		//Player to the correct spot based on if it's a Vertical
		//Transition or not while maintain the other position
		SpawnPoint = GetNode<Node2D>("/root/GameScene/" + NM + "/SpawnPoints/Spawn" + S);
		if (VT == true)
		{
			P.Position = new Vector2(P.Position.X, SpawnPoint.Position.Y);
		}
		else if (VT == false)
		{
			P.Position = new Vector2(SpawnPoint.Position.X, P.Position.Y);
		}
		
		//This stuff is temp, don't worry about it
		if (NM == "Map1") {Camera.Position = new Vector2(-160, 0);}
		else {Camera.Position = new Vector2(0, 0);}
		
	}
	
	public void CollectInfo(int Spawn, string NextMap, string CurrentMap, bool VertTrans)
	{
		ChangeMap(Spawn, NextMap, CurrentMap, VertTrans, Player);
	}
}
