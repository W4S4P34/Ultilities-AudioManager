using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu( fileName = "Sound Configuration", menuName = "Scriptable Objects/Sound Configuration", order = 10 )]
public class AudioSet : ScriptableObject
{
	[SerializeField]
	private AudioSetEnum _AudioSetType;

	public AudioSetEnum AudioSetType
	{
		get => _AudioSetType;
		private set => _AudioSetType = value;
	}

	[SerializeField]
	private AudioMixerGroup _AudioMixerGroup;

	public AudioMixerGroup AudioMixerGroup
	{
		get => _AudioMixerGroup;
		private set => _AudioMixerGroup = value;
	}

	[SerializeField]
	private Audio3DSettings _Audio3DSettings;

	public Audio3DSettings Audio3DSettings
	{
		get => _Audio3DSettings;
		private set => _Audio3DSettings = value;
	}

	[SerializeField]
	private List<AudioConfig> _Configs = new();

	public List<AudioConfig> Configs
	{
		get => _Configs;
		private set => _Configs = value;
	}
}
