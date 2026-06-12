using Godot;
using System;

public partial class SimpleShooterNPC : CharacterBody2D
{
	[Export]public float Speed = 100.0f;
    [Export]public float JumpVelocity = -400.0f;
    [Export] public float GRAVITY = 200.0f;
    [Export]public double ShootingTime = 0.5;
    [Export] public double ReloadingTime = 1.0;
    [Export] public PackedScene Bullets;
    //experimental thing that can be changed to slow down the creature while it is reloading.
    private float loadingSpeed = 1.0f;

	private enum FiringState
	{
		IDLE,
		SHOOTING,
		RELOADING
	}

	private enum AggroState
	{
		PASSIVE,
		AGGROED
	}

	private enum MovementState
	{
		IDLE,
		LEFT,
		RIGHT
	}

	private FiringState currentFiringState = FiringState.IDLE;
	private AggroState currentAggroState = AggroState.PASSIVE;
	private MovementState currentMovementState = MovementState.IDLE;
	private Area2D enemyDetector;
	private Node2D target;
	public override void _Ready()
	{
		enemyDetector = GetNode<Area2D>("EnemyDetector");
	}

	public void _on_enemy_detector_body_entered(Node2D body)
	 {
		if (body.IsInGroup("Player"))
		{
			_change_aggro_state(AggroState.AGGROED);
			target = body;
		}
	}




	
	private void _process_firing_state(double delta)
	{
		if (currentAggroState == AggroState.PASSIVE)
		{
			return;
		}
		if(currentFiringState == FiringState.IDLE)
		{
			_change_firing_state(FiringState.SHOOTING);
		}
		


	}

	private void _process_aggro_state(double delta)
	{
		// Placeholder for aggro state logic
	}

	private void _process_movement_state()
	{
		if(currentAggroState == AggroState.PASSIVE)
		{
			return;
		}
		if(currentFiringState == FiringState.SHOOTING)
		{
			_change_movement_state(MovementState.IDLE);
			return;
		}
		if (_direction_helper() < 0)
		{
			_change_movement_state(MovementState.LEFT);
		}
		else
		{
			_change_movement_state(MovementState.RIGHT);
		}
	}

    private void _enact_movement_state(ref Vector2 velocity)
    {
        switch (currentMovementState)
        {
            case MovementState.IDLE:
                velocity.X = 0;
                break;
            case MovementState.LEFT:
                velocity.X = -Speed * loadingSpeed;
                break;
            case MovementState.RIGHT:
                velocity.X = Speed * loadingSpeed;
                break;
        }
    }

	private void _enact_firing_state(double delta)
	{
		switch (currentFiringState)
		{
			case FiringState.IDLE:
				break;
			case FiringState.SHOOTING:
				_firing_timer -= delta;
				if(_firing_timer <= 0)
				{
					
					// Spawn bullet and set its direction towards the target(hopefully)
					Bullet bulletInstance = Bullets.Instantiate() as Bullet;
					bulletInstance.GlobalPosition = this.GlobalPosition; // Set the bullet's position to the NPC's position
					Vector2 directionToTarget = new Vector2(0,0);
					directionToTarget.X = _direction_helper();
					bulletInstance._begin(directionToTarget);
					GetParent().AddChild(bulletInstance); // Add the bullet to the scene tree
					_change_firing_state(FiringState.RELOADING);
				}
		
				break;
			case FiringState.RELOADING:
				_reload_timer -= delta;
				if(_reload_timer <= 0)
				{
					_change_firing_state(FiringState.SHOOTING);
				}
				break;
		}
	}

	//this will eventually be changed to be part of the environmental state process fuction
	//this will eventually be changed to be part of the environmental state process fuction
	//AND NOW I COPY IT INTO OTHER CREATURES SO IT HAS TO BE CHANGED IN ALL OF THEM
	private void _apply_gravity(double delta, ref Vector2 velocity)
	{
		if (!this.IsOnFloor())
		{
			velocity.Y += GRAVITY * (float)delta;
		}
	}

	Vector2 velocity;
	private double _firing_timer = 0.0;
	private double _reload_timer = 0.0;
	public override void _PhysicsProcess(double delta)
	{
		velocity = Velocity;
		_process_aggro_state(delta);
		_process_firing_state(delta);
		_process_movement_state();
		_apply_gravity(delta, ref velocity);

		_enact_firing_state(delta);
		_enact_movement_state(ref velocity);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void _change_firing_state(FiringState newState)
	{
		if(currentFiringState == newState)
		{
			return;
		}

        switch (newState)
        {
            case FiringState.IDLE:
                currentFiringState = FiringState.IDLE;
                break;
            case FiringState.SHOOTING:
                currentFiringState = FiringState.SHOOTING;
                _firing_timer = ShootingTime; 
                loadingSpeed = 0.0f;
                break;
            case FiringState.RELOADING:
                currentFiringState = FiringState.RELOADING;
                loadingSpeed = 0.5f;
                _reload_timer = ReloadingTime;
                break;
        }
        
    }

	private void _change_aggro_state(AggroState newState)
	{
		switch (newState)
		{
			case AggroState.AGGROED:
				currentAggroState = AggroState.AGGROED;
				break;
			case AggroState.PASSIVE:
				currentAggroState = AggroState.PASSIVE;
				break;

		}
	}

    private void _change_movement_state(MovementState newState)
    {
        if(currentAggroState == AggroState.PASSIVE)
        {
            return;
        }
        switch (newState)
        {
            case MovementState.IDLE:
                if(target != null)
                {
                    // we would want to check how far they are from the player, if the player is too far or too close we might not want to keep moving
                    return;
                }
                else
                {
                    currentMovementState = MovementState.IDLE;
                }
                break;
            case MovementState.LEFT:
                currentMovementState = MovementState.LEFT;
                break;

			case MovementState.RIGHT:
				currentMovementState = MovementState.RIGHT;
				break;
		}
	   
	}

	private int _direction_helper()
	{
		if (target == null)
		{
			return 0;
		}
		Vector2 directionToTarget = this.GlobalPosition.DirectionTo(target.GlobalPosition);
		if (directionToTarget.X < 0)
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}
}
