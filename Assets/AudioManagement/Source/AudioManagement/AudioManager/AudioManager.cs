using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

using Random = UnityEngine.Random;

public class AudioManagerPostProcessor : AssetPostprocessor
{
	public static event Action<string> ImportedAssetsEvent;
	public static event Action<string> DeletedAssetsEvent;

	private static void OnPostprocessAllAssets(
		string [] importedAssets,
		string [] deletedAssets,
		string [] movedAssets,
		string [] movedFromAssetPaths )
	{
		foreach ( string path in importedAssets )
		{
			if ( AssetDatabase.TryGetGUIDAndLocalFileIdentifier<AudioSet>(
				AssetDatabase.LoadAssetAtPath<AudioSet>( path ), out string guid, out long localID
			) )
			{
				ImportedAssetsEvent?.Invoke( path );
			}
		}

		foreach ( string path in deletedAssets )
		{
			DeletedAssetsEvent?.Invoke( path );
		}

		AssetDatabase.Refresh();
	}
}

[ExecuteInEditMode]
public class AudioManager : GenericSingleton<AudioManager>
{
	[Header( "Audio Settings" )]
	[SerializeField]
	private AudioMixer _AudioMixer;
	[SerializeField]
	private AudioPool _AudioPool;
	[SerializeField]
	private List<AudioSet> _AudioSets = new();

	private readonly List<AudioMixerGroup> _audioMixerGroups = new();
	private readonly List<AudioMixerGroup> _audioMixerGroupsPausable = new();
	private readonly Dictionary<GameObject, List<AudioSource>> _audioSetEntries = new();

	private readonly Dictionary<AudioSet, int> _audioSetHashCodes = new();

	private bool _isPausing = false;

	private void OnEnable()
	{
		ConfiguratePlatformSettings();
	}

	private void ConfiguratePlatformSettings()
	{
#if UNITY_WEBGL
		Application.runInBackground = true;
#endif

#if UNITY_EDITOR
		if ( !Application.isPlaying )
		{
			RegisterAudioSetEvents();

			LoadAllAudioMixerGroups();

			ClearAllAudioSets();
			LoadAllAudioSets();
		}
#endif
	}

	private void OnDisable()
	{
		DeconfiguratePlatformSettings();
	}

	private void DeconfiguratePlatformSettings()
	{
#if UNITY_EDITOR
		if ( !Application.isPlaying )
		{
			UnregisterAudioSetEvents();
		}
#endif
	}

	private void LoadAllAudioMixerGroups()
	{
		foreach ( var audioMixerGroup in _AudioMixer.FindMatchingGroups( "Master" ) )
		{
			_audioMixerGroups.Add( audioMixerGroup );
		}

		foreach ( var audioMixerGroup in _audioMixerGroups )
		{
			if ( audioMixerGroup.name.Equals( "Master" ) ) continue;
			if ( audioMixerGroup.name.Contains( "UI" ) ) continue;
			_audioMixerGroupsPausable.Add( audioMixerGroup );
		}
	}

	private void ClearAllAudioSets()
	{
		_AudioSets.Clear();
	}

	private void LoadAllAudioSets()
	{
		List<string> guids = new();
		guids.AddRange( AssetDatabase.FindAssets( "t:AudioSet" ) );
		guids.ForEach( ( guid ) =>
		{
			var path = AssetDatabase.GUIDToAssetPath( guid );
			_AudioSets.Add( AssetDatabase.LoadAssetAtPath<AudioSet>( path ) );
		} );
		LoadAudioSetEnum();
	}

	private void RegisterAudioSetEvents()
	{
		Debug.Log( "Registered!" );
		AudioManagerPostProcessor.ImportedAssetsEvent += AddAudioSet;
		AudioManagerPostProcessor.DeletedAssetsEvent += RemoveAudioSet;
	}

	private void UnregisterAudioSetEvents()
	{
		Debug.Log( "Unregistered!" );
		AudioManagerPostProcessor.ImportedAssetsEvent -= AddAudioSet;
		AudioManagerPostProcessor.DeletedAssetsEvent -= RemoveAudioSet;
	}

	private void AddAudioSet( string path )
	{
		_AudioSets.Add( AssetDatabase.LoadAssetAtPath<AudioSet>( path ) );
		UpdateAudioSetEnum();
	}

	private void RemoveAudioSet( string path )
	{
		_AudioSets = _AudioSets.Where( audioSet => audioSet != null ).ToList();
		UpdateAudioSetEnum();
	}

