using Godot;
using System;

public interface DamagableEntity
{
	public void dealDamage(float damage, int direction, Node2D damager);

}
