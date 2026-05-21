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


	public CharacterBody2D PlayerObject;
    public Sprite2D PlayerSprite;
    public CollisionShape2D PlayerCollisionShape;

    public override void _Ready()
    {
        PlayerObject = GetNode<CharacterBody2D>("Player");
		PlayerSprite = GetNode<Sprite2D>("PlayerSprite2D");
		PlayerCollisionShape = GetNode<CollisionShape2D>("PlayerCollisionShape2D");
    }

	private void _apply_gravity(double delta)
	{
		if (!PlayerObject.IsOnFloor())
		{
			Vector2 velocity = Velocity;
            velocity += GetGravity() * (float)delta;
            Velocity = velocity;
        }
	}

	private void _process_state(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		_apply_gravity(delta);

		/*
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
		MoveAndSlide();*/
	}
}
