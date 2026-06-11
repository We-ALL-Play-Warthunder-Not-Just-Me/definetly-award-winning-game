using Godot;
using System;

public partial class RoomSetUp : Sprite2D
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
	public int ChunksMade;
	Node2D BoundBox;
	Marker2D TopLeft;
	Marker2D BottomRight;
	public Node2D SpawnLeft;
	public Node2D SpawnRight;
	public Node2D SpawnTop;
	public Node2D SpawnBottom;
	float StartX;
	float StartY;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//This is just grabbing all relevant information for later.
		BoundBox = this;
		TopLeft = GetNode<Marker2D>("TopLeft");
		BottomRight = GetNode<Marker2D>("BottomRight");
		SpawnLeft = GetNode<Node2D>("SpawnLeft");
		SpawnRight = GetNode<Node2D>("SpawnRight");
		SpawnTop = GetNode<Node2D>("SpawnTop");
		SpawnBottom = GetNode<Node2D>("SpawnBottom");
		//This is for centering the chunks.
		StartX = (-this.Scale.X/2) + (ChunkSizeX/2);
		StartY = (-this.Scale.Y/2) + (ChunkSizeY/2);
		
		//Runs the code to create the Chunks as soon as the Map exists.
		CalculateChunks();
		//Runs the code to set up the Cardinal Spawn points of the Map.
		SetUpSpawns();
		//Runs the code to set up the Camera Restraints for the Map.
		SetUpRestraints();
	}
	
	//The Brains of my Chunk Maker. It's beautiful...
	private void CalculateChunks()
	{
		//Calculates how many Chunks exist Vertically and Horizontally.
		HoriChunks = (int)this.Scale.X / ChunkSizeX;
		VertChunks = (int)this.Scale.Y / ChunkSizeY;
		//A simple Vector2 to start us up at the Top Left of the map.
		//GD.Print("StartX: " + StartX + " StartY: " + StartY);
		ChunksMade = HoriChunks * VertChunks;
		Vector2 StartingPoint = new Vector2(StartX, StartY);
		
		//Inputing the relevant information for Chunk Creation.
		CreateChunks(HoriChunks, VertChunks, StartingPoint, 1);
	}
	
	//Just a bunch of fucking math to create the Chunk Markers
	//for the sake of better Map Spawning and moving.
	private void CreateChunks(int HC, int VC, Vector2 SP, int NC)
	{
		//Basically setting the Rows of Chunks left to do.
		if (VC > 0)
		{
			//Basically setting the Collumns of Chunks left to do.
			if (HC > 0)
			{
				//All of this handles putting in the Chunk Markers
				//for how many Horizontal Collumns exist in this Row.
				Marker2D Marker = new Marker2D();
				if (HC == HoriChunks) {Marker.Position = SP;}
				else {SP.X = SP.X + ChunkSizeX;Marker.Position = SP;}
				//This is just naming the Chunk accordingly, starting
				//from 1 and ticking up until the function is over.
				Marker.Name = "Chunk" + NC;
				NC += 1;
				//Adds the newly made and named Chunk Marker to
				//ChunkBounds before checking how many collumns
				//are left and repeating itself until 0.
				this.AddChild(Marker);
				HC -= 1;
				CreateChunks(HC, VC, SP, NC);
			}
			else
			{
				//Resets the Starting Point of the Collumns like
				//a type writer. Pretty simple.
				SP.X = StartX;
				//This moves the row down one Chunk and proceeds to
				//loop on itself until it's gone through all rows.
				SP.Y = SP.Y + ChunkSizeY;
				VC -= 1;
				CreateChunks(HoriChunks, VC, SP, NC);
			}
		}
	}
	
	private void SetUpRestraints()
	{
		//All of this just calculates where to put each marker on the
		//map automatically so you don't have to do it manually.
		var BBEdge = new Vector2(BoundBox.Scale.X/2, BoundBox.Scale.Y/2);
		TopLeft.Position = new Vector2(-BBEdge.X, -BBEdge.Y);
		BottomRight.Position = new Vector2(BBEdge.X, BBEdge.Y - 12);
		//That -12 is just to keep the small rooms in a 320/180 restraint
		//and to keep the camera fairly even across all maps.
	}
	
	private void SetUpSpawns()
	{
		//All of this just calculates where to put each point on the
		//map automatically so you don't have to do it manually.
		var BBEdge = new Vector2(BoundBox.Scale.X/2, BoundBox.Scale.Y/2);
		SpawnLeft.Position = new Vector2(-(BBEdge.X-10), 0);
		SpawnRight.Position = new Vector2((BBEdge.X-10), 0);
		SpawnTop.Position = new Vector2(0, -(BBEdge.Y-20));
		SpawnBottom.Position = new Vector2(0, (BBEdge.Y-20));
		//Those minus 10s and 20s are padding for dodging transitioners so
		//you're not endlessly bouncing between two rooms. There's a better way
		//of doing this, I'm sure.
	}
}
