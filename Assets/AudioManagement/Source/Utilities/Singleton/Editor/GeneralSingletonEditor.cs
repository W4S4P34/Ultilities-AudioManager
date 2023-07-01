using UnityEditor;


[CustomEditor( typeof( GenericSingleton<> ) )]
public class GenericSingletonEditor : Editor
{
	SerializedProperty _notDestroy;

	protected virtual void OnEnable()
	{
		_notDestroy = serializedObject.FindProperty( "NotDestroy" );
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField( _notDestroy );

		serializedObject.ApplyModifiedProperties();
	}
}
