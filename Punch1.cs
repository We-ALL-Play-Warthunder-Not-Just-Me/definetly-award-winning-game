using Godot;
using System;

public partial class Punch1 : Area2D
{

    private float _knockback = 0;
    private float _damage = 0;
    private int _direction = 0;
    private bool active = false;


    public void _on_enemy_detector_body_entered(Node2D body)
    {
        GD.Print("Test");
        if (active)
        {
            GD.Print("Active");
            if (body.IsInGroup("Enemy"))
            {
                GD.Print("I HIT YOU");
                body.Call("dealDamage", _damage, _knockback);
            }
        }
        
    }

    public void _begin(int direction, float damage, float knockback)
    {
        _direction = direction;
        _knockback = knockback;
        _damage = damage;
        active = false;
    }

    public void _activate(int direction, float damage, float knockback)
    {
        _direction = direction;
        _knockback = knockback;
        _damage = damage;
        active = true;
    }

    public void _deactivate() { active = false; }



    

}