	private void LoadAudioSetEnum()
	{
		UpdateAudioSetEnum();
		AssetDatabase.Refresh();
	}

	private void UpdateAudioSetEnum()
	{
		_audioSetHashCodes.Clear();
		_AudioSets.ForEach( ( audioSet ) =>
		{
			_audioSetHashCodes [ audioSet ] = audioSet.name.GetHashCode();
		} );

		WriteToFile();
	}

	private void WriteToFile()
	{
		var content = string.Empty;
		foreach ( var pair in _audioSetHashCodes )
		{
			content += $"	{pair.Key.name} = {pair.Value}, {Environment.NewLine}";
		}
		content = content.Remove( content.LastIndexOf( $"," ) );

		var dataPath =
			Application.dataPath.Remove( Application.dataPath.LastIndexOf( $"Assets" ) ) +
			AssetDatabase.GUIDToAssetPath( AssetDatabase.FindAssets( "AudioSetEnum" ).GetValue( 0 ).ToString() );

		string enumImplementation =
			string.Join(
				Environment.NewLine,
				"public enum AudioSetEnum",
				"{",
				content,
				"}"
			);

		using StreamWriter writer = new( dataPath, false );
		writer.Write( enumImplementation );
		writer.Close();
	}

	// **********************************
	// *********** PUBLIC ZONE **********
	// **********************************

	public void Play( AudioSetEnum audioSetType, GameObject source = null )
	{
		AudioSet audioSet = GetAudioSetFromType( audioSetType );
		if ( !_isPausing )
		{
			PlayAudioSet( audioSet, source );
		}
		else
		{
			if ( _audioMixerGroupsPausable.Contains( audioSet.AudioMixerGroup ) ) return;
			PlayAudioSet( audioSet, source );
		}
	}

	public void Stop()
	{
		foreach ( var entry in _audioSetEntries )
		{
			_audioSetEntries [ entry.Key ].ForEach( audioSource =>
			{
				audioSource.Stop();
			} );
		}
	}

	public void Stop( AudioSetEnum audioSetType )
	{
		AudioSet audioSet = GetAudioSetFromType( audioSetType );

		foreach ( var entry in _audioSetEntries )
		{
			_audioSetEntries [ entry.Key ].ForEach( audioSource =>
			{
				audioSet.Configs.ForEach( config =>
				{
					if ( audioSource.clip.Equals( config.Audio ) )
					{
						audioSource.Stop();
					}
				} );
			} );
		}
	}

	public void Stop( GameObject source )
	{
		if ( source == null ) source = Instance.gameObject;

		foreach ( var entry in _audioSetEntries )
		{
			if ( !entry.Key.Equals( source ) ) continue;
			_audioSetEntries [ entry.Key ].ForEach( audioSource =>
			{
				audioSource.Stop();
			} );
		}
	}

	public void Stop( AudioSetEnum audioSetType, GameObject source )
	{
		if ( source == null ) source = Instance.gameObject;

		AudioSet audioSet = GetAudioSetFromType( audioSetType );

		foreach ( var entry in _audioSetEntries )
		{
			if ( !entry.Key.Equals( source ) ) continue;
			_audioSetEntries [ entry.Key ].ForEach( audioSource =>
			{
				audioSet.Configs.ForEach( config =>
				{
					if ( audioSource.clip.Equals( config.Audio ) )
					{
						audioSource.Stop();
					}
				} );
			} );
		}
	}

	public void Resume()
	{
		_isPausing = false;

		foreach ( var entry in _audioSetEntries )
		{
			foreach ( var audioSource in entry.Value )
			{
				if ( !_audioMixerGroupsPausable.Contains( audioSource.outputAudioMixerGroup ) ) continue;
				audioSource.UnPause();
			}
		}
	}

	public void Pause()
	{
		_isPausing = true;

		foreach ( var entry in _audioSetEntries )
		{
			foreach ( var audioSource in entry.Value )
			{
				if ( !_audioMixerGroupsPausable.Contains( audioSource.outputAudioMixerGroup ) ) continue;
				audioSource.Pause();
			}
		}
	}

	public void SelfDestroy()
	{
		Stop();
		Destroy( gameObject );
	}

	// **********************************
	// ********* END PUBLIC ZONE ********
	// **********************************

