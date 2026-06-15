using Godot;
using System;

public partial class MapManager : Node2D
{
	//Empty Values
	CharacterBody2D Player;
	CollisionShape2D PlayerCollision;
	Node2D SpawnPoint;
	Node2D SpawnOffset;
	Node2D GameScene;
	Camera2D Camera;
	Node2D Restraints;
	MvmSongIdea Music;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//More grabbing of relevant information for later use.
		Player = GetNode<CharacterBody2D>("/root/GameScene/Player");
		PlayerCollision = Player.GetNode<CollisionShape2D>("PlayerCollisionShape2D");
		GameScene = GetNode<Node2D>("/root/GameScene");
		Camera = GetNode<Camera2D>("/root/GameScene/TheCamera");
		//Some Music Fun
		Music = GetNode<MvmSongIdea>("../GameScene/MvmSongIdea");
	}
	
	private void ChangeMap(string S, string NM, int TC, string CM, int FC, float EM)
	{
		//This isn't the best way to do it, there should probably
		//be a script that holds all the levels in a series of
		//dictionaries, but that's only necessary if this was a full
		//project that goes beyond the Jam.
		
		//This finds the New Map among the resources and loads it up.
		var NewMap = GD.Load<PackedScene>("res://Scenes/" + NM + ".tscn");
		var SpawnNewMap = NewMap.Instantiate();
		GameScene.AddChild(SpawnNewMap);
		MoveMap(S, NM, TC, CM, FC, EM);
		//This spawns in the New Map when it's safe and adds it to
		//the scene before killing the Old Map in the safe zone.
		//We're all about safety here lest Godot screams at us.
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/" + CM);
		OldMap.QueueFree();
		//This is making sure that the MovePlayer and MoveCamera
		//functions only run in the safe zone after the maps have
		//been successfully swapped.
	}
	
	private void MoveMap(string S, string NM, int TC, string CM, int FC, float EM)
	{
		//Grabbing all of the relevant Nodes and Markers that will be needed
		//to run the code for Moving the Map.
		Node2D SpawnedMap = GetNode<Node2D>("/root/GameScene/"+NM);
		Node2D OldMap = GetNode<Node2D>("/root/GameScene/"+CM);
		Node2D CCB = OldMap.GetNode<Node2D>("BoundingBox");
		Node2D NCB = SpawnedMap.GetNode<Node2D>("BoundingBox");
		Marker2D GFC = CCB.GetNode<Marker2D>("Chunk"+FC);
		Marker2D GTC = NCB.GetNode<Marker2D>("Chunk"+TC);
		
		//This is setting up relevant information for later use for proper
		//Map Spawning and making sure it moves in relation to the already
		//existing OldMap for best effect.
		SpawnedMap.Position = OldMap.Position;
		var MoveX = CCB.Scale.X/2 + NCB.Scale.X/2;
		var MoveY = CCB.Scale.Y/2 + NCB.Scale.Y/2 + EM;
		var ChunkDistance = 0f;
		
		//All of these if statements are for checking what direction to spawn
		//the map in, essentially. They aren't the most straightforward, though.
		//Referrence//
		//Left = Move Right, Right = Move Left, Top = Move Down, Bottom = Move Up. 
		//You got that? Good. Cause my other comments only help calrify a little.
		
		//Left = Entering on Left, so Move Map to the Right.
		if (S == "Left")
		{
			//ChunkDistance is basically just getting the difference between
			//the two chunks and moving the Map along the desired axis to allign
			//the NewMap with the OldMap cleanly, and if the distance is zero,
			//then they just stay on the same level.
			//Also, refer to Tops comment.
			ChunkDistance = GTC.Position.Y - GFC.Position.Y;
			if (ChunkDistance != 0)
			{SpawnedMap.Position = new Vector2(OldMap.Position.X + MoveX, SpawnedMap.Position.Y - ChunkDistance);}
			else
			{SpawnedMap.Position = new Vector2(OldMap.Position.X + MoveX, SpawnedMap.Position.Y);}
		}
		
		//Right = Entering on Right, so Move Map to the Left.
		else if (S == "Right")
		{
			//Refer to Lefts comment.
			ChunkDistance = GTC.Position.Y - GFC.Position.Y;
			if (ChunkDistance != 0)
			{SpawnedMap.Position = new Vector2(OldMap.Position.X - MoveX, SpawnedMap.Position.Y - ChunkDistance);}
			else
			{SpawnedMap.Position = new Vector2(OldMap.Position.X - MoveX, SpawnedMap.Position.Y);}
		}
		
		//Top = Entering on Top, so Move the Map Down.
		if (S == "Top")
		{
			//Refer to Bottoms comment.
			ChunkDistance = GTC.Position.X - GFC.Position.X;
			if (ChunkDistance != 0)
			{SpawnedMap.Position = new Vector2(SpawnedMap.Position.X - ChunkDistance, OldMap.Position.Y + MoveY);}
			else
			{SpawnedMap.Position = new Vector2(SpawnedMap.Position.X, OldMap.Position.Y + MoveY);}
		}
		
		//Bottom = Entering on Bottom, so Move the Map Up.
		else if (S == "Bottom")
		{
			//Refer to Rights comment.
			ChunkDistance = GTC.Position.X - GFC.Position.X;
			if (ChunkDistance != 0)
			{SpawnedMap.Position = new Vector2(SpawnedMap.Position.X - ChunkDistance, OldMap.Position.Y - MoveY);}
			else
			{SpawnedMap.Position = new Vector2(SpawnedMap.Position.X, OldMap.Position.Y - MoveY);}
		}
	}
	
	private void MovePlayer(string S, string NM, bool VT, Node2D P)
	{
		//This finds the corresponding SpawnPoint needed from the
		//scene tree AND its Offset from the center to make sure
		//the Player will move the correct distance.
		SpawnOffset = GetNode<Node2D>("/root/GameScene/"+NM);
		SpawnPoint = SpawnOffset.GetNode<Node2D>("BoundingBox/Spawn"+S);
		
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
	
	private void MoveCamera(string NM)
	{
		//These are all about grabbing the markers that define the
		//edges of the camera bounds on the newly spawned in map.
		Restraints = GetNode<Node2D>("/root/GameScene/"+NM);
		Marker2D TopLeft = Restraints.GetNode<Marker2D>("BoundingBox/TopLeft");
		Marker2D BottomRight = Restraints.GetNode<Marker2D>("BoundingBox/BottomRight");
		//This sets the contraints of the camera to that of the
		//markers grabbed up above.
		Camera.LimitTop = (int)(TopLeft.Position.Y + Restraints.Position.Y);
		Camera.LimitBottom = (int)(BottomRight.Position.Y + Restraints.Position.Y);
		Camera.LimitLeft = (int)(TopLeft.Position.X + Restraints.Position.X);
		Camera.LimitRight = (int)(BottomRight.Position.X + Restraints.Position.X);
	}
	
	public void ProcessInformation(string S, string NM, int TC, string CM, int FC, bool VT, float EM)
	{
		//Disables the Player Collision until everything is done.
		PlayerCollision.SetDisabled(true);
		//Changes and Moves the maps accordingly to the desired outcome.
		ChangeMap(S, NM, TC, CM, FC, EM);
		//Moves the Player to the desired SpawnPoint as defined by the info.
		MovePlayer(S, NM, VT, Player);
		//Moves the Camera to its new restraints on the new Map.
		MoveCamera(NM);
		//Allowing the Player to collide again now that it's all complete.
		PlayerCollision.SetDisabled(false);
		//Just doing a little funny goof.
		Music.SwitchSong();
	}
	
	public void CollectInfo(string Spawn, string NextMap, int ToChunk,
				string CurrentMap, int FromChunk, bool VertTrans, float ExtraMaps)
	{
		//Takes all that Collected information from the MapTransitioner and
		//procceeds to call the Processing in the safety net of Deferred land.
		CallDeferred(MapManager.MethodName.ProcessInformation, Spawn, NextMap,
								ToChunk, CurrentMap, FromChunk, VertTrans, ExtraMaps);
	}
}
