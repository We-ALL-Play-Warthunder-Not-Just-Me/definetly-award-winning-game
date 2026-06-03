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
		//This was part of my solution to make sure the Player stays
		//still long enough to pass the Area2Ds safely. It literally
		//wouldn't work if I had this check anywhere else.
		if (!Halt)
		{
			PlayerCollision.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
	}
	
	public void ChangeMap(string NM, string CM)
	{
		//This isn't the best way to do it, there should probably
		//be a script that holds all the levels in a series of
		//dictionaries, but that's only necessary if this was a full
		//project that goes beyond the Jam.
		
		//This finds the New Map among the resources and loads it up.
		var NewMap = GD.Load<PackedScene>("res://Scenes/" + NM + ".tscn");
		var SpawnNewMap = NewMap.Instantiate();
		//This spawns in the New Map when it's safe and adds it to
		//the scene before killing the Old Map in the safe zone.
		//We're all about safety here lest Godot screams at us.
		GameScene.CallDeferred(Node2D.MethodName.AddChild, SpawnNewMap);
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/" + CM);
		OldMap.CallDeferred(Node2D.MethodName.QueueFree);
	}
	
	public void MovePlayer(string S, string NM, bool VT, Node2D P)
	{
		//This finds the corresponding SpawnPoint needed from the
		//scene tree AND its Offset from the center to make sure
		//the Player will move the correct distance.
		SpawnOffset = GetNode<Node2D>("/root/GameScene/"+NM+"/BoundingBox");
		SpawnPoint = SpawnOffset.GetNode<Node2D>("Spawn"+S);
		
		var ActualSpawn = new Vector2(SpawnPoint.Position.X + SpawnOffset.Position.X, SpawnPoint.Position.Y + SpawnOffset.Position.Y);
		
		//This is just checking if the Player is moving through a
		//Horizontal or Vertical transition point and moving them
		//on the correct Axis.
		if (VT == true)
		{
			P.Position = new Vector2(P.Position.X, ActualSpawn.Y);
		}
		else if (VT == false)
		{
			P.Position = new Vector2(ActualSpawn.X, P.Position.Y);
		}
	}
	
	public void MoveCamera(string NM)
	{
		//These are all about grabbing the markers that define the
		//edges of the camera bounds on the newly spawned in map.
		Restraints = GetNode<Node2D>("/root/GameScene/"+NM+"/BoundingBox");
		Marker2D TopLeft = Restraints.GetNode<Marker2D>("TopLeft");
		Marker2D BottomRight = Restraints.GetNode<Marker2D>("BottomRight");
		//This sets the contraints of the camera to that of the
		//markers grabbed up above.
		Camera.LimitTop = (int)(TopLeft.Position.Y + Restraints.Position.Y);
		Camera.LimitBottom = (int)(BottomRight.Position.Y + Restraints.Position.Y);
		Camera.LimitLeft = (int)(TopLeft.Position.X + Restraints.Position.X);
		Camera.LimitRight = (int)(BottomRight.Position.X + Restraints.Position.X);
	}
	
	public void ProcessInfo(string Spawn, string NextMap, string CurrentMap, bool VertTrans)
	{
		//Set up Halt to track how long the Player's Collision will
		//be disabled for to clear the Area2Ds to prevent map fuckery.
		Halt = true;
		PlayerCollision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		//Pretty self explanitory what this is doing.
		ChangeMap(NextMap, CurrentMap);
		//This is making sure that the MovePlayer and MoveCamera
		//functions only run in the safe zone after the maps have
		//been successfully swapped.
		CallDeferred(MapManager.MethodName.MovePlayer, Spawn, NextMap, VertTrans, Player);
		CallDeferred(MapManager.MethodName.MoveCamera, NextMap);
		//Sends the update that the Player can have their collision back.
		Halt = false;
	}
}
