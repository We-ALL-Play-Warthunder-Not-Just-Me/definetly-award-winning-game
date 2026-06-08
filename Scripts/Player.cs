using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D, DamagableEntity
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
		COYOTE,
		LIQUID,
		WALL//Uncertain if this should be here
	}

	public void dealDamage(float damage)
    {
		//change the player state here
        //Health_Bar.setcurrenthealth(Health_Bar.Value - damage);
		GD.Print(Health_Bar.hurt(damage));
    }
    //Constants
    public const float SPEED = 120.0f;
	public const float RUNSPEED = 200.0f;
	public const float DASHSPEED = 350.0f;
	public const float JUMPVELOCITY = -225.0f;
	public const float GRAVITY = 500.0f;
	public const double COYOTEPERIOD = 0.5;

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
	public PlayerState ps = PlayerState.FINE;
	public PlayerJumpState pjs = PlayerJumpState.FALLING;
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
		Health_Bar = GetNode<HealthBar>("HealthBar");
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
		if (es == EnvironmentalState.COYOTE || es == EnvironmentalState.AIRBORN)
		{
			velocity.Y += GRAVITY * (float)delta;
		}
	}


	//check if there are events that might change your environmental state
	private double coyote_timer = 0.0;
	private void _pes(ref Vector2 v, double delta)
	{
		if (PlayerObject.IsOnFloor())
		{
			_ces(EnvironmentalState.GROUNDED);
		}
		//if the player walks off something they would no longer be on hte floor, but would not have yet lost the grounded state.
		if(!PlayerObject.IsOnFloor() && es == EnvironmentalState.GROUNDED && pjs != PlayerJumpState.JUMPING)
		{
			_ces(EnvironmentalState.COYOTE);
		}
		else if (!PlayerObject.IsOnFloor())
		{
			_ces(EnvironmentalState.AIRBORN);
		}
		else
		{
			GD.Print("FUCKING ERROR IDIOT");
		}
	}

	//change your environmental state, 
	private void _ces(EnvironmentalState s)
	{
		if(s == es) return;
        
		switch(s)
		{
			case (EnvironmentalState.GROUNDED):
				coyote_timer = 0;
				es = EnvironmentalState.GROUNDED;
				break;
			case (EnvironmentalState.COYOTE):
				es = EnvironmentalState.COYOTE;
				coyote_timer = COYOTEPERIOD;
				break;
			case (EnvironmentalState.AIRBORN):
				if(coyote_timer > 0)
				{
					break;
				}
				es = EnvironmentalState.AIRBORN;
				break;
		}
		 _environmental_text_helper();
	}

	//enacts the environmental state
	private void _enact_environmental_state(double delta, ref Vector2 velocity)
	{
		switch(es)
		{
			case (EnvironmentalState.GROUNDED):
				//right now we don't need to do anthing
				break;
			case (EnvironmentalState.COYOTE):
				coyote_timer -= delta;
				_apply_gravity(delta, ref velocity);
				if(coyote_timer <= 0 || pjs == PlayerJumpState.JUMPING)
				{
					_ces(EnvironmentalState.AIRBORN);
				}
				break;
			case (EnvironmentalState.AIRBORN):
				_apply_gravity(delta, ref velocity);
				break;
		}
	}

	//check if there are events that might change your jumping state
	private void _pjs(ref Vector2 v)
	{
		//lets try to jump.
		if (Input.IsActionJustPressed("Jump"))
		{
			_cjs(PlayerJumpState.JUMPING, ref v);
		}
		//if velocity is less than 0 we are now falling even if we werent jumping.
		if(es == EnvironmentalState.AIRBORN && this.GetRealVelocity().Y > 0.0)
		{
			_cjs(PlayerJumpState.FALLING, ref v);
		}
		//if the player is jumping and lets go of the jump button we are now falling, this allows for variable jump height.
		if (pjs == PlayerJumpState.JUMPING && !Input.IsActionPressed("Jump"))
		{
			_cjs(PlayerJumpState.FALLING, ref v);
		}
		//if the player is falling and hits the ground we are now landing. Right now not much is going on with landing, but
		//this could be used to trigger landing animations or something like that.
		if (pjs == PlayerJumpState.FALLING && es == EnvironmentalState.GROUNDED)
		{
			_cjs(PlayerJumpState.LANDING, ref v);
			//since landing doesnt do anything we actually just want none
			_cjs(PlayerJumpState.NONE, ref v);
		}
	}

	private void _cjs(PlayerJumpState s, ref Vector2 v)
	{
		if(s == pjs) return;
		
		switch(s)
		{
			case (PlayerJumpState.NONE):
				pjs = PlayerJumpState.NONE;
				break;
			case (PlayerJumpState.JUMPING):
			//we only want to start jumping if your on the ground or in coyote mode 
				if(es == EnvironmentalState.GROUNDED || es == EnvironmentalState.COYOTE)
				{
					pjs = PlayerJumpState.JUMPING;	
					v.Y = JUMPVELOCITY;
				}
				break;
			case (PlayerJumpState.FALLING):
				//we were jumping and we let go of the jump button so we begin to fall, this allows for variable jump height.
				if(pjs == PlayerJumpState.JUMPING && !Input.IsActionPressed("Jump") && v.Y < 0)
				{
					//if we are still going upwards I want to keep some of that momentum so its a little smoother transition to falling
					//instead of just losing all upward momentum and falling
					if(v.Y < -100)	
					{
						v.Y = -100;
					}	
					pjs = PlayerJumpState.FALLING;	
				} else if (v.Y >= 0)
                {
                    pjs = PlayerJumpState.FALLING;
                }
                break;
			case (PlayerJumpState.LANDING):
				//if the player aint falling then we don't land, eventually we will want to add a timer if this feature is included
				if(pjs != PlayerJumpState.FALLING) break;
				pjs = PlayerJumpState.LANDING;
				break;
		}
		 _jump_text_helper();
	}

	private void _enact_jump_state(double delta, ref Vector2 velocity)
	{
		switch(pjs)
		{
			//I dunno exactly what would go here, but I am sure in testing it will come up. Especially landing.
			//a gravity is handled by player state.
			case (PlayerJumpState.NONE):
				break;
			case (PlayerJumpState.JUMPING):
				break;
			case (PlayerJumpState.FALLING):
				break;
			case (PlayerJumpState.LANDING):
				//landing animation and -time
				break;
		}
	}

	private void _ppms(ref Vector2 v)
	{
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		// we will want to check for dash and run inputs evntually.
		if (direction != Vector2.Zero)
		{
			_cpms(PlayerMoveState.WALK, ref v);
		}
		else 
		{
			_cpms(PlayerMoveState.IDLE, ref v);
		}   
	}

	private void _cpms(PlayerMoveState s, ref Vector2 v)
	{
		if(s == pms) return;
		
		switch (s)
		{
			case (PlayerMoveState.IDLE):
				pms = PlayerMoveState.IDLE;
				break;
			case (PlayerMoveState.WALK):
				pms = PlayerMoveState.WALK;
				break;
			case (PlayerMoveState.RUN):
				if(es == EnvironmentalState.GROUNDED)
				{
					pms = PlayerMoveState.RUN;
				}
				break;
			case (PlayerMoveState.DASH):
				pms = PlayerMoveState.DASH;
				break;
		}
		_move_text_helper();
	}

	private void _enact_move_state(double delta, ref Vector2 velocity)
	{
		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		switch (pms)
		{
			case (PlayerMoveState.IDLE):
				velocity.X = Mathf.MoveToward(velocity.X, 0, SPEED);
				break;
			case (PlayerMoveState.WALK):
				velocity.X = direction.X * SPEED;
				break;
			case (PlayerMoveState.RUN):
				velocity.X = direction.X * RUNSPEED;
				break;
			case (PlayerMoveState.DASH):
				velocity.X = direction.X * DASHSPEED;
				break;
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
		//(This is ALSO not ideal. Might need tweeking.)
		if (velocity.X == 0 && PlayerObject.IsOnFloor())
		{
			PlayerAnimations.Play("PlayerIdle");
		}
		else if (this.GetRealVelocity().X != 0 && PlayerObject.IsOnFloor())
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
		_pes(ref velocity, delta);
		_ppms(ref velocity);
		_pjs(ref velocity);
		
		_animation_handler(delta, ref velocity);
		
		_enact_environmental_state(delta, ref velocity);
		_enact_jump_state(delta, ref velocity);
		_enact_move_state(delta, ref velocity);
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
