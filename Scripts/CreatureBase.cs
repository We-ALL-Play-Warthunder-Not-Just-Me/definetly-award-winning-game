using Godot;
using System;

public partial class CreatureBase : CharacterBody2D
{

    public virtual float SPEED { get; set; } = 200f;
    public virtual float DASHSPEED { get; set; } = 350f;

    public virtual float RUNSPEED { get; set; } = 250f;
    public virtual int HP { get; set; } = 100;
    public virtual float JUMPVELOCITY { get; set; }=-230f;
    public virtual float GRAVITY { get; set; } = 400f;



    public enum MoveState
    {
        IDLE,
        WALK,
        RUN,
        DASH
    }
    public enum JumpState
    {
        NONE,
        JUMPING,
        RISING,
        FALLING,
        LANDING
    }
    public enum AttackState
    {
        NONE,
        ATTACKING,
        RECOVERING,
        SHOOTING,
        MELEEING,
        COMBO1, COMBO2, COMBO3
    }

    public enum DamageState
    {
        HURT,
        RECOVING,
        FINE,
        DYING,
        RESPAWNING,
        UNDETECTED
    }

    public enum EnvironmentalState
    {
        GROUNDED,
        AIRBORN,
        LIQUID,
        WALL
    }

    public CharacterBody2D CreatureObject { get; set; }
    public virtual CollisionShape2D CollisionShape { get; set; }
    public Sprite2D Sprite { get; set; }
    public AnimationPlayer Animations { get; set; }

