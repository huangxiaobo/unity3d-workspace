using UnityEngine;
using System.Collections;

public class Utility  {
	public static float clampAngle(float a)
	{
		if (a > 180) return a - 360;
		return a;
	}
	
	
	public static Vector3 clampAngles(Vector3 a)
	{
		if (a.x > 180) a.x -= 360;
		if (a.y > 180) a.y -= 360;
		if (a.z > 180) a.z -= 360;
		
		return a;
	}

	public static void SetLayerRecursively( GameObject obj, int newLayer )

	{
		
		obj.layer = newLayer;
		
		
		
		foreach( Transform child in obj.transform )
			
		{
			
			SetLayerRecursively( child.gameObject, newLayer );
			
		}
		
	}

	public static int LayerLevel (string name)
	{
		return LayerMask.NameToLayer (name);
	}
}
