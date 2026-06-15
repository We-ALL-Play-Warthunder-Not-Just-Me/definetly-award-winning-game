using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		COMBO1,
		COMBO2,
		BLOCKING,
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

	public void dealDamage(float damage, int direction)
	{
		if(parry_timer>= 0)
		{
			//we will do something here later. Hopefully
		}
		else if(pas == PlayerAttackState.BLOCKING)
		{
            GD.Print(Health_Bar.hurt(damage/2));
        }
		else
		{
            GD.Print(Health_Bar.hurt(damage));
        }
	}
	//Constants
	public const float SPEED = 120.0f;
	public const float RUNSPEED = 200.0f;
	public const float DASHSPEED = 350.0f;
	public const float JUMPVELOCITY = -225.0f;
	public const float GRAVITY = 500.0f;
	public const double COYOTEPERIOD = 0.15;
	
	//other thingies
	[Export] public double AttackRecoveryTimer = 0.3f;
    [Export] public double AttackTimer = 0.6f;
	[Export] public double ParryTimer = 0.7f;//this is generous for testing purposes
	[Export] public float BlockingSpeedMultiplier;
	private float Knockback = 0;
	private float MovementModifier = 1;
	private int PlayerDirection = 1;

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
	[Export] public Area2D Punch1;
	private Punch1 AttackScript;

	//Random Health Bar class
	[Export]public HealthBar Health_Bar;

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
		if(Health_Bar == null) Health_Bar = GetNode<HealthBar>("CanvasLayer/HealthBar");
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
		AttackScript = Punch1 as Punch1;
	}

	//this will eventually be changed to be part of the environmental state process fuction
	//this will eventually be changed to be part of the environmental state process fuction
	//lies
	private void _apply_gravity(double delta, ref Vector2 velocity)
	{
		if (es == EnvironmentalState.COYOTE || es == EnvironmentalState.AIRBORN)
		{
			velocity.Y += GRAVITY * (float)delta;
		}
	}


	//check if there are events that might change your environmental state
	private double coyote_timer = 0.0;
	//Player Environmental State checker. This is constantly checking weather the player is in the air or on the ground, or just walked off a ledge
	//no other state limits what the player environmental state can be making this pretty simple.
	private void _pes(ref Vector2 v, double delta)
	{
		if (PlayerObject.IsOnFloor())
		{
			_ces(EnvironmentalState.GROUNDED);
		}
		//if the player walks off something they would no longer be on the floor, but would not have yet lost the grounded state.
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
			//honestly I dunno, do nothing atm
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

	//check if there are events that might change your jumping state.
	
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

    //Jump State Jumping N, Falling N, Landing N, None Y,
    //Environmental state Grounded Y Coyote Y Airborn N
    //

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
			if(direction.X > 0)
			{
				PlayerDirection = 1;
			}
			else
			{
				PlayerDirection = -1;
			}
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
				velocity.X = direction.X * SPEED * MovementModifier;
				break;
			case (PlayerMoveState.RUN):
				velocity.X = direction.X * RUNSPEED * MovementModifier;
				break;
			case (PlayerMoveState.DASH):
				velocity.X = direction.X * DASHSPEED * MovementModifier;
				break;
		}	
	}

	private void _pas(double delta, ref Vector2 velocity)
	{
		if (Input.IsActionJustPressed("Attack") )
		{
            
            if (attack_timer <= 0.5 && attack_timer > 0)
			{
				
				attack_queued = true;
			}
			else
			{
				if(pas == PlayerAttackState.ATTACKING)
				{
					
					_cas(PlayerAttackState.COMBO1, ref velocity);
				}
				else if(pas == PlayerAttackState.COMBO1)
				{
				
					_cas(PlayerAttackState.COMBO2, ref  velocity);
				}
				else
				{
					
                    _cas(PlayerAttackState.ATTACKING, ref velocity);
                }
            }
		}
		else if (Input.IsActionJustPressed("Defend") || Input.IsActionPressed("Defend"))
		{
			_cas(PlayerAttackState.BLOCKING, ref velocity);
		}
		else
		{
			_cas(PlayerAttackState.IDLE, ref velocity);
		}
	}


	private double recovery_timer = 0;
	private double attack_timer = 0;
	private double parry_timer = 0;

	//I would like to make it so if someone spam clicks they can attack on cooldown without having to have hit the button RIGHT as the timer expires
	//currently not implemented
	private bool attack_queued = false;
	private void _cas(PlayerAttackState c, ref Vector2 velocity)
	{
		//essentially we should not be changing this state while recovering
		if (pas == c || recovery_timer >= 0)
		{
			return;
		}

		switch (c)
		{
			case PlayerAttackState.ATTACKING:
				if(pms is PlayerMoveState.IDLE or PlayerMoveState.WALK && ps is PlayerState.FINE && pas is not PlayerAttackState.ATTACKING or PlayerAttackState.BLOCKING or PlayerAttackState.RECOVERING)
				{

					attack_queued = false;
					attack_timer = AttackTimer;
					MovementModifier = 0.4f;
					pas = PlayerAttackState.ATTACKING;

				}
				break;
			case PlayerAttackState.COMBO1:
				if(attack_timer <= 0 && attack_queued)
				{
                    attack_queued = false;
                    attack_timer = AttackTimer;
                    MovementModifier = 0.4f;
                    pas = PlayerAttackState.COMBO1;
				}
				break;
			case PlayerAttackState.COMBO2:
				if(attack_timer <= 0 && attack_queued)
				{
                    attack_queued = false;
                    attack_timer = AttackTimer;
                    MovementModifier = 0.2f;
                    pas = PlayerAttackState.COMBO2;
				}
				break;
			case PlayerAttackState.BLOCKING:
                if (pms is PlayerMoveState.IDLE or PlayerMoveState.WALK && ps is PlayerState.FINE)
                {
					MovementModifier = 0.2f;
					parry_timer = ParryTimer;
					pas = PlayerAttackState.BLOCKING;
                }
                break;
			case PlayerAttackState.IDLE:
				
				if(recovery_timer <= 0 && attack_timer <= 0)
				{
					MovementModifier = 1.0f;
                    pas = PlayerAttackState.IDLE;
                }
				break;
			case PlayerAttackState.RECOVERING:
				recovery_timer = AttackRecoveryTimer;
                MovementModifier = 1.0f;
                pas = PlayerAttackState.RECOVERING;
				break;
		}

	}

	private void _attack_state_helper(bool startorfinish)
	{

		//Punch1.Call("_activate", PlayerDirection, );
	}
	private void _enact_attack_state(double delta, ref Vector2 velocity)
	{
		attack_timer -= delta;
		recovery_timer -= delta;
		parry_timer -= delta;
		switch(pas)
		{
			case PlayerAttackState.ATTACKING:
				if(attack_timer <= 0 && attack_queued)
				{
					_cas(PlayerAttackState.COMBO1, ref velocity);
				}
                else if (attack_timer <= 0)
                {
                    _cas(PlayerAttackState.RECOVERING, ref velocity);
                }
                break;
			case PlayerAttackState.COMBO1:
                if (attack_timer <= 0 && attack_queued)
                {
                    _cas(PlayerAttackState.COMBO2, ref velocity);
                }
				else if(attack_timer <= 0)
				{
					_cas(PlayerAttackState.RECOVERING, ref velocity);
				}
				break;
			case PlayerAttackState.COMBO2:
				if(attack_timer <= 0)
				{
					_cas(PlayerAttackState.RECOVERING, ref velocity);
				}
				break;
            case PlayerAttackState.BLOCKING:

				break;

			case PlayerAttackState.RECOVERING:

				break;

			case PlayerAttackState.IDLE:
				//nothing happens
				break;
		}

		if(pas == PlayerAttackState.ATTACKING)
		{
			attack_timer-=delta;
		}
		else if(pas == PlayerAttackState.RECOVERING)
		{
			recovery_timer -=delta;
			if(recovery_timer <= 0)
			{
				
				_cas(PlayerAttackState.IDLE, ref velocity);
                //change state to idle, if the player had an attack qeued up then we q the attack up again
				if (attack_queued)
                {
                    attack_queued = false;
                    _cas(PlayerAttackState.ATTACKING, ref velocity);
                }
            }
		}
		else if (pas == PlayerAttackState.BLOCKING)
		{
			parry_timer -=delta;
		}
		//not much to do if you are idle, yet....

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
		_pas(delta, ref velocity);
		
		_animation_handler(delta, ref velocity);
		
		_enact_environmental_state(delta, ref velocity);
		_enact_jump_state(delta, ref velocity);
		_enact_move_state(delta, ref velocity);
		_enact_attack_state(delta, ref velocity);
		Velocity = velocity;
		_playerXSpeed_helper();
		_playerYSpeed_helper();
		_attack_text_helper();

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
