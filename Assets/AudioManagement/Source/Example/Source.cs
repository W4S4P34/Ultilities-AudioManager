using UnityEngine;


public class Source : MonoBehaviour
{
	private bool _hasTriggeredOnce = false;

	private void OnTriggerEnter2D( Collider2D other )
	{
		if ( !_hasTriggeredOnce )
		{
			AudioManager.Instance.Play( AudioSetEnum.WazzupBGM, gameObject );
			_hasTriggeredOnce = true;
		}
	}
}
