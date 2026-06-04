using Godot;
using System;

public partial class Bullet : Area2D
{
    [Export] private float _speed;
    [Export] private float _damage;
    private Vector2 _direction;
    [Export] private float _lifetime;


	Bullet()
	{
        
    }

    public void _on_enemy_detector_body_entered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            body.Call("dealDamage", _damage);
            QueueFree();
        }
    }

    public void _begin(Vector2 direction)
    {
        _direction = direction;
    }
    

	private Vector2 _velocity;
    public override void _PhysicsProcess(double delta)
    {
		_velocity = Position;
        _velocity.Y = 0;
        _velocity.X = _direction.X * _speed * (float)delta;
		Position += _velocity;
        _lifetime -= (float)delta;
		if (_lifetime < 0)
		{
			QueueFree();
		}
    }
	
}
