using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class TerrifyingMaw : CharacterBody2D
{
	[Export] public float Speed = 300.0f;
	[Export] public float JumpVelocity = -400.0f;
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
		DASHING,
		BOUNCING
	}

	private enum JumpState
	{
		IDLE,
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

		//if we are alerted to the enemy we will run at them
		if(cas == AggessionState.ALERTED)
		{

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
			case MoveState.DASHING: 
				cms = MoveState.DASHING; 
				break;
			case MoveState.BOUNCING: 
				cms = MoveState.BOUNCING; 
				break;
		}
	}


	private void _enact_move_state(double delta, ref Vector2 velocity) 
	{ 
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
        }
		GD.Print(cms.ToString());
	}

	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		_process_environmental_state(delta, ref velocity);
		_process_target_distance(delta, ref velocity);
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
}




