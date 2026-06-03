using Godot;
using System;

public partial class Playerv2 : CreatureBase
{
	


	public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;
        Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
        xd = direction.X;
        yd = direction.Y;
        _process_move_state(delta, ref velocity);


        _enact_move_state(ref velocity);

        Velocity = velocity;
        MoveAndSlide();
	}

    //MOVE STATE FUNCTIONS
    public override void _process_move_state(double delta, ref Vector2 Velocity)
    {
        //if dash button is pressed, set move state to dash


        if (xd == 0)
        {
            _change_Move_State(MoveState.IDLE, ref Velocity);
        }
        else if (xd != 0)
        {
            _change_Move_State(MoveState.WALK, ref Velocity);
        }
    }

    public override bool _change_Move_State(MoveState s, ref Vector2 velocity)
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

    public override void _enact_move_state(ref Vector2 Velocity)
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
                break;
            case (MoveState.DASH):
                break;
        }
    }

    //no need to overide _state_idle since it is perfectly fine.

    public override void _state_walk(ref Vector2 Velocity)
    {
        Velocity.X = xd * SPEED;
    }

    //JUMP STATE FUNCTIONS
    public override void _process_jump_state(double delta)
    {
        if(Input.IsActionJustPressed("Jump") && es == EnvironmentalState.GROUNDED)
        {
            _change_Jump_State(JumpState.JUMPING);
        }
        else if (es != EnvironmentalState.GROUNDED)
        {
            _change_Jump_State(JumpState.FALLING);
        }
    }

}
