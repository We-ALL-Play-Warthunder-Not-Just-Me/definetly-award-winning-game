using Godot;
using System;

public partial class MapTransitioner : Area2D
{
	[Export]
	public int NewSpawn;
	[Export]
	public string NextMap;
	[Export]
	public string CurrentMap;
	[Export]
	public bool VertTrans;
	MapManager MapMan;
	
	public override void _Ready()
	{
		//This grabs the global MapManager to communicate with.
		MapMan = GetNode<MapManager>("/root/MapManager");
	}
	
	public void _SendInfo(CharacterBody2D body)
	{
		//This will take the stored information and send it to
		//the MapManager upon the Player entering the Area2D.
		MapMan.ProcessInfo(NewSpawn, NextMap, CurrentMap, VertTrans);
	}
}
