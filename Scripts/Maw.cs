using Godot;
using System;

public partial class Maw : Sprite2D
{
	[Export]
	public bool IsSproutMaw;
	public AnimationPlayer MawAnimations;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MawAnimations = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsSproutMaw)
		{
			MawAnimations.Play("SproutMawIdle");
		}
		else
		{
			MawAnimations.Play("MawIdle");
		}
	}
}
