using System;
using UnityEngine;
using UnityEngine.Pool;


public class AudioPool : MonoBehaviour
{
	[SerializeField]
	[Range( 32, 64 )]
	private int _MaxSize = 32;
	private readonly bool _collectionChecks = true;

	private IObjectPool<AudioSource> _audioPool;

	private void Awake()
	{
		Initialize();
	}

	public AudioSource Get( Transform parentTransform = null )
	{
		var audioSource = _audioPool.Get();
		if ( parentTransform != null )
		{
			audioSource.transform.SetParent( parentTransform );
			audioSource.transform.localPosition = Vector3.zero;
		}

		return audioSource;
	}

	public void Release( AudioSource audioSource )
	{
		try
		{
			_audioPool.Release( audioSource );
		}
		catch ( Exception )
		{
			Debug.LogWarning( $"Object is already in the object pool." );
		}
	}

	private void Initialize() => _audioPool = new LinkedPool<AudioSource>(
		OnCreateAudioSource, OnGetAudioSource, OnReleaseAudioSource, OnDestroyAudioSource,
		_collectionChecks, _MaxSize
	);

	private AudioSource OnCreateAudioSource()
	{
		var audioGameObject = new GameObject( "AudioSource" );
		var audioSource = audioGameObject.AddComponent<AudioSource>();

		ResetAudioSource( audioSource );
		audioSource.gameObject.transform.SetParent( transform );
		audioSource.gameObject.transform.localPosition = Vector3.zero;
		audioSource.gameObject.SetActive( false );

		return audioSource;
	}

	private void OnGetAudioSource( AudioSource audioSource )
	{
		audioSource.gameObject.SetActive( true );
	}

	private void OnReleaseAudioSource( AudioSource audioSource )
	{
		ResetAudioSource( audioSource );
		audioSource.gameObject.transform.SetParent( transform );
		audioSource.gameObject.transform.localPosition = Vector3.zero;
		audioSource.gameObject.SetActive( false );
	}

	private void OnDestroyAudioSource( AudioSource audioSource )
	{
		Destroy( audioSource.gameObject );
	}

	private void ResetAudioSource( AudioSource audioSource )
	{
		audioSource.clip = null;
		audioSource.outputAudioMixerGroup = null;

		audioSource.bypassEffects = false;
		audioSource.bypassListenerEffects = false;
		audioSource.bypassReverbZones = false;

		audioSource.playOnAwake = false;
		audioSource.loop = false;

		audioSource.priority = 128;
		audioSource.volume = 1;
		audioSource.pitch = 1;
		audioSource.panStereo = 0;
		audioSource.spatialBlend = 0;
		audioSource.reverbZoneMix = 1;

		audioSource.dopplerLevel = 1;
		audioSource.spread = 0;

		audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
		audioSource.minDistance = 1;
		audioSource.maxDistance = 500;
	}
}