	private List<AudioSource> SetupAudioSet( AudioSet audioSet, GameObject source )
	{
		List<AudioSource> audioSources = new();
		foreach ( var config in audioSet.Configs )
		{
			AudioSource audioSource;

			if ( source == null )
			{
				audioSource = _AudioPool.Get( Instance.transform );
				audioSource.spatialBlend = 0;
			}
			else
			{
				audioSource = _AudioPool.Get( source.transform );
				audioSource.spatialBlend = 1;

				audioSource.dopplerLevel = audioSet.Audio3DSettings.DopplerLevel;

				if ( audioSet.Audio3DSettings.SpreadType.Equals( Audio3DSettings.SpreadTypeEnum.Constant ) )
				{
					audioSource.spread = audioSet.Audio3DSettings.SpreadConstant;
				}
				else if ( audioSet.Audio3DSettings.SpreadType.Equals( Audio3DSettings.SpreadTypeEnum.Custom ) )
				{
					audioSource.SetCustomCurve( AudioSourceCurveType.Spread, audioSet.Audio3DSettings.CustomSpreadCurve );
				}

				audioSource.minDistance = audioSet.Audio3DSettings.MinDistance;
				audioSource.maxDistance = audioSet.Audio3DSettings.MaxDistance;

				audioSource.rolloffMode = audioSet.Audio3DSettings.VolumeRollOff;

				if ( audioSet.Audio3DSettings.VolumeRollOff.Equals( AudioRolloffMode.Custom ) )
				{
					audioSource.SetCustomCurve( AudioSourceCurveType.CustomRolloff, audioSet.Audio3DSettings.CustomVolumeCurve );
				}
			}

			audioSource.clip = config.Audio;
			audioSource.loop = config.Loop;

			audioSource.outputAudioMixerGroup = audioSet.AudioMixerGroup;

			audioSource.volume = Random.Range( config.VolumeRange.x, config.VolumeRange.y );
			audioSource.pitch = Random.Range( config.PitchRange.x, config.PitchRange.y ); ;
			audioSource.panStereo = config.PanRange;

			audioSources.Add( audioSource );
		}

		return audioSources;
	}

	private AudioSet GetAudioSetFromType( AudioSetEnum audioSetType )
	{
		foreach ( var set in _AudioSets )
		{
			if ( set.AudioSetType.Equals( audioSetType ) )
			{
				return set;
			}
		}
		return null;
	}

	private void PlayAudioSet( AudioSet audioSet, GameObject source )
	{
		var audioSources = SetupAudioSet( audioSet, source );
		AddToEntries( audioSources, source );

		foreach ( var audioSource in audioSources )
		{
			audioSource.Play();
		}

		StartCoroutine( CoroutineReleaseAudios( audioSources, source ) );
	}

	private void AddToEntries( List<AudioSource> audioSources, GameObject source )
	{
		var targetObject = source != null ? source : Instance.gameObject;

		if ( !_audioSetEntries.ContainsKey( targetObject ) )
		{
			_audioSetEntries.Add( targetObject, new List<AudioSource>() );
		}

		_audioSetEntries [ targetObject ].AddRange( audioSources );
	}

	private void RemoveFromEntries( List<AudioSource> audioSources, GameObject source )
	{
		var targetObject = source != null ? source : Instance.gameObject;

		foreach ( var audioSource in audioSources )
		{
			if ( _audioSetEntries [ targetObject ].Contains( audioSource ) )
			{
				_audioSetEntries [ targetObject ].Remove( audioSource );
			}
		}
	}

	private void ReleaseAudios( List<AudioSource> audioSources )
	{
		foreach ( var audioSource in audioSources )
		{
			_AudioPool.Release( audioSource );
		}
	}

	private IEnumerator CoroutineReleaseAudios( List<AudioSource> audioSources, GameObject source )
	{
		yield return new WaitUntil( () =>
		{
			var canRelease = true;

			if ( _isPausing && _audioMixerGroupsPausable.Contains( audioSources.First().outputAudioMixerGroup ) )
			{
				canRelease = false;
			}

			return audioSources.All( audioSource => !audioSource.isPlaying ) && canRelease;
		} );
		ReleaseAudios( audioSources );
		RemoveFromEntries( audioSources, source );
	}

#pragma warning disable IDE0051
	private void LogEntries( string title = "Title" )
	{
		string output = title + $":{Environment.NewLine}";
		foreach ( var entry in _audioSetEntries )
		{
			output += $"    {entry.Key.name} : ";
			foreach ( var value in entry.Value )
			{
				if ( entry.Value.IndexOf( value ).Equals( entry.Value.Count - 1 ) )
				{
					output += $"{value.clip.name}";
				}
				else
				{
					output += $"{value.clip.name}, ";
				}
			}
			output += $"{Environment.NewLine}";
		}
		Debug.Log( $"{output}" );
	}
#pragma warning restore IDE0051
}
