using UnityEngine;
using System.Collections;

public class GameObjectRotate : MonoBehaviour {
	public float speed = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.right * Time.deltaTime * speed);
		transform.Rotate(Vector3.up * Time.deltaTime * speed, Space.World);
	}
}
