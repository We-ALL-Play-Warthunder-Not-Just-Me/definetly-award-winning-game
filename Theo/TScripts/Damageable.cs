using Godot;
using System;

public interface Damageable
{
	int HP { get; }
	public bool TakeDamage(int damage, int direction, int knockback, Node2D damager);
}
