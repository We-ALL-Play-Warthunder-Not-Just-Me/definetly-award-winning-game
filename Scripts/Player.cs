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
	public const float SPEED = 160.0f;
	public const float DASHSPEED = 350.0f;
	public const float JUMPVELOCITY = -240.0f;
	public const float GRAVITY = 500.0f;

	//player parts
	public CharacterBody2D PlayerObject;
	public Sprite2D PlayerSprite;
	public CollisionShape2D PlayerCollisionShape;
	public RichTextLabel PlayerStateLabel;
	public RichTextLabel PlayerMoveStateLabel;
	public RichTextLabel PlayerJumpStateLabel;
	public RichTextLabel EnvironmentalStateLabel;
	public RichTextLabel PlayerAttackStateLabel;
	public RichTextLabel PlayerXSpeedLabel;
	public RichTextLabel PlayerYSpeedLabel;
	public AnimationPlayer PlayerAnimations;

	//Random Health Bar class
	public HealthBar Health_Bar;

	public PlayerMoveState pms = PlayerMoveState.IDLE;
	public PlayerMoveState pms = PlayerMoveState.IDLE;
	public PlayerState ps = PlayerState.FINE;
	public PlayerJumpState pjs = PlayerJumpState.FALLING;
	public EnvironmentalState es = EnvironmentalState.AIRBORN;
	public PlayerAttackState pas = PlayerAttackState.IDLE;
	public EnvironmentalState es = EnvironmentalState.AIRBORN;
	public PlayerAttackState pas = PlayerAttackState.IDLE;

	// starting off we set all the labels and get all the player components we need.
	public override void _Ready()
	{
		PlayerObject = this;
		PlayerSprite = GetNode<Sprite2D>("PlayerSprite2D");
		PlayerCollisionShape = GetNode<CollisionShape2D>("PlayerCollisionShape2D");
		PlayerAnimations = GetNode<AnimationPlayer>("AnimationPlayer");
		PlayerStateLabel = GetNode<RichTextLabel>("PlayerStateLabel");
		_player_text_helper();
		PlayerMoveStateLabel = GetNode<RichTextLabel>("PlayerMoveStateLabel");
		_move_text_helper();
		PlayerJumpStateLabel = GetNode<RichTextLabel>("PlayerJumpStateLabel");
		_jump_text_helper();
		EnvironmentalStateLabel = GetNode<RichTextLabel>("EnvironmentalStateLabel");
		_environmental_text_helper();
		PlayerAttackStateLabel = GetNode<RichTextLabel>("PlayerAttackStateLabel");
		_attack_text_helper();
		PlayerXSpeedLabel = GetNode<RichTextLabel>("XSpeed");
		PlayerYSpeedLabel = GetNode<RichTextLabel>("YSpeed");
	}

	//this will eventually be changed to be part of the environmental state process fuction
	//this will eventually be changed to be part of the environmental state process fuction
	private void _apply_gravity(double delta, ref Vector2 velocity)
	{
		if (!PlayerObject.IsOnFloor())
		{
			velocity.Y += GRAVITY * (float)delta;
		}
	}

	//returns true if state changes and false if state is the same as before.
	private bool _change_Environmental_State(EnvironmentalState s)
	{
		if (s == es)
		{
			return false;
		}
		switch (s)
		{
			case (EnvironmentalState.GROUNDED):
				// don't change the environmental state to grounded if we are not on the ground
				if(PlayerObject.IsOnFloor())
				{
					es = s;
					break;
				}
				else
				{
					return false;
				}
			case (EnvironmentalState.AIRBORN):
				es = s;
				break;
			case (EnvironmentalState.LIQUID):
				es = s;
				break;
			case (EnvironmentalState.WALL):
				es = s;
				break;
		}
		_environmental_text_helper();
		return true;
	}

	//returns true if state changes and false if state is the same as before.
	private bool _change_Jump_State(PlayerJumpState s)
	{
		if (s == pjs)
		{
			return false;
		}
		switch(s)
		{
			case (PlayerJumpState.NONE):
				pjs = s;
				break;
			case (PlayerJumpState.JUMPING):
				pjs = s;
				break;
			case (PlayerJumpState.FALLING):
				pjs = s;
				break;
			case (PlayerJumpState.LANDING):
				pjs = s;
				break;

		}
		_jump_text_helper();
		return true;
	}

	//returns true if state changes and false it has not changed.
	private bool _change_Move_State(PlayerMoveState s)
	{
		if (s == pms)
		{
			return false;
		}
		switch (s)
		{
			case (PlayerMoveState.IDLE):
				pms = s;
				break;
			case (PlayerMoveState.WALK):
				pms = s;
				break;
			case (PlayerMoveState.RUN):
				// player can only run if we are on the ground (assuming we want that)
				if (es == EnvironmentalState.GROUNDED)
				{
					pms = s;
					break;
				}
				else
				{
					return false;
				}
			case (PlayerMoveState.DASH):
				pms = s;
				break;
		}
		_move_text_helper();
		return true;
	}

	// determines if the player is jumping, falling, or landing. Probably mostly for handling animations if we attach them.
	// also handles variable jump height if the player lets go of the jump button early.
	private void _process_jump_state(double delta, ref Vector2 velocity)
	{
		//if we are on the ground and press jump we jump
		if (Input.IsActionJustPressed("Jump") && es == EnvironmentalState.GROUNDED)
		{
			_change_Jump_State(PlayerJumpState.JUMPING);
			velocity.Y = JUMPVELOCITY;
		}
		//if the player is jumping and the velocity is less than 0 we are now falling
		else if(es == EnvironmentalState.AIRBORN && pjs == PlayerJumpState.JUMPING && velocity.Y > 0.0)
		{
			_change_Jump_State(PlayerJumpState.FALLING);
		}
		//if the player is jumping and lets go of the jump button we are now falling, this allows for variable jump height.
		else if (es == EnvironmentalState.AIRBORN && pjs == PlayerJumpState.JUMPING && !Input.IsActionPressed("Jump"))
		{
			velocity.Y = 0;
			_change_Jump_State(PlayerJumpState.FALLING);
		}
		//if the player is falling and hits the ground we are now landing
		else if (pjs == PlayerJumpState.FALLING && es == EnvironmentalState.GROUNDED)
		{
			_change_Jump_State(PlayerJumpState.LANDING);
		}
		//if nothing else is going on and we are on the ground then we are doing nothing
		else if (es == EnvironmentalState.GROUNDED)
		{
			_change_Jump_State(PlayerJumpState.NONE);
		}
	}
	//this handles player movement
	private void _process_move_state(double delta, ref Vector2 velocity)
	{
		//get if the player is pressing the move buttons
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		//if they are pressing the move buttons then move
		if (direction != Vector2.Zero)
		{
			_change_Move_State(PlayerMoveState.WALK);
			velocity.X = direction.X * SPEED;
		}
		//if they were moving and let go of the controls then slow down to a stop, this might be wierd with controllers if theres no deadzone
		else if (pms == PlayerMoveState.IDLE && direction == Vector2.Zero)
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, SPEED);
		} 
		// if nothing is happening make sure we are idle
		else 
		{
			_change_Move_State(PlayerMoveState.IDLE);
		}   
	}

	private void _process_environmental_state(double delta, ref Vector2 velocity)
	{
		//right now we just check if the player is on the floor or not.
		if (PlayerObject.IsOnFloor())
		{
			_change_Environmental_State(EnvironmentalState.GROUNDED);
		}
		else
		{
			_change_Environmental_State(EnvironmentalState.AIRBORN);
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
		_process_environmental_state(delta, ref velocity);
		_process_move_state(delta, ref velocity);
		_process_jump_state(delta, ref velocity);
		_apply_gravity(delta, ref velocity);
		_animation_handler(delta, ref velocity);
		Velocity = velocity;
		_playerXSpeed_helper();
		_playerYSpeed_helper();

		MoveAndSlide();
		
	}



	// these are helpers that update the text labels onscreen.
	private void _jump_text_helper()
	{
		PlayerJumpStateLabel.Text = "Jump State: " + pjs.ToString();
	}
	private void _player_text_helper()
	{
		PlayerStateLabel.Text = "Player State: " + ps.ToString();
	}
	private void _environmental_text_helper()
	{
		EnvironmentalStateLabel.Text = "Environmental State: " + es.ToString();
	}
	private void _move_text_helper()
	{
		PlayerMoveStateLabel.Text = "Move State: " + pms.ToString();
	}
	private void _attack_text_helper()
	{
		PlayerAttackStateLabel.Text = "Attack State: " + pas.ToString();
	}
	private void _playerXSpeed_helper()
	{
		PlayerXSpeedLabel.Text = "X: " + Velocity.X.ToString();
	}
	private void _playerYSpeed_helper()
	{
		PlayerYSpeedLabel.Text = "Y: " + Velocity.Y.ToString();
	}
}
