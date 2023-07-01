using System;
using UnityEngine;


[Serializable]
public class Audio3DSettings
{
	public enum SpreadTypeEnum
	{
		Constant,
		Custom
	}

	[Range( 0f, 5f )]
	public float DopplerLevel;
	public SpreadTypeEnum SpreadType;
	[Range( 0, 360 )]
	public float SpreadConstant;
	public AnimationCurve CustomSpreadCurve;
	public float MinDistance;
	public float MaxDistance;
	public AudioRolloffMode VolumeRollOff;
	public AnimationCurve CustomVolumeCurve;

	public Audio3DSettings()
	{
		DopplerLevel = 1;
		SpreadType = SpreadTypeEnum.Custom;
		SpreadConstant = 0;
		CustomSpreadCurve = new( new Keyframe [] { new Keyframe( 0, 0.5f ), new Keyframe( 1, 0 ) } );
		MinDistance = 1;
		MaxDistance = 500;
		VolumeRollOff = AudioRolloffMode.Custom;
		CustomVolumeCurve = new( new Keyframe [] { new Keyframe( 0, 1 ), new Keyframe( 1, 0 ) } );
	}
}
