using UnityEngine;
using System.Collections;

public class SynchronizeLights : MonoBehaviour
{
	public Light light0;
	public Light light1;
	
	void LateUpdate()
	{
		if (light0)
		{
			Vector3 lightDirection = light0.transform.rotation * new Vector3(0f, 0f, -1f);
			GetComponent<Renderer>().material.SetVector("_LightDirection0", new Vector4(lightDirection.x, lightDirection.y, lightDirection.z, 0f));
			GetComponent<Renderer>().material.SetColor("_MyLightColor0", light0.color);
		}

		if (light1)
		{
			Vector3 lightDirection = light1.transform.rotation * new Vector3(0f, 0f, -1f);
			GetComponent<Renderer>().material.SetVector("_LightDirection1", new Vector4(lightDirection.x, lightDirection.y, lightDirection.z, 0f));
			GetComponent<Renderer>().material.SetColor("_MyLightColor1", light1.color);
		}
	}
}
