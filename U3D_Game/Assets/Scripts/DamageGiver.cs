using UnityEngine;
using System.Collections;

public class DamageGiver : MonoBehaviour {

	public int Damage = 20;
	public int Power = 1;
	public bool Explode = true;
	public bool Burn;

	public void Update() {
		Debug.Log("<color=blue>DamageGiver Update</color> ");
	}

	public void OnCollisionEnter (Collision Colide) {
		Debug.Log("<color=blue>DamageGiver OnCollisionEnter</color> ");
		Colide.collider.SendMessageUpwards("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
		if (Explode){
			AreaDamage ();
		}
	}
	
	public void OnTriggerStay (Collider Colide) {
		if (Burn){
			if (!Colide == null)
				//yield WaitForSeconds (.05);
				Colide.collider.SendMessageUpwards("ApplyDamage" , Damage/2, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void AreaDamage (){
		Debug.Log("<color=blue>DamageGiver AreaDamage</color> ");

		Vector3 explosionPosition = transform.position; 
		Collider[] colliders  = Physics.OverlapSphere (explosionPosition, (transform.collider as SphereCollider).radius); 
		
		foreach (Collider hit in colliders) { 
			if (!hit) 
				continue; 
			if (hit.rigidbody) { 
				//对爆照物施加一个冲力
				hit.rigidbody.AddExplosionForce(Power, explosionPosition, (transform.collider as SphereCollider).radius, 1.0f); 
				
				var closestPoint = hit.rigidbody.ClosestPointOnBounds(explosionPosition); 
				var distance = Vector3.Distance(closestPoint, explosionPosition); 
				
				// The hit points we apply fall decrease with distance from the hit point 
				var hitPoints = 1.0f - Mathf.Clamp01(distance /  (transform.collider as SphereCollider).radius);
				hitPoints *= Damage; 
				
				// Tell the rigidbody or any other script attached to the hit object 
				// how much damage is to be applied! 
				hit.rigidbody.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver); 
			} 
		} 
		
	}
	


}
