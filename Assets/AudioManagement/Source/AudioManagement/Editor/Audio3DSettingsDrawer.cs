using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer( typeof( Audio3DSettings ) )]
public class Audio3DSettingsDrawer : PropertyDrawer
{
	private readonly float ROW_COUNT = 8;
	private readonly float ROW_SIZE = 20;
	private readonly float ROW_SPACE = 2;

	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		property.isExpanded =
			EditorGUI.BeginFoldoutHeaderGroup( new Rect( position.x, position.y, position.width, 20 ), property.isExpanded, label );

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 1;

		if ( property.isExpanded )
		{
			EditorGUI.PropertyField(
				new Rect( position.x, position.y + 1 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
				property.FindPropertyRelative( "DopplerLevel" )
			);

			EditorGUI.PropertyField(
				new Rect( position.x, position.y + 2 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
				property.FindPropertyRelative( "SpreadType" )
			);

			if ( property.FindPropertyRelative( "SpreadType" ).enumValueIndex == ( int ) Audio3DSettings.SpreadTypeEnum.Constant )
			{
				EditorGUI.PropertyField(
					new Rect( position.x, position.y + 3 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
					property.FindPropertyRelative( "SpreadConstant" )
				);
			}
			else if ( property.FindPropertyRelative( "SpreadType" ).enumValueIndex == ( int ) Audio3DSettings.SpreadTypeEnum.Custom )
			{
				EditorGUI.PropertyField(
					new Rect( position.x, position.y + 3 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
					property.FindPropertyRelative( "CustomSpreadCurve" )
				);
			}

			EditorGUI.PropertyField(
				new Rect( position.x, position.y + 4 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
				property.FindPropertyRelative( "MinDistance" )
			);

			EditorGUI.PropertyField(
				new Rect( position.x, position.y + 5 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
				property.FindPropertyRelative( "MaxDistance" )
			);

			EditorGUI.PropertyField(
				new Rect( position.x, position.y + 6 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
				property.FindPropertyRelative( "VolumeRollOff" )
			);

			if ( property.FindPropertyRelative( "VolumeRollOff" ).enumValueIndex == ( int ) AudioRolloffMode.Custom )
			{
				EditorGUI.PropertyField(
					new Rect( position.x, position.y + 7 * ( ROW_SIZE + ROW_SPACE ), position.width, ROW_SIZE ),
					property.FindPropertyRelative( "CustomVolumeCurve" )
				);
			}
		}

		EditorGUI.indentLevel = indent;

		EditorGUI.EndFoldoutHeaderGroup();
	}

	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		if ( property.isExpanded )
		{
			if ( property.FindPropertyRelative( "VolumeRollOff" ).enumValueIndex != ( int ) AudioRolloffMode.Custom )
			{
				return ( ( base.GetPropertyHeight( property, label ) + ROW_SPACE ) * ROW_COUNT ) - 10;
			}
			return ( ( base.GetPropertyHeight( property, label ) + ROW_SPACE ) * ( ROW_COUNT + 1 ) ) - 5;
		}
		return base.GetPropertyHeight( property, label );
	}
}
