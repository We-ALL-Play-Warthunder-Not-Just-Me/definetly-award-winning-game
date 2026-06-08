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

	private Node2D Target;


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;




		Velocity = velocity;
		MoveAndSlide();
	}
}
