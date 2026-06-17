using Godot;
using System;

public partial class BaseCreature : CharacterBody2D
{
	enum EnvironmentalState
	{
		AIRBORN,
		COYOTE,
		GROUNDED,
		WATERBORN
	}

	enum CharacterState
	{
		FINE,
		HURT,
		RECOVERING
	}

	
	public virtual void _process_environmental_state(double delta, ref Vector2 velocity)
	{

	}




}
