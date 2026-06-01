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
		MapMan = GetNode<MapManager>("/root/MapManager");
	}
	
	public void _SendInfo(CharacterBody2D body)
	{
		//This will take the stored information and send it to
		//the MapManager once it's ready
		MapMan.ProcessInfo(NewSpawn, NextMap, CurrentMap, VertTrans);
	}
}
