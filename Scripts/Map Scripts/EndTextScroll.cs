using Godot;
using System;

public partial class EndTextScroll : Node2D
{
	float ScrollSpeed;
	bool ScrollText;
	AudioStreamPlayer Music;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScrollSpeed = 0.1f;
		ScrollText = false;
		Music = GetNode<AudioStreamPlayer>("/root/GameScene/MvmSongIdea");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ScrollText == true)
		{
			this.Position = new Vector2(this.Position.X, this.Position.Y - ScrollSpeed);
			Music.Stop();
		}
	}
	
	public void _CheckPlayer(CharacterBody2D Player)
	{
		ScrollText = true;
	}
}
