using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public partial class HealthBar : ProgressBar
{
	private static double max_hp;
	private static double curr_hp;
	public ProgressBar Health_Bar;
	public ProgressBar Damage_Bar;
	public Timer timer;

	private void _init_health(double h)
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
		Health_Bar.MaxValue = h;
		Damage_Bar.MaxValue = h;
	}

	public void setcurrenthealth(double h)
	{
		double prev_hp = curr_hp;
		curr_hp = Math.Min(Health_Bar.MaxValue, h);
		Health_Bar.Value = curr_hp;

		// when die
		if (curr_hp <= 0)
		{
			//I dunno health bar gone
			//QueueFree();
			//maybe we handle this particular part in the player script.
		}
		if (curr_hp < prev_hp)
		{
			timer.Start();
		}
		else
		{
			Damage_Bar.Value = curr_hp;
		}

	}

	private void _on_timer_timeout()
	{
		Damage_Bar.Value = curr_hp;
	}


	// Returns and sets the current health you have after taking damage
	public double hurt(double d)
	{
		double new_hp = curr_hp - d;
		setcurrenthealth(new_hp);
		return new_hp;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health_Bar = this;
		Damage_Bar = GetNode<ProgressBar>("DamageBar");
		timer = GetNode<Timer>("Timer");
		//WAH
		_init_health(100);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if (Input.IsActionJustPressed("Jump"))
		//{
			//hurt(10);
		//}
	}
}
