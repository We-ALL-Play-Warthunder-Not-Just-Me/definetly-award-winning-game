using Godot;
using System;

public partial class TerrifyingMaw : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;


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
	private EnvironmentState ces = EnvironmentState.GROUNDED;
	private MoveState cms = MoveState.IDLE;
	private JumpState cjs = JumpState.IDLE;
	private AggessionState cas = AggessionState.PASSIVE;
	private AttackState cats = AttackState.IDLE;


	private Node2D Target;

    public void SetTarget(Node2D t)
	{
		Target = t; 
	}

	private void _process_environmental_state(double delta, ref Vector2 velocity)
	{

	}

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
