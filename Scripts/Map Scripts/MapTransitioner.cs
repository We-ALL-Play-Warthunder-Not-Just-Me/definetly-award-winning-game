using Godot;
using System;

public partial class MapTransitioner : Area2D
{
	//The tedious variables that need filling per transitioner on scene...
	[Export]
	public string NextMap;
	[Export]
	public int ToChunk;
	[Export]
	public bool VertTrans;
	[Export]
	public int ExtraRooms;
	
	//Empty Values
	MapManager MapMan;
	CollisionShape2D CollLR;
	CollisionShape2D CollUD;
	float MapHeight = 192f;
	float ExtraMaps;
	RoomSetUp BoundBox;
	int CheckChunks;
	string SpawnSide;
	string CurrentMap;
	int FromChunk;
	
	public override void _Ready()
	{
		//This grabs the global MapManager to communicate with.
		MapMan = GetNode<MapManager>("/root/MapManager");
		//Getting the Collisions for swapping purposes.
		CollLR = GetNode<CollisionShape2D>("TransitionerLeftRight");
		CollUD = GetNode<CollisionShape2D>("TransitionerUpDown");
		//Providing the padding number needed for spacing maps.
		ExtraMaps = MapHeight * ExtraRooms;
		//Testing stuff
		BoundBox = GetNode<RoomSetUp>("../BoundingBox");
		CheckChunks = BoundBox.ChunksMade;
		//GD.Print("Chunks in the scene: " + BoundBox.ChunksMade);
		_CheckChunkDistances(CheckChunks, VertTrans);
		_CheckSpawnDistances(VertTrans);
		Node2D TheMap = GetNode<Node2D>("..");
		CurrentMap = TheMap.Name;
	}
	
	public override void _Process(double delta)
	{
		//Don't worry too much about all this. It's just making the transitioners
		//look nicer in game when collision shapes are turned on in the debug
		//options so there isn't giant white boxes jutting out across the map.
		//It's also handling what box is active for map transitions.
		if (VertTrans == true)
		{
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Visible, false);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Visible, true);
		}
		else
		{
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Visible, true);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Visible, false);
		}
	}
	
	private void _CheckSpawnDistances(bool VertTrans)
	{
		if (VertTrans == true)
		{
			Node2D SpawnT = BoundBox.SpawnTop;
			Node2D SpawnB = BoundBox.SpawnBottom;
			float STDistance = this.Position.DistanceTo(SpawnT.Position);
			float SBDistance = this.Position.DistanceTo(SpawnB.Position);
			//GD.Print("Top is this far: " + STDistance);
			//GD.Print("Bottom is this far: " + SBDistance);
			if (STDistance > SBDistance){SpawnSide = "Top";}
			else if (STDistance < SBDistance){SpawnSide = "Bottom";}
		}
		else
		{
			Node2D SpawnL = BoundBox.SpawnLeft;
			Node2D SpawnR = BoundBox.SpawnRight;
			float SLDistance = this.Position.DistanceTo(SpawnL.Position);
			float SRDistance = this.Position.DistanceTo(SpawnR.Position);
			//GD.Print("Left is this far: " + SLDistance);
			//GD.Print("Right is this far: " + SRDistance);
			if (SLDistance > SRDistance){SpawnSide = "Left";}
			else if (SLDistance < SRDistance){SpawnSide = "Right";}
			//GD.Print("Entering on side: " + SpawnSide);
		}
	}
	
	private void _CheckChunkDistances(int CheckChunks, bool VertTrans)
	{
		if (CheckChunks > 0)
		{
			Node2D CurrentChunk = BoundBox.GetNode<Node2D>("Chunk" + CheckChunks);
			float ChDistance = this.Position.DistanceTo(CurrentChunk.Position);
			//GD.Print("Chunk is this far: " + ChDistance);
			if (VertTrans == true)
			{
				if (ChDistance == 96){FromChunk = CheckChunks;}
				else{CheckChunks -= 1;_CheckChunkDistances(CheckChunks, VertTrans);}
			}
			else
			{
				if (ChDistance == 160){FromChunk = CheckChunks;}
				else{CheckChunks -= 1;_CheckChunkDistances(CheckChunks, VertTrans);}
			}
		}
	}
	
	public void _SendInfo(CharacterBody2D body)
	{
		//GD.Print("ExtraMaps: " + ExtraMaps);
		//This will take the stored information and send it to
		//the MapManager upon the Player entering the Area2D.
		MapMan.CollectInfo(SpawnSide, NextMap, ToChunk,
						CurrentMap, FromChunk, VertTrans, ExtraMaps);
	}
}
