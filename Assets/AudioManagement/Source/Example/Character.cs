using UnityEngine;


public class Character : MonoBehaviour
{
	[Header( "Configs" )]
	[SerializeField]
	private int _MovementSpeed = 2;
	private Vector2 _direction;

	private void Update()
	{
		_direction = new Vector2(
			Input.GetAxis( "Horizontal" ),
			Input.GetAxis( "Vertical" )
		).normalized;
		transform.position += _MovementSpeed * Time.deltaTime * ( Vector3 ) _direction;

		if ( Input.GetKeyDown( KeyCode.Space ) )
		{
			AudioManager.Instance.Play( AudioSetEnum.DramaticSFX );
		}
	}
}
