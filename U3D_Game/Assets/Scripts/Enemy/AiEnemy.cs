using UnityEngine;
using System.Collections;

public class AiEnemy : MonoBehaviour {
	private Rigidbody self;
	public Transform Target;
	public int Damage = 1;
	public int Range = 50;
	//Effects
	public bool Flash = true;
	private Renderer rend;
	private Color origColor;
	public Color FlashColor;
	
	//Movement & Rotation Variables
	public float Speed = 5;
	private Quaternion Rot; 
	private Vector3 NextPos;
	private bool OnMove = false;
	
	//---------------------------------------------------------------------------------//
	public void Awake (){
		// Find Self
		
		rend = gameObject.renderer;
		origColor = rend.material.color;		
		
		if(!Target){
			Target = GameObject.FindWithTag("Player").transform;
		}
	}
	//---------------------------------------------------------------------------------//
	public void Update () {
		// Make Sure We are Always Facing Proper/ And Not Flying
		
		// If we don't see the Player (Patrol/Move)-------
		if(!CanSeeTarget()){
			Vector3 v = transform.eulerAngles;
			v.x = 1;
			transform.eulerAngles = v;
			//Restore color
			if(Flash){
				rend.material.color = origColor;
			}

			if(!OnMove){
				Patrol();
			}
			
			//Rotate
			Rot = Quaternion.LookRotation(NextPos - transform.position,Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation,Rot,Time.deltaTime * 1);
			//Move
			RaycastHit hitz;
			Vector3 fwd = Vector3.forward;
			
			if(Physics.Raycast(transform.position,fwd, out hitz,20)){ 
				if(hitz.collider.gameObject.tag != "Player"){
					NextPos = - NextPos;
					//Patrol();
				}
			}
			transform.Translate(new Vector3(0,0,5) * Time.deltaTime * Speed);
		}
		
		// We Do see the Player------------------------------
		else if(CanSeeTarget()){
			//Flash
			if(Flash){
				float lerp = Mathf.PingPong (Time.time, .5f / .5f);
				rend.material.color = Color.Lerp (origColor, FlashColor, lerp);
			}
			// For Facing
			NextPos = Target.position;
			
			//RotateTowards
			Rot = Quaternion.LookRotation(Target.position - transform.position,Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, Rot,Time.deltaTime * 5);
			//Move Towards
			transform.Translate(new Vector3(0,0,5) * Time.deltaTime * Speed);
		}
	}
	//---------------------------------------------------------------------------------//
	public bool CanSeeTarget (){
		
		// If we are Too Far
		if(Vector3.Distance(Target.position, transform.position) > Range){
			return false;
		}
		// If we are close Check, Also see if there are objects
		else{
			//Cast For Objects
			RaycastHit hit;
			if(Physics.Linecast(transform.position, Target.position, out hit)){ 
				if(hit.collider.gameObject.tag != "Player"){
					Patrol();
					
					return false;
				}
				else{
					NextPos = Target.position;
					return true;
				}
			}
		}
		return false;
	}
	
	//Collision With Object------------------------------------------------------------//
	public void OnCollisionEnter (Collision colide){
		if(colide.gameObject.tag == "Player"){
			for(int i = 0; i < 5; i ++){
				Target.SendMessageUpwards("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
			}
		}
		else {
			Patrol();
			//NextPos = -NextPos;
		}
	}
	
	//Patrol-------------------------------------------------------------------------//
	public void Patrol (){
		OnMove = true;
		NextPos.x = transform.position.x + Random.Range(-5,5);
		NextPos.z = transform.position.z + Random.Range(-5,5);
		StartCoroutine(Do(Random.Range(5,5)));
		OnMove = false;
	}

	IEnumerator  Do (float time)
	{
		yield return new WaitForSeconds (time); 
	}
}