using Godot;
using System;

public partial class HealthContainer : HBoxContainer
{
	private ProgressBar HealthBar;
	private Label HealthNum;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HealthBar = GetNode<ProgressBar>("HealthBar");
		HealthNum = GetNode<Label>("HealthNum");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_health_bar_changed()
	{
	}

	public void _on_health_bar_value_changed(float value)
	{
		HealthBar.Value = value;
		HealthNum.Text = value.ToString();
	}
}
