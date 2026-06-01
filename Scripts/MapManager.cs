using Godot;
using System;

public partial class MapManager : Node2D
{
	public int TestSpawn;
	Node2D Player;
	CollisionShape2D PlayerCollision;
	Node2D SpawnPoint;
	Node2D SpawnOffset;
	Node2D GameScene;
	Camera2D Camera;
	Node2D Restraints;
	bool Halt;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<Node2D>("/root/GameScene/Player");
		PlayerCollision = Player.GetNode<CollisionShape2D>("PlayerCollisionShape2D");
		GameScene = GetNode<Node2D>("/root/GameScene");
		Camera = GetNode<Camera2D>("/root/GameScene/TheCamera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Halt)
		{
			PlayerCollision.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
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
		SpawnOffset = GetNode<Node2D>("/root/GameScene/"+NM+"/SpawnPoints");
		var ActualSpawn = new Vector2(SpawnPoint.Position.X + SpawnOffset.Position.X, SpawnPoint.Position.Y + SpawnOffset.Position.Y);
		
		if (VT == true)
		{
			P.Position = new Vector2(P.Position.X, ActualSpawn.Y);
		}
		else if (VT == false)
		{
			P.Position = new Vector2(ActualSpawn.X, P.Position.Y);
		}
		MoveCamera(NM);
	}
	
	public void MoveCamera(string NM)
	{
		//This stuff is temp, don't worry about it
		//if (NM == "Map1") {Camera.Position = new Vector2(-160, 0);}
		//else {Camera.Position = new Vector2(0, 0);}
		
		//Move the Camera to the correct spot
		Restraints = GetNode<Node2D>("/root/GameScene/"+NM+"/CameraRestraints");
		Marker2D TopLeft = Restraints.GetNode<Marker2D>("TopLeft");
		Marker2D BottomRight = Restraints.GetNode<Marker2D>("BottomRight");
		Camera.LimitTop = (int)(TopLeft.Position.Y + Restraints.Position.Y);
		Camera.LimitBottom = (int)(BottomRight.Position.Y + Restraints.Position.Y);
		Camera.LimitLeft = (int)(TopLeft.Position.X + Restraints.Position.X);
		Camera.LimitRight = (int)(BottomRight.Position.X + Restraints.Position.X);
	}
	
	public void ProcessInfo(int Spawn, string NextMap, string CurrentMap, bool VertTrans)
	{
		Halt = true;
		PlayerCollision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		ChangeMap(Spawn, NextMap, CurrentMap);
		CallDeferred(MapManager.MethodName.MovePlayer, Spawn, NextMap, VertTrans, Player);
		Halt = false;
	}
}
