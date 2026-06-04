using Godot;
using System;

public partial class MapTransitioner : Area2D
{
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
	
	MapManager MapMan;
	CollisionShape2D CollLR;
	CollisionShape2D CollUD;
	
	public override void _Ready()
	{
		//This grabs the global MapManager to communicate with.
		MapMan = GetNode<MapManager>("/root/MapManager");
		CollLR = GetNode<CollisionShape2D>("TransitionerLeftRight");
		CollUD = GetNode<CollisionShape2D>("TransitionerUpDown");
	}
	
	public override void _Process(double delta)
	{
		if (VertTrans == true)
		{
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
		else
		{
			CollLR.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			CollUD.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}
	
	public void _SendInfo(CharacterBody2D body)
	{
		//This will take the stored information and send it to
		//the MapManager upon the Player entering the Area2D.
		MapMan.ProcessInfo(SpawnSide, NextMap, ToChunk,
							CurrentMap, FromChunk, VertTrans);
	}
}
