using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class HealthBar : ProgressBar
{
	private static double max_hp;
	private static double curr_hp;
	public ProgressBar Health_Bar;
	public ProgressBar Damage_Bar;
	public Timer timer;

	private void init_health(double h)
	{
		max_hp = h;
		curr_hp = h;
		Health_Bar.MaxValue = h;
		Health_Bar.Value = h;
		Damage_Bar.MaxValue = h;
		Damage_Bar.Value = h;
	}

	// if you're max hp changes somehow? we probably set it from a save file instead 
	public void setmaxhealth(double h)
	{
		max_hp = h;
	}

	public void setcurrenthealth(double h)
	{
		double prev_hp = curr_hp;
		curr_hp = Math.Min(Health_Bar.MaxValue, h);
		Health_Bar.Value = curr_hp;

		if (curr_hp < 0)
		{
			
		}

	}
	// Returns the current health you have after taking damage
	public double hurt(double d)
	{
		curr_hp -= d;
		return curr_hp;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health_Bar = this;
		Damage_Bar = GetNode<ProgressBar>("DamageBar");
		timer = GetNode<Timer>("Timer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
