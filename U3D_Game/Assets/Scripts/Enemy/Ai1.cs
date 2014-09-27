using UnityEngine;
using System.Collections;

public class Ai1 : MonoBehaviour {
	public Transform target;
	public int moveSpeed;
	public int rotationSpeed;
	public int maxdistance;

	public GameObject Explosion;
	
	private Transform myTransform;
	
	void Awake(){
		myTransform = transform;
		SearchPlayer ();
	}
	
	
	void Start () {
		;
	}

	
	void Update () {

		if (target == null)
			return;

		Debug.DrawRay (transform.position, transform.forward);
		Debug.DrawLine(target.position, myTransform.position, Color.red); 
		
		
		myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
		
		if(Vector3.Distance(target.position, myTransform.position) > maxdistance){
			//Move towards target
			myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
			
		}
	}

	public void OnCollisionEnter (Collision colide){
		if(colide.gameObject.tag == "Player"){
	
			target.SendMessageUpwards("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);
			DamageReciever reciver = (DamageReciever)gameObject.GetComponent(typeof(DamageReciever));
			if (reciver) {
				reciver.ApplyDamage(reciver.Life);//自己的生命值清零
			}
		}
	}


	private void Kill(){
		//Destroy 
		Destroy(gameObject,.1f);
		
	}


	private void SearchPlayer() {
		GameObject go = GameObject.FindGameObjectWithTag("Player");

		if (go == null)
			return;
		
		target = go.transform;
		
		maxdistance = 2;
	}
}