using Godot;
using System;

public partial class MvmSongIdea : AudioStreamPlayer
{
	[Export]
	AudioStream OriginalSong;
	[Export]
	AudioStream SingLark;
	
	Node2D GameScene;
	string SingLarkRoom;
	string EndRoom;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameScene = GetNode<Node2D>("..");
		SingLarkRoom = "ForestSave1";
		EndRoom = "Forest23";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SwitchSong()
	{
		foreach(Node child in GameScene.GetChildren())
		{
			AudioStream CurrentSong = this.Stream;
			if (child.Name == SingLarkRoom)
			{
				this.SetStream(SingLark);
				this.Play();
			}
			else if (child.Name == EndRoom)
			{
				this.SetStream(SingLark);
			}
			else if (CurrentSong != OriginalSong)
			{
				this.SetStream(OriginalSong);
				this.Play();
			}
		}
	}
	
	private void SongFinished()
	{
		// Looping the simple song
		this.Play();
	}
}
