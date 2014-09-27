using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
	public float Timer = 1.0f;
	// Use this for initialization
	void Start () {
		Destroy(gameObject,Timer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}



