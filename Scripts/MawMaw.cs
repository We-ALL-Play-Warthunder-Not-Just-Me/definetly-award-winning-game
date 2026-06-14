using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class MawMaw : CharacterBody2D
{
	[Export] public float Speed = 10.0f;
    [Export] public float DashSpeed = 100.0f;
    [Export] public float JumpVelocity = -400.0f;
	[Export] public double DashJump = -100f;
	[Export] public double DashDuration = 1.0f;
    [Export] public double DashRecovery = 2.0f;
    [Export] public double FallSpeed = 200f;
	[Export] public double MeleeRange = 50f;
	[Export] public double RangedRange = 150f;
	[Export] public double OutOfRangeRange = 200f;
	[Export] public double UndetectedRange = 400f;
	[Export] public double PassiveSpeedMultiplier = 0.5;
    [Export] public double WanderingMinimum = 1.0;
    [Export] public double WanderingMaximum = 3.0;
    [Export] public double MeleeDuration = 0.8f;
    [Export] public double MeleeWindup = 0.3f;
    [Export] public double MeleeRecovery = 1.5f;
    [Export] public int HP = 100;
    [Export] public double MeleeAttackOffsetX=4;
    [Export] public double MeleeAttackOffsetY=1;
    [Export] public RichTextLabel l;
    [Export] public PackedScene MeleeAttack1;
    [Export] public PackedScene MeleeAttack2;

    private enum State
	{
		IDLING,
		INTERESTED,
		DASHING,
		HURT,
        MELEEEXHAUST,
        MELEEING1WINDUP,
		MELEEING1,
        MELEEING1RECOVERY, 
        MELEEING2WINDUP,
        MELEEING2,
        MELEEING2RECOVERY,
        
	}

    private enum TargetDistance
    {
        MELEE,
        RANGED,
        OUTOFRANGE,
        UNDETECTED
    }
    private Node2D Target;
    private double distance_to_target;
    //also used by melee attacks to choose their direction
    private int dash_direction;
    private Vector2 target_direction;
    private State current_state = State.IDLING;
    private TargetDistance current_target_distance = TargetDistance.UNDETECTED;

    //timers
    private double _dashing_time = 0;
    private double _dash_recovery_time = 0;
    private double _wandering_time = 0;
    private double _melee_time = 0;
    private double _melee_recovery_time = 0;
    private double _melee_windup_time = 0;

    public override void _Ready()
    {
        Target = (Node2D)GetTree().GetFirstNodeInGroup("Player");
    }

    private void _process_idle_state(double delta, ref Vector2 velocity)
    {

    }

    private void _process_interested_state(double delta, ref Vector2 velocity)
    {
        if (current_state == State.INTERESTED)
        {
            velocity.X = Speed * target_direction_helper();
        }
        
    }

    private void _process_dashing_state(double delta, ref Vector2 velocity)
    {
        _dashing_time -= delta;
        _dash_recovery_time -= delta;
        if(current_state == State.DASHING)
        {
            if (this.IsOnWall())
            {
                dash_direction = dash_direction * -1;
            }
            velocity.X = DashSpeed * dash_direction;
        }
        if(_dashing_time <= 0 && (int)current_state <=3)
        {
            change_state(State.INTERESTED, ref velocity);
        }
    }



    private void _process_melee_state(double delta, ref Vector2 velocity)
    {
        _melee_recovery_time -= delta;
        _melee_time -= delta;
        _melee_windup_time -= delta;
        switch (current_state)
        {
            case State.MELEEING1WINDUP:
                velocity.X = Speed * dash_direction * 0.2f;
                if (_melee_windup_time <= 0)
                {
                    change_state(State.MELEEING1, ref velocity);
                }
                break;
            case State.MELEEING1:
                //Wierd scaling jump forwards
                velocity.X = Speed * dash_direction * ((float)_melee_time * 0.8f);
                
                if (_melee_time <= 0)
                {
                    change_state(State.MELEEING1RECOVERY, ref velocity);
                }
                break;
            case State.MELEEING1RECOVERY:
                if(_melee_recovery_time<=0)
                {
                    change_state(State.MELEEING2WINDUP, ref velocity);
                }
                break;
            case State.MELEEING2WINDUP:
                velocity.X = Speed * dash_direction * 0.2f;
                if (_melee_windup_time <= 0)
                {
                    change_state(State.MELEEING2, ref velocity);
                }
                break;
            case State.MELEEING2:
                //Wierd scaling jump forwards
                velocity.X = Speed * dash_direction * ((float)_melee_time * 0.3f);
                if (_melee_time <= 0)
                {
                    change_state(State.MELEEING2RECOVERY, ref velocity);
                }
                break;
            case State.MELEEING2RECOVERY:
                if (_melee_recovery_time <= 0)
                {
                    change_state(State.MELEEING2WINDUP, ref velocity);
                }
                break;
        }
    }
    private void change_state(State switchto, ref Vector2 velocity)
    {
        //I don't think we need to switch to the same state
        if (current_state == switchto)
        {
            return;
        }

        switch (switchto)
        {
            case State.IDLING:
                //if we are mid dash we don't wana suddenly snap out of it
                if(_dashing_time < 0)
                {
                    _wandering_time = WanderingMinimum;
                    current_state = State.IDLING;
                }
                break;
            case State.INTERESTED:
                current_state = State.INTERESTED;
                break;
            case State.DASHING:
                if(_dash_recovery_time <= 0)
                {
                    dash_direction = target_direction_helper();
                    _dashing_time = DashDuration;
                    _dash_recovery_time = DashRecovery;
                    velocity.Y = (float)DashJump;
                    current_state = State.DASHING;
                }
                break;
            case State.MELEEING1WINDUP:
                //if we are not exhausted and not dashing we can start meleeing
                if(current_state != State.MELEEEXHAUST && current_state != State.DASHING && _melee_recovery_time <= 0)
                {
                    GD.Print("woooo");
                    _melee_windup_time = MeleeWindup;
                    dash_direction = target_direction_helper();
                    current_state = State.MELEEING1WINDUP;
                }
                break;
            case State.MELEEING1:
                
                if(current_state == State.MELEEING1WINDUP)
                {
                    GD.Print("Noooo");
                    velocity.Y = -30;
                    _melee_time = MeleeDuration;
                    current_state = State.MELEEING1;
                    MawMawBite biteinstance = MeleeAttack1.Instantiate() as MawMawBite;
                    Vector2 position = this.GlobalPosition;
                    position.X += (float)MeleeAttackOffsetX * dash_direction;
                    position.Y += (float)MeleeAttackOffsetY;
                    biteinstance._begin(dash_direction, (Speed * ((float)_melee_time * 0.8f)), (float)MeleeDuration);
                    this.AddChild(biteinstance);
                }
                break;
            case State.MELEEING1RECOVERY:
                if(current_state == State.MELEEING1)
                {
                    _melee_recovery_time = MeleeRecovery;
                    current_state = State.MELEEING1RECOVERY;
                }
                break;

            case State.MELEEING2WINDUP:
                if(current_state == State.MELEEING1RECOVERY)
                {
                    _melee_windup_time = MeleeWindup * 0.8;
                    current_state = State.MELEEING2WINDUP;
                }
                break;
            case State.MELEEING2:
                if (current_state == State.MELEEING2WINDUP)
                {
                    GD.Print("Noooo");
                    velocity.Y = -30;
                    _melee_time = MeleeDuration;
                    current_state = State.MELEEING2;
                    MawMawBite biteinstance = MeleeAttack1.Instantiate() as MawMawBite;
                    Vector2 position = this.GlobalPosition;
                    position.X += (float)MeleeAttackOffsetX * dash_direction;
                    position.Y += (float)MeleeAttackOffsetY;
                    biteinstance._begin(dash_direction, (Speed * ((float)_melee_time * 4f)), (float)MeleeDuration+0.3f);
                    this.AddChild(biteinstance);
                }
                break;
            case State.MELEEING2RECOVERY:
                if (_melee_time <= 0)
                {
                    _melee_recovery_time = MeleeRecovery;
                    current_state = State.IDLING;
                }
                break;
            
        }

    }
    private void _process_target_distance(double delta, ref Vector2 velocity)
    {
        distance_to_target = this.GlobalPosition.DistanceTo(Target.GlobalPosition);
        switch (distance_to_target)
        {
            //double check does this work? don't yell at me I used var and I hate it
            case var value when value < MeleeRange:
                current_target_distance = TargetDistance.MELEE;
                break;
            case var value when value < RangedRange:
                current_target_distance = TargetDistance.RANGED;
                break;
            case var value when value < OutOfRangeRange:
                current_target_distance = TargetDistance.OUTOFRANGE;
                break;
            case var value when value >= UndetectedRange:
                current_target_distance = TargetDistance.UNDETECTED;
                break;
        }
    }

    private void _pick_state(double delta, ref Vector2 velocity)
    {
        //if we are in the attacking zone we don't want to check these other things
        if ((int)current_state >= 3)
        {
            return;
        }
        //ok the player has gone too far, become idle
        if(current_target_distance == TargetDistance.UNDETECTED)
        {
           change_state(State.IDLING, ref velocity);
        }
        else if (current_state != State.DASHING && current_target_distance == TargetDistance.MELEE)
        {
            change_state(State.MELEEING1WINDUP, ref velocity);
        }
        //if the target is far away, or is close, but the dash cooldown is going then we walk at them
        else if(current_target_distance == TargetDistance.OUTOFRANGE || (current_target_distance == TargetDistance.RANGED && _dash_recovery_time >=0 && _dashing_time <=0))
        {
            change_state(State.INTERESTED, ref velocity);
        }
        //if the target is in range and we are not on a dash cooldown timer, lets dash at them
        else if(current_target_distance == TargetDistance.RANGED && _dash_recovery_time < 0)
        {
            change_state(State.DASHING, ref velocity);
        } 
        
    }
    private void _apply_gravity(double delta, ref Vector2 velocity)
    {
        if (!this.IsOnFloor())
        {
            velocity.Y += (float)FallSpeed * (float)delta;
        }
    }
    public override void _PhysicsProcess(double delta)
    {
       
        target_direction = this.GlobalPosition.DirectionTo(Target.GlobalPosition);
        Vector2 velocity = Velocity;
        
        _process_target_distance(delta, ref velocity);
        _pick_state(delta, ref velocity);
        _process_melee_state(delta, ref velocity);
        _process_dashing_state(delta, ref velocity);
        _process_interested_state(delta, ref velocity);
        _process_idle_state(delta, ref velocity);
        _text_helper();
        _apply_gravity(delta, ref velocity);

        Velocity = velocity;
        //GD.Print(_dashing_time + " : Dash time, " + _dash_recovery_time + " : dash recov");
        MoveAndSlide();
        
    }

    private int target_direction_helper()
    {
        if (target_direction.X <= 0)
            return -1;
        else
            return 1;
    }
    public void _text_helper()
    {
        l.Text = "State: " + current_state.ToString() + " Target : " + current_target_distance.ToString();
    }
   



    //   private enum EnvironmentState
    //{
    //	GROUNDED,
    //	AIRBORN
    //}
    //private enum MoveState
    //{
    //	IDLE,
    //	MOVINGLEFT,
    //	MOVINGRIGHT,
    //	DASHINGLEFT,
    //	DASHINGRIGHT,
    //	BOUNCING
    //}

    //private enum JumpState
    //{
    //	IDLE,
    //	DASHING,
    //	JUMPING,
    //	FALLING,
    //}

    //private enum AggessionState
    //{
    //	PASSIVE,
    //	ALERTED,
    //}

    //private enum AttackState
    //{
    //	IDLE,
    //	DASHING,
    //	BITING,
    //	RECOVERING
    //}

    //private enum TargetDistance
    //{
    //	MELEE,
    //	RANGED,
    //	OUTOFRANGE,
    //	UNDETECTED
    //}
    //private EnvironmentState ces = EnvironmentState.GROUNDED;
    //private MoveState cms = MoveState.IDLE;
    //private JumpState cjs = JumpState.IDLE;
    //private AggessionState cas = AggessionState.PASSIVE;
    //private AttackState cats = AttackState.IDLE;
    //private TargetDistance tds = TargetDistance.UNDETECTED;

    //   [Export]public RichTextLabel l;


    //private Node2D Target;

    //   public override void _Ready()
    //   {
    //	Target = (Node2D)GetTree().GetFirstNodeInGroup("Player");
    //   }


    //private void _process_agression_state()
    //{
    //	if(tds != TargetDistance.OUTOFRANGE && cas == AggessionState.PASSIVE)
    //	{
    //		PassiveSpeedMultiplier = 1;
    //		cas = AggessionState.ALERTED;
    //	}
    //	else if(tds >= TargetDistance.UNDETECTED && cas == AggessionState.ALERTED)
    //	{
    //		PassiveSpeedMultiplier = 0.5;
    //		cas = AggessionState.PASSIVE;
    //	}
    //}

    ////simple enough that we don't need a special _change and _enact for this. 
    //private double distance_to_target = 10000;
    //private void _process_target_distance(double delta, ref Vector2 velocity)
    //{
    //	distance_to_target = this.GlobalPosition.DistanceTo(Target.GlobalPosition);
    //	switch (distance_to_target)
    //	{
    //		//double check does this work? don't yell at me I used var and I hate it
    //		case var value when value < MeleeRange:
    //			tds = TargetDistance.MELEE;
    //			break;
    //		case var value when value < RangedRange:
    //			tds = TargetDistance.RANGED;
    //			break;
    //		case var value when value < OutOfRangeRange:
    //			tds= TargetDistance.OUTOFRANGE;
    //			break;
    //		case var value when value >= UndetectedRange:
    //			tds = TargetDistance.UNDETECTED;
    //			break;
    //	}
    //	_text_helper();
    //}

    ////HAHAHA I MADE IT ONE LINE LOSERS. Its hella simple and just calles change environmental state depending on if on ground or not
    //   private void _process_environmental_state(double delta, ref Vector2 velocity)
    //{if (this.IsOnFloor()){_change_environmental_state(EnvironmentState.GROUNDED);}else{_change_environmental_state(EnvironmentState.AIRBORN);}}

    //private void _change_environmental_state(EnvironmentState e)
    //{
    //	if (ces == e)
    //	{  return; }

    //	switch (e)
    //	{
    //		case(EnvironmentState.GROUNDED):
    //			if(this.IsOnFloor())
    //			{
    //				ces = EnvironmentState.GROUNDED;
    //			}
    //			break;
    //		case (EnvironmentState.AIRBORN):
    //			if(!this.IsOnFloor())
    //			{
    //				ces = EnvironmentState.AIRBORN;
    //			}
    //			break;
    //	}
    //}

    //private void _enact_environmental_state(double delta, ref Vector2 velocity)
    //{
    //	if (ces == EnvironmentState.AIRBORN)
    //	{
    //		velocity.Y += (float)FallSpeed * (float) delta;
    //	}

    //}


    //private double _wander_time = 0;
    //private double _dash_time = -3;
    //private void _process_move_state(double delta, ref Vector2 velocity)
    //{
    //	//if we are passive we just wander a little
    //	if(cas == AggessionState.PASSIVE)
    //	{
    //		//for the duration travel in a particular direction unless they hit a wall 
    //		_wander_time -= delta;
    //		//if wander time is 0 lets pick another direction to wander
    //		if (_wander_time <= 0)
    //		{
    //			_wander_time = GD.RandRange(1.0, 3.0);
    //               int direction = GD.RandRange(-1, 1);
    //			if (direction == 0)
    //			{
    //				_change_move_state(MoveState.IDLE);
    //			}
    //			else if (direction == 1)
    //			{
    //				_change_move_state(MoveState.MOVINGRIGHT);
    //			}
    //			else
    //			{
    //				_change_move_state(MoveState.MOVINGLEFT);
    //			}

    //           }
    //		//if we hit a wall lets go the other way
    //		if(this.IsOnWall())
    //		{
    //			GD.Print("bonk");
    //			if(this.GetWallNormal().X > 0)
    //			{
    //				_change_move_state(MoveState.MOVINGRIGHT);
    //			}
    //			else
    //			{
    //                   _change_move_state(MoveState.MOVINGLEFT);
    //               }
    //		}

    //	}

    //	//if we are alerted to the enemy we will run at them until we are close
    //	if(cas == AggessionState.ALERTED)
    //	{
    //		//if we are not already dashing, and we are not recharging our dash, we jump at them
    //		if((cms != MoveState.DASHINGLEFT || cms != MoveState.DASHINGRIGHT) && tds == TargetDistance.RANGED && (_dash_time <= -3 || _dash_time >= 0))
    //		{
    //               if (_illegal_direction_helper() == -1)
    //               {
    //                   _change_move_state(MoveState.DASHINGLEFT);
    //               }
    //               else if (_illegal_direction_helper() == 1)
    //               {
    //                   _change_move_state(MoveState.DASHINGRIGHT);
    //               }
    //           }
    //		// we are far away, lets walk at them
    //		else if (tds != TargetDistance.MELEE && tds != TargetDistance.RANGED)
    //		{
    //			_change_move_state(_direction_helper());
    //		}
    //		//oh no, we hit a wall, lets bounce!
    //		else if ((cms == MoveState.DASHINGLEFT || cms == MoveState.DASHINGRIGHT) && this.IsOnWall())
    //		{
    //               if (this.GetWallNormal().X > 0)
    //               {
    //                   _change_move_state(MoveState.DASHINGRIGHT);
    //               }
    //               else
    //               {
    //                   _change_move_state(MoveState.DASHINGLEFT);
    //               }
    //           }


    //	}
    //}

    //private void _change_move_state(MoveState s)
    //{
    //	if(cms == s)
    //	{
    //		return;
    //	}
    //	switch(s)
    //	{
    //		case MoveState.IDLE:
    //			cms = MoveState.IDLE;
    //			break;
    //		case MoveState.MOVINGRIGHT: 
    //			cms = MoveState.MOVINGRIGHT; 
    //			break;
    //		case MoveState.MOVINGLEFT: 
    //			cms = MoveState.MOVINGLEFT; 
    //			break;
    //		case MoveState.DASHINGRIGHT:
    //               GD.Print("Checking dash time: " + _dash_time);
    //               if (cms != MoveState.DASHINGLEFT && cms != MoveState.DASHINGRIGHT && _dash_time <= 3)
    //               {

    //                   _dash_time = DashDuration;
    //                   GD.Print("Time now: " + _dash_time);
    //                   cms = MoveState.DASHINGRIGHT;
    //               }
    //               if (_dash_time >= 0)
    //			{
    //                   cms = MoveState.DASHINGRIGHT;
    //               }
    //               break;
    //           case MoveState.DASHINGLEFT:
    //               GD.Print("Checking dash time: " + _dash_time);
    //               if (cms != MoveState.DASHINGLEFT && cms != MoveState.DASHINGRIGHT && _dash_time <= 3)
    //               {

    //                   _dash_time = DashDuration;
    //                   GD.Print("Time now: " + _dash_time);
    //                   cms = MoveState.DASHINGLEFT;
    //               }
    //               if (_dash_time >= 0)
    //               {
    //                   cms = MoveState.DASHINGLEFT;
    //               }
    //               break;
    //           case MoveState.BOUNCING: 
    //			cms = MoveState.BOUNCING; 
    //			break;
    //	}
    //}


    //private void _enact_move_state(double delta, ref Vector2 velocity) 
    //{
    //       _dash_time -= delta;
    //       GD.Print("Time now: : " + _dash_time);
    //       switch (cms)
    //	{
    //		case MoveState.IDLE:
    //			velocity.X = 0;
    //			break;
    //		case MoveState.MOVINGLEFT:
    //			velocity.X = -Speed * (float)PassiveSpeedMultiplier;
    //			break;
    //		case MoveState.MOVINGRIGHT:
    //               velocity.X = Speed * (float)PassiveSpeedMultiplier;
    //               break;
    //		case MoveState.DASHINGLEFT:
    //			velocity.X = -DashSpeed;
    //			break;
    //           case MoveState.DASHINGRIGHT:
    //               velocity.X = DashSpeed;
    //               break;
    //       }
    //	GD.Print(cms.ToString());
    //}


    //public override void _PhysicsProcess(double delta)
    //{
    //	Vector2 velocity = Velocity;
    //	_process_environmental_state(delta, ref velocity);
    //	_process_target_distance(delta, ref velocity);
    //	_process_agression_state();
    //	_process_move_state(delta, ref velocity);



    //	_enact_environmental_state(delta, ref velocity);
    //	_enact_move_state(delta, ref velocity);
    //	Velocity = velocity;
    //	MoveAndSlide();
    //}


    //public void _text_helper()
    //{
    //	l.Text = cms.ToString();
    //}

    ////a little janky. Returns a movestate of the direction the player is in.
    //   private MoveState _direction_helper()
    //   {
    //       if (Target == null)
    //       {
    //           return 0;
    //       }
    //       Vector2 directionToTarget = this.GlobalPosition.DirectionTo(Target.GlobalPosition);
    //       if (directionToTarget.X < 0)
    //       {
    //           return MoveState.MOVINGLEFT;
    //       }
    //       else
    //       {
    //           return MoveState.MOVINGRIGHT;
    //       }
    //   }
    //private int _illegal_direction_helper()
    //{
    //	MoveState m = _direction_helper();

    //	if (m == MoveState.MOVINGLEFT) { return -1; }
    //	else
    //	{
    //		return 1;
    //	}
    //}
}




