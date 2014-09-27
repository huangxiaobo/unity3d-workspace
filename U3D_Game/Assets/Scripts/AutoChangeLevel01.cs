using UnityEngine;
using System.Collections;

public class AutoChangeLevel01 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (UploadPosition (1.1f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	
	IEnumerator UploadPosition (float time)
	{
			yield return new WaitForSeconds (time);
		Application.LoadLevel ("Level01");
	}
}
