using Godot;
using System;

public partial class ExampleBasicEnemy : CharacterBody2D
{
	





	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		
		MoveAndSlide();
	}
}
