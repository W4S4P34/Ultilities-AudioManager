using System;
using UnityEngine;


[Serializable]
public class AudioConfig
{
	public AudioClip Audio;
	[MinMaxRange( 0f, 1.0f )]
	public Vector2 VolumeRange;
	[MinMaxRange( -3.0f, 3.0f )]
	public Vector2 PitchRange;
	[Range( -1.0f, 1.0f )]
	public float PanRange;
	public bool Loop;

	public AudioConfig()
	{
		Audio = null;
		VolumeRange = Vector2.one;
		PitchRange = Vector2.one;
		PanRange = 0f;
		Loop = false;
	}
}
