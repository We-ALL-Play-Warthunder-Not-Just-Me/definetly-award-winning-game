using Godot;
using System;

public interface Attack
{
	Area2D[] damageAreas { get; }
	double duration { get; }
	public bool On_Hit(int damage, int direction, int knockback, Node2D damager);

}
