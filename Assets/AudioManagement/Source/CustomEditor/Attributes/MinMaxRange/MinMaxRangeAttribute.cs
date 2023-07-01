using UnityEngine;


public class MinMaxRangeAttribute : PropertyAttribute
{
	public float Min;
	public float Max;

	public MinMaxRangeAttribute( float minValue, float maxValue )
	{
		Min = minValue;
		Max = maxValue;
	}
}
