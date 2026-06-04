using Godot;
using System;

public partial class MapTransitioner : Area2D
{
	//The tedious variables that need filling per transitioner on scene...
	[Export]
	public string SpawnSide;
	[Export]
	public string NextMap;
	[Export]
	public int ToChunk;
	[Export]
	public string CurrentMap;
	[Export]
	public int FromChunk;
	[Export]
	public bool VertTrans;
	
	//Empty Values
	MapManager MapMan;
	CollisionShape2D CollLR;
	CollisionShape2D CollUD;
	
	public override void _Ready()
	{
		//This grabs the global MapManager to communicate with.
		MapMan = GetNode<MapManager>("/root/MapManager");
		//Getting the Collisions for swapping purposes.
		CollLR = GetNode<CollisionShape2D>("TransitionerLeftRight");
		CollUD = GetNode<CollisionShape2D>("TransitionerUpDown");
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
	
	public void _SendInfo(CharacterBody2D body)
	{
		//This will take the stored information and send it to
		//the MapManager upon the Player entering the Area2D.
		MapMan.CollectInfo(SpawnSide, NextMap, ToChunk,
							CurrentMap, FromChunk, VertTrans);
	}
}