    public virtual MoveState ms { get; set; } = MoveState.IDLE;
    public virtual JumpState js { get; set; } = JumpState.NONE;
    public virtual AttackState ast { get; set; } = AttackState.NONE;
    public virtual DamageState ds { get; set; } = DamageState.FINE;
    public virtual EnvironmentalState es { get; set; } = EnvironmentalState.AIRBORN;





    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        CreatureObject = this;
        CollisionShape = GetNode<CollisionShape2D>("Body");
        Sprite = GetNode<Sprite2D>("Sprite2D");
        Animations = GetNode<AnimationPlayer>("AnimationPlayer");
    }




    //these store the x axis and y axis inputs for movement.
    public float xd;
    public float yd;
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        xd = 0;
        yd = 0;
        _process_move_state(delta, ref velocity);


        _enact_move_state(ref velocity);

        Velocity = velocity;
    }
    //the process states check if the input to change a state is ocurring. If it does then it calls the change state functions.
    public virtual void _process_move_state(double delta, ref Vector2 Velocity)
    {
        if (xd == 0)
        {
            _change_Move_State(MoveState.IDLE, ref Velocity);
        }
        else if (xd != 0)
        {
            _change_Move_State(MoveState.WALK, ref Velocity);
        }
    }

    public virtual void _process_jump_state(double delta)
    {
    }

    public virtual void _process_attack_state(double delta)
    {
    }

    public virtual void _process_damage_state(double delta)
    {
    }

    public virtual void _process_environmental_state(double delta)
    {
    }

    //change state functions check if anything would prevent you from changing the state. if not it changes the state and returns true.
    public virtual bool _change_Move_State(MoveState s, ref Vector2 velocity)
    {
        if (s == ms)
        {
            return false;
        }
        switch (s)
        {
            case (MoveState.IDLE):
                ms = MoveState.IDLE;
                break;
            case (MoveState.WALK):
                ms = MoveState.WALK;
                break;
            case (MoveState.RUN):
                ms = MoveState.RUN;
                break;
            case (MoveState.DASH):
                ms = MoveState.DASH;
                break;
        }

        return false;
    }

    
   

    public virtual bool _change_Jump_State(JumpState s)
    {
        if(js == s)
        {
            return false;
        }
        switch (s)
        {
            case (JumpState.NONE):
                js = JumpState.NONE;
                break;
            case (JumpState.JUMPING):
                js = JumpState.JUMPING;
                break;
            case (JumpState.RISING):
                js = JumpState.RISING;
                break;
            case (JumpState.FALLING):
                js = JumpState.FALLING;
                break;
            case (JumpState.LANDING):
                js = JumpState.LANDING;
                break;
        }
        return true;
    }

    public virtual bool _change_Attack_State(AttackState s)
    {
        if(ast == s)
        {
            return false;
        }
        switch (s)
        {
            case (AttackState.NONE):
                ast = AttackState.NONE; 
                break;
            case (AttackState.ATTACKING):
                ast = AttackState.ATTACKING;
                break;
            case (AttackState.RECOVERING):
                ast = AttackState.RECOVERING;
                break;
            case (AttackState.SHOOTING):
                ast = AttackState.SHOOTING;
                break;
            case (AttackState.MELEEING):
                ast = AttackState.MELEEING;
                break;
            case (AttackState.COMBO1):
                ast = AttackState.COMBO1;
                break;
            case (AttackState.COMBO2):
                ast = AttackState.COMBO2;
                break;
            case (AttackState.COMBO3):
                ast = AttackState.COMBO3;
                break;
        }
        return true;
    }

    public virtual bool _change_Damage_State(DamageState s)
    {
        if(ds == s)
        {
            return false;
        }
        switch (s)
        {
            case (DamageState.HURT):
                ds = DamageState.HURT;
                break;
            case (DamageState.RECOVING):
                ds = DamageState.RECOVING;
                break;
            case (DamageState.FINE):
                ds = DamageState.FINE;
                break;
            case (DamageState.DYING):
                ds = DamageState.DYING;
                break;
            case (DamageState.RESPAWNING):
                ds = DamageState.RESPAWNING;
                break;
            case (DamageState.UNDETECTED):
                ds = DamageState.UNDETECTED;
                break;
        }
        return true;
    }

    public virtual bool _change_Environmental_State(EnvironmentalState s)
    {
        switch (s)
        {
            case (EnvironmentalState.GROUNDED):

                break;
            case (EnvironmentalState.AIRBORN):
                break;
            case (EnvironmentalState.LIQUID):
                break;
            case (EnvironmentalState.WALL):
                break;
        }
        return true;
    }

    //enact state enacts the effects of the current state.
    public virtual void _enact_move_state(ref Vector2 Velocity)
    {
       
        switch (ms)
        {
            case (MoveState.IDLE):
                _state_idle(ref Velocity);
                break;
            case (MoveState.WALK):
                _state_walk(ref Velocity);
                break;
            case (MoveState.RUN):
                _state_run(ref Velocity);
                break;
            case (MoveState.DASH):
                _state_dash(ref Velocity);
                break;
        }
    }

    public virtual void _enact_jump_state(ref Vector2 Velocity)
    {
        switch (js)
        {
            case (JumpState.NONE):
                break;
            case (JumpState.JUMPING):
                break;
            case (JumpState.RISING):
                break;
            case (JumpState.FALLING):
                break;
            case (JumpState.LANDING):
                break;
        }
    }

    public virtual void _enact_attack_state(ref Vector2 Velocity)
    {
        switch (ast)
        {
            case (AttackState.NONE):
                break;
            case (AttackState.ATTACKING):
                break;
            case (AttackState.RECOVERING):
                break;
            case (AttackState.SHOOTING):
                break;
            case (AttackState.MELEEING):
                break;
            case (AttackState.COMBO1):
                break;
            case (AttackState.COMBO2):
                break;
            case (AttackState.COMBO3):
                break;
        }
    }

    public virtual void _enact_damage_state(ref Vector2 Velocity)
    {
        switch (ds)
        {
            case (DamageState.HURT):
                break;
            case (DamageState.RECOVING):
                break;
            case (DamageState.FINE):
                break;
            case (DamageState.DYING):
                break;
            case (DamageState.RESPAWNING):
                break;
            case (DamageState.UNDETECTED):
                break;
        }
    }

    public virtual void _enact_environmental_state(ref Vector2 Velocity)
    {
        switch (es)
        {
            case (EnvironmentalState.GROUNDED):
                break;
            case (EnvironmentalState.AIRBORN):
                break;
            case (EnvironmentalState.LIQUID):
                break;
            case (EnvironmentalState.WALL):
                break;
        }
    }

    //these handle the actual effect of the states
    public virtual void _state_idle(ref Vector2 velocity)
    {
        velocity.X = Mathf.MoveToward(velocity.X, 0, SPEED);
    }

    public virtual void _state_walk(ref Vector2 velocity)
    {
        velocity.X = xd * SPEED;
    }

    public virtual void _state_run(ref Vector2 velocity)
    {
        velocity.X = xd * RUNSPEED;
    }

    public virtual void _state_dash(ref Vector2 velocity)
    {
        velocity.X = xd * DASHSPEED;
    }

    public virtual void _state_jumping(ref Vector2 velocity)
    {
        
        velocity.Y = JUMPVELOCITY;
        
    }

    public virtual void _state_falling(ref Vector2 velocity)
    {
        velocity.Y = 0;
    }

    public virtual void _state_landing(ref Vector2 velocity)
    {
        velocity.Y = 0;
    }  

    public virtual void _state_attacking(ref Vector2 velocity)
    {
        
    }
    //...etc for the rest of the states. THere are a ton and not everything will need every state.
}
