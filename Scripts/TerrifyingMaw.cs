using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class TerrifyingMaw : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	[Export] public double MeleeRange = 50f;
	[Export] public double RangedRange = 150f;
	[Export] public double OutOfRangeRange = 500f;
	[Export] public double UndetectedRange = 1000f;



    private enum EnvironmentState
	{
		GROUNDED,
		AIRBORN
	}
	private enum MoveState
	{
		IDLE,
		MOVING,
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
			//double check does this work?
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



	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;




		Velocity = velocity;
		MoveAndSlide();
	}
}
