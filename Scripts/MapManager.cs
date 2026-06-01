using Godot;
using System;

public partial class MapManager : Node2D
{
	public int TestSpawn;
	Node2D Player;
	Node2D SpawnPoint;
	Node2D GameScene;
	Camera2D Camera;
	Node2D Background;
	int Halt;
	
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
	
	public void ChangeMap(int S, string NM, string CM)
	{
		//This takes the Old Map data and the New Map data and swaps
		//the two by Loading in the New Map and killing the Old Map
		var NewMap = GD.Load<PackedScene>("res://Scenes/" + NM + ".tscn");
		var SpawnNewMap = NewMap.Instantiate();
		GameScene.CallDeferred(Node2D.MethodName.AddChild, SpawnNewMap);
		//GameScene.AddChild(SpawnNewMap);
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/" + CM);
		OldMap.CallDeferred(Node2D.MethodName.QueueFree);
		//OldMap.QueueFree();
	}
	
	public void MovePlayer(int S, string NM, bool VT, Node2D P)
	{
		//This finds the corresponding SpawnPoint needed and moves the
		//Player to the correct spot based on if it's a Vertical
		//Transition or not while maintaining the other position
		SpawnPoint = GetNode<Node2D>("/root/GameScene/"+NM+"/SpawnPoints/Spawn"+S);
		
		if (SpawnPoint is null)
		{
			GD.Print("Blergh! Don't got it...");
			//Dangerous?
			//MovePlayer(S, NM, VT, P);
		}
		else
		{
			GD.Print("Got it!");
			if (VT == true)
			{
				P.Position = new Vector2(P.Position.X, SpawnPoint.Position.Y);
			}
			else if (VT == false)
			{
				P.Position = new Vector2(SpawnPoint.Position.X, P.Position.Y);
			}
			MoveCamera(NM);
		}
	}
	
	public void MoveCamera(string NM)
	{
		//This stuff is temp, don't worry about it
		//if (NM == "Map1") {Camera.Position = new Vector2(-160, 0);}
		//else {Camera.Position = new Vector2(0, 0);}
		
		//Move the Camera to the correct spot
		Background = GetNode<Node2D>("/root/GameScene/"+NM+"/TempBacking");
		var TopLimit = -(Background.Scale.Y / 2);
		var BottomLimit = (Background.Scale.Y / 2);
		var LeftLimit = -(Background.Scale.X / 2);
		var RightLimit = (Background.Scale.X / 2);
		Camera.LimitTop = (int)TopLimit;
		Camera.LimitBottom = (int)BottomLimit;
		Camera.LimitLeft = (int)LeftLimit;
		Camera.LimitRight = (int)RightLimit;
	}
	
	public void ProcessInfo(int Spawn, string NextMap, string CurrentMap, bool VertTrans)
	{
		ChangeMap(Spawn, NextMap, CurrentMap);
		CallDeferred(MapManager.MethodName.MovePlayer, Spawn, NextMap, VertTrans, Player);
	}
}
