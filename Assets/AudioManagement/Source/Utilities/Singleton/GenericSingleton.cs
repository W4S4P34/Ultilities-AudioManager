using UnityEngine;


public class GenericSingleton<T> : MonoBehaviour where T : Component
{
	#region Public Properties

	[Header( "Singleton" )]
	[Tooltip( "Will this game object be destroyed after loading new scene?" )]
	public bool NotDestroy = false;

	#endregion


	#region Private Static Instance

	private static T instance;

	#endregion


	#region Public Instance Getter

	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = FindObjectOfType<T>();

				if ( instance == null )
				{
					GameObject instanceGameObject = new()
					{
						name = typeof( T ).Name
					};

					instance = instanceGameObject.AddComponent<T>();
				}
			}

			return instance;
		}
	}

	#endregion


	#region Inheritable MonoBehaviour Callbacks

	public virtual void Awake()
	{
		if ( instance == null )
		{
			instance = this as T;

			if ( NotDestroy )
			{
				DontDestroyOnLoad( gameObject );
			}
		}
		else
		{
			Destroy( gameObject );
		}
	}

	#endregion
}
