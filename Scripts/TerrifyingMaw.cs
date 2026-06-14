using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class TerrifyingMaw : CharacterBody2D
{
	[Export] public float Speed = 50.0f;
    [Export] public float DashSpeed = 100.0f;
    [Export] public float JumpVelocity = -400.0f;
	[Export] public double DashJump = -100f;
	[Export] public double DashDuration = 1.0f;
    [Export] public double FallSpeed = 200f;
	[Export] public double MeleeRange = 50f;
	[Export] public double RangedRange = 150f;
	[Export] public double OutOfRangeRange = 500f;
	[Export] public double UndetectedRange = 1000f;
	[Export] public double PassiveSpeedMultiplier = 0.5;
	[Export] public int HP = 100;



	private enum EnvironmentState
	{
		GROUNDED,
		AIRBORN
	}
	private enum MoveState
	{
		IDLE,
		MOVINGLEFT,
		MOVINGRIGHT,
		DASHINGLEFT,
		DASHINGRIGHT,
		BOUNCING
	}

	private enum JumpState
	{
		IDLE,
		DASHING,
		JUMPING,
		FALLING,
	}

	private enum AggessionState
	{
		PASSIVE,
		ALERTED,
	}

	private enum AttackState
	{
		IDLE,
		DASHING,
		BITING,
		RECOVERING
	}

	private enum TargetDistance
	{
		MELEE,
		RANGED,
		OUTOFRANGE,
		UNDETECTED
	}
	private EnvironmentState ces = EnvironmentState.GROUNDED;
	private MoveState cms = MoveState.IDLE;
	private JumpState cjs = JumpState.IDLE;
	private AggessionState cas = AggessionState.PASSIVE;
	private AttackState cats = AttackState.IDLE;
	private TargetDistance tds = TargetDistance.UNDETECTED;

	[Export]public RichTextLabel l;


	private Node2D Target;

	public override void _Ready()
	{
		Target = (Node2D)GetTree().GetFirstNodeInGroup("Player");
	}


	private void _process_agression_state()
	{
		if(tds != TargetDistance.OUTOFRANGE && cas == AggessionState.PASSIVE)
		{
			PassiveSpeedMultiplier = 1;
			cas = AggessionState.ALERTED;
		}
		else if(tds >= TargetDistance.UNDETECTED && cas == AggessionState.ALERTED)
		{
			PassiveSpeedMultiplier = 0.5;
			cas = AggessionState.PASSIVE;
		}
	}

	//simple enough that we don't need a special _change and _enact for this. 
	private double distance_to_target = 10000;
	private void _process_target_distance(double delta, ref Vector2 velocity)
	{
		distance_to_target = this.GlobalPosition.DistanceTo(Target.GlobalPosition);
		switch (distance_to_target)
		{
			//double check does this work? don't yell at me I used var and I hate it
			case var value when value < MeleeRange:
				tds = TargetDistance.MELEE;
				break;
			case var value when value < RangedRange:
				tds = TargetDistance.RANGED;
				break;
			case var value when value < OutOfRangeRange:
				tds= TargetDistance.OUTOFRANGE;
				break;
			case var value when value >= UndetectedRange:
				tds = TargetDistance.UNDETECTED;
				break;
		}
		_text_helper();
	}

	//HAHAHA I MADE IT ONE LINE LOSERS. Its hella simple and just calles change environmental state depending on if on ground or not
	private void _process_environmental_state(double delta, ref Vector2 velocity)
	{if (this.IsOnFloor()){_change_environmental_state(EnvironmentState.GROUNDED);}else{_change_environmental_state(EnvironmentState.AIRBORN);}}

	private void _change_environmental_state(EnvironmentState e)
	{
		if (ces == e)
		{  return; }

		switch (e)
		{
			case(EnvironmentState.GROUNDED):
				if(this.IsOnFloor())
				{
					ces = EnvironmentState.GROUNDED;
				}
				break;
			case (EnvironmentState.AIRBORN):
				if(!this.IsOnFloor())
				{
					ces = EnvironmentState.AIRBORN;
				}
				break;
		}
	}

	private void _enact_environmental_state(double delta, ref Vector2 velocity)
	{
		if (ces == EnvironmentState.AIRBORN)
		{
			velocity.Y += (float)FallSpeed * (float) delta;
		}

	}


	private double _wander_time = 0;
	private double _dash_time = -3;
	private void _process_move_state(double delta, ref Vector2 velocity)
	{
		//if we are passive we just wander a little
		if(cas == AggessionState.PASSIVE)
		{
			//for the duration travel in a particular direction unless they hit a wall 
			_wander_time -= delta;
			//if wander time is 0 lets pick another direction to wander
			if (_wander_time <= 0)
			{
				_wander_time = GD.RandRange(1.0, 3.0);
				int direction = GD.RandRange(-1, 1);
				if (direction == 0)
				{
					_change_move_state(MoveState.IDLE);
				}
				else if (direction == 1)
				{
					_change_move_state(MoveState.MOVINGRIGHT);
				}
				else
				{
					_change_move_state(MoveState.MOVINGLEFT);
				}

			}
			//if we hit a wall lets go the other way
			if(this.IsOnWall())
			{
				GD.Print("bonk");
				if(this.GetWallNormal().X > 0)
				{
					_change_move_state(MoveState.MOVINGRIGHT);
				}
				else
				{
					_change_move_state(MoveState.MOVINGLEFT);
				}
			}

		}

		//if we are alerted to the enemy we will run at them until we are close
		if(cas == AggessionState.ALERTED)
		{
			//if we are not already dashing, and we are not recharging our dash, we jump at them
			if((cms != MoveState.DASHINGLEFT || cms != MoveState.DASHINGRIGHT) && tds == TargetDistance.RANGED && (_dash_time <= -3 || _dash_time >= 0))
			{
                if (_illegal_direction_helper() == -1)
                {
                    _change_move_state(MoveState.DASHINGLEFT);
                }
                else if (_illegal_direction_helper() == 1)
                {
                    _change_move_state(MoveState.DASHINGRIGHT);
                }
            }
			// we are far away, lets walk at them
			else if (tds != TargetDistance.MELEE && tds != TargetDistance.RANGED)
			{
				_change_move_state(_direction_helper());
			}
			//oh no, we hit a wall, lets bounce!
			else if ((cms == MoveState.DASHINGLEFT || cms == MoveState.DASHINGRIGHT) && this.IsOnWall())
			{
                if (this.GetWallNormal().X > 0)
                {
                    _change_move_state(MoveState.DASHINGRIGHT);
                }
                else
                {
                    _change_move_state(MoveState.DASHINGLEFT);
                }
            }
			
		}
	}

	private void _change_move_state(MoveState s)
	{
		if(cms == s)
		{
			return;
		}
		switch(s)
		{
			case MoveState.IDLE:
				cms = MoveState.IDLE;
				break;
			case MoveState.MOVINGRIGHT: 
				cms = MoveState.MOVINGRIGHT; 
				break;
			case MoveState.MOVINGLEFT: 
				cms = MoveState.MOVINGLEFT; 
				break;
			case MoveState.DASHINGRIGHT:
                GD.Print("Checking dash time: " + _dash_time);
                if (cms != MoveState.DASHINGLEFT && cms != MoveState.DASHINGRIGHT && _dash_time <= 3)
                {

                    _dash_time = DashDuration;
                    GD.Print("Time now: " + _dash_time);
                    cms = MoveState.DASHINGRIGHT;
                }
                if (_dash_time >= 0)
				{
                    cms = MoveState.DASHINGRIGHT;
                }
                break;
            case MoveState.DASHINGLEFT:
                GD.Print("Checking dash time: " + _dash_time);
                if (cms != MoveState.DASHINGLEFT && cms != MoveState.DASHINGRIGHT && _dash_time <= 3)
                {

                    _dash_time = DashDuration;
                    GD.Print("Time now: " + _dash_time);
                    cms = MoveState.DASHINGLEFT;
                }
                if (_dash_time >= 0)
                {
                    cms = MoveState.DASHINGLEFT;
                }
                break;
            case MoveState.BOUNCING: 
				cms = MoveState.BOUNCING; 
				break;
		}
	}


	private void _enact_move_state(double delta, ref Vector2 velocity) 
	{
        _dash_time -= delta;
        GD.Print("Time now: : " + _dash_time);
        switch (cms)
		{
			case MoveState.IDLE:
				velocity.X = 0;
				break;
			case MoveState.MOVINGLEFT:
				velocity.X = -Speed * (float)PassiveSpeedMultiplier;
				break;
			case MoveState.MOVINGRIGHT:
                velocity.X = Speed * (float)PassiveSpeedMultiplier;
                break;
			case MoveState.DASHINGLEFT:
				velocity.X = -DashSpeed;
				break;
            case MoveState.DASHINGRIGHT:
                velocity.X = DashSpeed;
                break;
        }
		GD.Print(cms.ToString());
	}

	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		_process_environmental_state(delta, ref velocity);
		_process_target_distance(delta, ref velocity);
		_process_agression_state();
		_process_move_state(delta, ref velocity);



		_enact_environmental_state(delta, ref velocity);
		_enact_move_state(delta, ref velocity);
		Velocity = velocity;
		MoveAndSlide();
	}


	public void _text_helper()
	{
		l.Text = cms.ToString();
	}

	//a little janky. Returns a movestate of the direction the player is in.

    private MoveState _direction_helper()
    {
        if (Target == null)
        {
            return 0;
        }
        Vector2 directionToTarget = this.GlobalPosition.DirectionTo(Target.GlobalPosition);
        if (directionToTarget.X < 0)
        {
            return MoveState.MOVINGLEFT;
        }
        else
        {
            return MoveState.MOVINGRIGHT;
        }
    }
	private int _illegal_direction_helper()
	{
		MoveState m = _direction_helper();

		if (m == MoveState.MOVINGLEFT) { return -1; }
		else
		{
			return 1;

		}
	}
}
