using UnityEditor;
using UnityEngine;


[CustomEditor( typeof( AudioManager ) )]
public class AudioManagerEditor : GenericSingletonEditor
{
	private readonly float SPACE = 20f;

	private SerializedProperty _audioMixer;
	private SerializedProperty _audioPool;
	private SerializedProperty _audioSets;

	protected override void OnEnable()
	{
		base.OnEnable();

		_audioMixer = serializedObject.FindProperty( "_AudioMixer" );
		_audioPool = serializedObject.FindProperty( "_AudioPool" );
		_audioSets = serializedObject.FindProperty( "_AudioSets" );
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		GUI.enabled = false;
		EditorGUILayout.ObjectField(
			"Script", MonoScript.FromMonoBehaviour( ( AudioManager ) target ), typeof( AudioManager ), false
		);
		GUI.enabled = true;

		base.OnInspectorGUI();

		EditorGUILayout.PropertyField( _audioMixer );
		EditorGUILayout.PropertyField( _audioPool );

		EditorGUILayout.Space( SPACE );

		GUI.enabled = false;
		EditorGUILayout.PropertyField( _audioSets );
		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}
}
