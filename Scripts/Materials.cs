using Godot;
using System;

public partial class Materials : Sprite2D
{
	Sprite2D MaterialSprite;
	Area2D MaterialArea;
	CollisionShape2D MaterialCollision;
	float FrameTime = 48;
	Vector2 NewOffset;
	float CountDown;
	AudioStreamPlayer PickupNoise;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaterialSprite = this;
		MaterialArea = GetNode<Area2D>("Area2D");
		MaterialCollision = MaterialArea.GetNode<CollisionShape2D>("CollisionShape2D");
		PickupNoise = GetNode<AudioStreamPlayer>("Pickup");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// MANUAL ANIMATIONS! (This is so dumb... But it works)
		CountDown -= 1;
		if (CountDown > (FrameTime/2))
		{
			// Resetting the Sprite to it's original Offset
			NewOffset.Y = 0;
			MaterialSprite.SetOffset(NewOffset);
		}
		else if (CountDown > 0)
		{
			// Moving the Sprite down 2 Pixels
			NewOffset.Y = 2;
			MaterialSprite.SetOffset(NewOffset);
		}
		else
		{
			// Resetting the FrameTime
			CountDown = FrameTime;
		}
	}
	private void OnArea2DBodyEntered(CharacterBody2D body)
	{
		GD.Print("AAAAAAAAHHHH");
		PickupNoise.Play();
		this.Visible = false;
		MaterialCollision.SetDeferred("disabled", true);
	}
	
	private void NoiseFinished()
	{
		QueueFree();
	}
}
