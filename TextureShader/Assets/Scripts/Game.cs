using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	public GameObject	obj;
	public Transform	trans;
	// Use this for initialization
	void Start () {
		obj.transform.localScale = new Vector3 (1, 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		trans.Rotate(Vector3.up, Time.deltaTime * 50);
	}
}
