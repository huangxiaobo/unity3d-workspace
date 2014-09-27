using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	//待创建对象列表
	public GameObject[] Objects;
	public bool SelfSpawn = false;
	public float maxDistance = 5.0f;
	public float maxHeight = 0.0f;
	public float MaxSpawn = 50;
	public float Wait = 1.0f;
	private int currSpawn = 0;
	private bool Spawning = false;
	private int m = 0;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < 100; ++i) {
			if (currSpawn < MaxSpawn) {
				if (!Spawning) {
					Spawn ();
				}
			}
		}
	}
	
	private void Spawn ()
	{
		Spawning = true;
		//Choose Object
		m = Random.Range (0, Objects.Length);
		//Choose Position
		Vector3 Z = Vector3.zero;
		if (!SelfSpawn) {
			
			float X = Random.Range (-maxDistance, maxDistance);
			float Y = Random.Range (0, maxHeight);
			Z = new Vector3 (X, Y, X);
		} else if (SelfSpawn) {
			Z = Vector3.zero;
		}
		
		//Instantiate Objects
		GameObject go = (GameObject)Instantiate (Objects [m], transform.position + Z, transform.rotation);
		
		currSpawn++;
		StartCoroutine (Do(Wait));
		Spawning = false;
	}

	IEnumerator Do(float time) {
		print("Do now");
		yield return new WaitForSeconds(time);
		print("Do 2 seconds later");
	}
	
}
