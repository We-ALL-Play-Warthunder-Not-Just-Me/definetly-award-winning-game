using Godot;
using System;

public partial class MawMawBite : Area2D
{
    [Export] private float _speed = 5;
    [Export] private float _damage = 10;
    private int _direction;
    [Export] private float _lifetime = 0.5f;

    public void _on_enemy_detector_body_entered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            GD.Print("hit?");
            body.Call("dealDamage", _damage, _direction);
            QueueFree();
        }
    }
    // direction should be an int either -1 or 1
    public void _begin(int direction, float speed, float lifetime)
    {
        _direction = direction;
        _speed = speed;
        _lifetime = lifetime;
    }


    private Vector2 _velocity;
    public override void _PhysicsProcess(double delta)
    {
        _velocity = Position;
        _velocity.Y = 0;
        _velocity.X = _speed * _direction * (float)delta;
        Position += _velocity;
        _lifetime -= (float)delta;
        if (_lifetime < 0)
        {
            QueueFree();
        }
    }

}
