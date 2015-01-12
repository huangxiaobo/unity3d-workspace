using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public GameObject obj;
	public Transform trans;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		trans.Rotate (Vector3.up, 10 * Time.deltaTime);
	}
}
