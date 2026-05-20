using Godot;
using System;

public partial class Player : CharacterBody2D
{

	enum PlayerState
	{
		IDLE,
		WALK,
		JUMP,
		CLIMB,
		FALL,
		LAND,
		HURT
	}


	public const float SPEED = 300.0f;
	public const float JUMPVELOCITY = -200.0f;
	public const float GRAVITY = 700.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JUMPVELOCITY;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * SPEED;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
