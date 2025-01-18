using UnityEngine;

public static class DamageCodeGenerator
{
	public static int GenerateCode(int shape, int color, int outline)
	{
		//combine all properties into a single 3 digit code
		return shape * 100 + color * 10 + outline;
	}
}
