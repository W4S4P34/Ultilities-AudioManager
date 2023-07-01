// References: https://github.com/GucioDevs/SimpleMinMaxSlider

using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer( typeof( MinMaxRangeAttribute ) )]
public class MinMaxRangeDrawer : PropertyDrawer
{
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		var rangeAttribute = attribute as MinMaxRangeAttribute;
		var propertyType = property.propertyType;

		label.tooltip = rangeAttribute.Min.ToString( "F2" ) + " to " + rangeAttribute.Max.ToString( "F2" );

		Rect controlRect = EditorGUI.PrefixLabel( position, label );
		EditorGUI.indentLevel--;

		Rect [] splittedRects = SplitRect( controlRect, 3 );

		if ( propertyType == SerializedPropertyType.Vector2 )
		{
			EditorGUI.BeginChangeCheck();

			Vector2 vector = property.vector2Value;
			float minValue = vector.x;
			float maxValue = vector.y;

			minValue = EditorGUI.FloatField( splittedRects [ 0 ], float.Parse( minValue.ToString( "F2" ) ) );
			maxValue = EditorGUI.FloatField( splittedRects [ 2 ], float.Parse( maxValue.ToString( "F2" ) ) );

			EditorGUI.MinMaxSlider(
				splittedRects [ 1 ], ref minValue, ref maxValue,
				rangeAttribute.Min, rangeAttribute.Max
			);

			if ( minValue < rangeAttribute.Min )
			{
				minValue = rangeAttribute.Min;
			}

			if ( maxValue > rangeAttribute.Max )
			{
				maxValue = rangeAttribute.Max;
			}

			vector = new Vector2(
				minValue > maxValue ? maxValue : minValue,
				maxValue
			);

			if ( EditorGUI.EndChangeCheck() )
			{
				property.vector2Value = vector;
			}
		}
		else if ( propertyType == SerializedPropertyType.Vector2Int )
		{
			EditorGUI.BeginChangeCheck();

			Vector2Int vector = property.vector2IntValue;
			float minValue = vector.x;
			float maxValue = vector.y;

			minValue = EditorGUI.FloatField( splittedRects [ 0 ], minValue );
			maxValue = EditorGUI.FloatField( splittedRects [ 2 ], maxValue );

			EditorGUI.MinMaxSlider(
				splittedRects [ 1 ], ref minValue, ref maxValue,
				rangeAttribute.Min, rangeAttribute.Max
			);

			if ( minValue < rangeAttribute.Min )
			{
				maxValue = rangeAttribute.Min;
			}

			if ( minValue > rangeAttribute.Max )
			{
				maxValue = rangeAttribute.Max;
			}

			vector = new Vector2Int(
				Mathf.FloorToInt( minValue > maxValue ? maxValue : minValue ),
				Mathf.FloorToInt( maxValue )
			);

			if ( EditorGUI.EndChangeCheck() )
			{
				property.vector2IntValue = vector;
			}
		}
	}

	private Rect [] SplitRect( Rect targetRect, int quantity )
	{
		Rect [] rects = new Rect [ quantity ];

		for ( int index = 0 ; index < quantity ; index++ )
		{
			rects [ index ] = new Rect(
				targetRect.position.x, targetRect.position.y,
				targetRect.width, targetRect.height
			);
		}

		int elementWidth = 50;
		int elementSpace = 5;

		rects [ 0 ].x = targetRect.position.x;
		rects [ 0 ].width = elementWidth;

		rects [ 2 ].x = targetRect.position.x + targetRect.width - elementWidth;
		rects [ 2 ].width = elementWidth;

		rects [ 1 ].x = targetRect.position.x + rects [ 0 ].width + elementSpace;
		rects [ 1 ].width = rects [ 2 ].position.x - ( rects [ 0 ].x + rects [ 0 ].width ) - elementSpace * 2;

		return rects;
	}
}
