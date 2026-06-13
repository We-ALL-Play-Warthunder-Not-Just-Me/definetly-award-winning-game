using Godot;
using System;

public partial class Punch1 : Area2D
{

    [Export] private float _speed;
    [Export] private float _damage;
    private Vector2 _direction;
    [Export] private float _lifetime;



    public void _on_enemy_detector_body_entered(Node2D body)
    {
        if (body.IsInGroup("Enemy"))
        {
            body.Call("dealDamage", _damage);
        }
    }

    public void _begin(Vector2 direction)
    {
        _direction = direction;
    }

    private Vector2 _velocity;
    public override void _PhysicsProcess(double delta)
    {
        _lifetime -= (float)delta;
        if (_lifetime < 0)
        {
            QueueFree();
        }
    }

}
