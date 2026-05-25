using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{

	public enum PlayerMoveState
	{
		IDLE,
		WALK,
		RUN,
		DASH
	}

	public enum PlayerJumpState
	{
		NONE,
		JUMPING,
		FALLING,
		LANDING
	}
	public enum PlayerState
	{
		HURT,
		RECOVING,
		FINE
	}

	public enum  PlayerAttackState
	{
		ATTACKING,    
		RECOVERING,
		IDLE
	}

	public enum EnvironmentalState
	{
		GROUNDED,
		AIRBORN,
		LIQUID,
		WALL//Uncertain if this should be here
	}


	//Constants
	public const float SPEED = 120.0f;
	public const float JUMPVELOCITY = -240.0f;
	public const float GRAVITY = 600.0f;

	//player parts
	public CharacterBody2D PlayerObject;
	public Sprite2D PlayerSprite;
	public CollisionShape2D PlayerCollisionShape;
	public AnimationPlayer PlayerAnimations;

	public PlayerMoveState pms = PlayerMoveState.IDLE; 
	public PlayerState ps = PlayerState.FINE;
	public PlayerJumpState pjs = PlayerJumpState.FALLING;
	public EnvironmentalState es = EnvironmentalState.AIRBORN;

	public override void _Ready()
	{
		PlayerObject = this;
			//GetNode<CharacterBody2D>("Player");
		PlayerSprite = GetNode<Sprite2D>("PlayerSprite2D");
		PlayerCollisionShape = GetNode<CollisionShape2D>("PlayerCollisionShape2D");
		PlayerAnimations = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private void _apply_gravity(double delta, ref Vector2 velocity)
	{
		if (!PlayerObject.IsOnFloor())
		{
			velocity.Y += GRAVITY * (float)delta;
		}
	}


	//returns true if state changes and false if state is the same as before, currently not implemented.
	private bool _change_Jump_State(PlayerJumpState s, bool t)
	{
		if (s == pjs)
		{
			return false;
		}
		switch(s)
		{
			case (PlayerJumpState.NONE):
				pjs = s;
				return true;
			case (PlayerJumpState.JUMPING):
				pjs = s;
				return true;
			case (PlayerJumpState.FALLING):
				pjs = s;
				return true;
			case (PlayerJumpState.LANDING):
				pjs = s;
				return true;
		}
		return false;
	}

	private void _process_jump_state(double delta, ref Vector2 velocity)
	{
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
		{
			velocity.Y = JUMPVELOCITY;
		}
	}
	//returns true if state changes and false if state is the same as before, currently not implemented.
	private bool _change_Move_State(PlayerMoveState s, bool t)
	{
		if (s == pms)
		{
			return false;
		}
		switch (s)
		{
			case (PlayerMoveState.IDLE):
				pms = s;
				return true;
			case(PlayerMoveState.WALK):
				pms = s;
				return true;
		}
		return false;
	}
	private void _process_move_state(double delta, ref Vector2 velocity)
	{
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			_change_Move_State(PlayerMoveState.WALK, true);
			velocity.X = direction.X * SPEED;
		}
		else if (pms == PlayerMoveState.IDLE)
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, SPEED);
		} 
		else 
		{
			_change_Move_State(PlayerMoveState.IDLE, true);
		}   
	}

	private void _process_environmental_state(double delta)
	{
		if (PlayerObject.IsOnFloor())
		{
			//_change_State(PlayerState.GROUNDED)
		}
	}
	
	private void _animation_handler(double delta, ref Vector2 velocity)
	{
		//Animation Handler?
		
		//This is Flipping the Player Sprite depending on  Velocity.
		//(This is NOT a solid way to do it. Will fix later.)
		if (velocity.X < 0)
		{
			PlayerSprite.SetFlipH(true);
		}
		else if (velocity.X > 0)
		{
			PlayerSprite.SetFlipH(false);
		}
		
		//This is handling switching between movement animations.
		//This is ALSO not ideal. Might need tweeking.
		if (velocity.X == 0 && PlayerObject.IsOnFloor())
		{
			PlayerAnimations.Play("PlayerIdle");
		}
		else if (velocity.X != 0 && PlayerObject.IsOnFloor())
		{
			PlayerAnimations.Play("PlayerRun");
		}
		else if (velocity.Y < 0)
		{
			PlayerAnimations.Play("PlayerJump");
		}
		else if (velocity.Y > 0)
		{
			PlayerAnimations.Play("PlayerFall");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		_process_environmental_state(delta);
		_process_move_state(delta, ref velocity);
		_process_jump_state(delta, ref velocity);
		_apply_gravity(delta, ref velocity);
		_animation_handler(delta, ref velocity);


		Velocity = velocity;
		MoveAndSlide();
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
