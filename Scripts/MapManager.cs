using Godot;
using System;

public partial class MapManager : Node2D
{
	public int TestSpawn;
	Node2D Player;
	Node2D SpawnPoint;
	Node2D GameScene;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<Node2D>("/root/GameScene/Player");
		GameScene = GetNode<Node2D>("/root/GameScene");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ChangeMap(int S, string NM, string CM, Node2D P)
	{
		//This takes the Old Map data and the New Map data and swaps
		//the two by Loading in the New Map and killing the Old Map
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/" + CM);
		var NewMap = GD.Load<PackedScene>("res://Scenes/" + NM + ".tscn");
		var SpawnNewMap = NewMap.Instantiate();
		OldMap.QueueFree();
		GameScene.AddChild(SpawnNewMap);
		
		GD.Print(S);
		GD.Print(NM);
		//This will move the Player
		SpawnPoint = GetNode<Node2D>("/root/GameScene/" + NM + "/SpawnPoints/Spawn" + S);
		P.Position = SpawnPoint.Position;
	}
	
	public void CollectInfo(int Spawn, string NextMap, string CurrentMap)
	{
		ChangeMap(Spawn, NextMap, CurrentMap, Player);
	}
}
