using Godot;
using System;

public partial class VolumeSlider : HSlider
{

	[Export] public string busName;
	private int busIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		busIndex = AudioServer.GetBusIndex(busName);
		ValueChanged += SetVolume;

		SetValueNoSignal(AudioServer.GetBusVolumeLinear(busIndex));
	}

	private void SetVolume(double Value)
	{
		AudioServer.SetBusVolumeLinear(busIndex,(float)Value);
	}


}
