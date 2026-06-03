using Godot;
using System;

public partial class ChunkMaker : Sprite2D
{
	//You set these in the Inspector to determine your base Room Size.
	//Why? 'Cause I said so, it made sense to me at the time.
	[Export]
	int ChunkSizeX;
	[Export]
	int ChunkSizeY;
	
	//Empty Values
	int HoriChunks;
	int VertChunks;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Runs the code to create the Chunks as soon as the Map exists.
		CalculateChunks();
	}
	
	//The Brains of my Chunk Maker. It's beautiful...
	private void CalculateChunks()
	{
		//Calculates how many Chunks exist Vertically and Horizontally.
		HoriChunks = (int)this.Scale.X / ChunkSizeX;
		VertChunks = (int)this.Scale.Y / ChunkSizeY;
		//A simple Vector2 to start us up at the Top Left of the map.
		Vector2 StartingPoint = new Vector2(-this.Scale.X/2, -this.Scale.Y/2);
		
		//Inputing the relevant information for Chunk Creation.
		CreateChunks(HoriChunks, HoriChunks, VertChunks, 0, StartingPoint, 1);
	}
	
	//Just a bunch of fucking math to create the Chunk Markers
	//for the sake of better Map Spawning and moving.
	private void CreateChunks(int HCMax, int HC, int VC, int MC, Vector2 SP, int NC)
	{
		//Basically setting the Rows of Chunks left to do.
		int VCLeft = VC;
		if (VCLeft > 0)
		{
			//Basically setting the Collumns of Chunks left to do.
			int HCLeft = HC;
			if (HCLeft > 0)
			{
				//All of this handles putting in the Chunk Markers
				//for how many Horizontal Collumns exist in this Row.
				Marker2D Marker = new Marker2D();
				if (MC > 0) {SP.X = SP.X + ChunkSizeX;Marker.Position = SP;}
				else {Marker.Position = SP;}
				MC += 1;
				//This is just naming the Chunk accordingly, starting
				//from 1 and ticking up until the function is over.
				Marker.Name = "Chunk" + NC;
				NC += 1;
				//Adds the newly made and named Chunk Marker to
				//ChunkBounds before checking how many collumns
				//are left and repeating itself until 0.
				this.AddChild(Marker);
				HCLeft -= 1;
				CreateChunks(HCMax, HCLeft, VCLeft, MC, SP, NC);
			}
			else
			{
				//Resets the Starting Point of the Collumns like
				//a type writer. Pretty simple.
				MC = 0;
				SP.X = -this.Scale.X/2;
				//This moves the row down one Chunk and proceeds to
				//loop on itself until it's gone through all rows.
				SP.Y = SP.Y + ChunkSizeY;
				VCLeft -= 1;
				CreateChunks(HCMax, HCMax, VCLeft, MC, SP, NC);
			}
		}
	}
}
