using UnityEngine;
using System.Collections;

public class GrenadeExplosive : MonoBehaviour
{
	//是否有重力
	public bool Gravity = true;
	//冲力
	public bool Force = false;
	//速度
	public int Speed = 10;
	public float timer = 0.3f;
	public GameObject Explosion;
	public GameObject Smoke;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (WaitForSeconds1 (timer));

	}
	
	// Update is called once per frame
	void Update ()
	{
		Debug.Log ("Explode========!!!!!!!!!====");

		//Force
		if (Force) {
			rigidbody.freezeRotation = true;
			transform.Translate (Vector3.forward * Speed * Time.deltaTime);
			rigidbody.AddRelativeForce (Vector3.forward * Speed);
		}
		//Gravity
		if (!Gravity) {
			transform.rigidbody.useGravity = true;
		}
	}

	private void Explode ()
	{
		Debug.Log ("Explode================");
		//var rotation =  Quaternion.FromToRotation(Vector3.up, transform.rotation);
		//var Burn : GameObject = Instantiate ( Burn, transform.position, transform.rotation);
		GameObject explosion = (GameObject)Instantiate (Explosion, transform.position, transform.rotation); 
		Debug.Log ("Explode================ explosion" + transform.position);

		
		Kill ();
	}
	
	public void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Explosion") {
			Explode ();
		}


	}

	public void OnCollisionStay (Collision other)
	{
		return;
		if (other.gameObject.tag != "Explosion" && other.gameObject.tag != "Player") {
			if (false && other.transform.tag != "Level") {
				
				transform.position = other.transform.position;
				transform.rigidbody.velocity = Vector3.zero;
				transform.rigidbody.freezeRotation = true; 
				transform.rigidbody.useGravity = false;
				Physics.IgnoreCollision (transform.collider, other.transform.collider);
			}
		}


	}

	private void OnCollisionEnter (Collision other)
	{
		Debug.Log("碰撞------ tag: " + other.gameObject.tag);
		if (other.gameObject.tag != "Explosion" 
		    && other.gameObject.tag != "Player") {
			Debug.Log("碰撞------ tag: " + other.gameObject.transform.tag);
			//other.transform.SendMessageUpwards("ApplyDamage", 15, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	private void Kill ()
	{	
		Destroy (gameObject);
	}

	IEnumerator  WaitForSeconds (float time)
	{
		while (true) {
			yield return new WaitForSeconds (time); 
		}
	}

	IEnumerator WaitForSeconds1 (float time)
	{
		yield return new WaitForSeconds (time);
		Explode ();
		yield break;
	}
}


