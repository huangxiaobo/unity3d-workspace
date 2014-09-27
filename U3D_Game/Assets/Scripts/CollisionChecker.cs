using UnityEngine;
using System.Collections;

public class CollisionChecker : MonoBehaviour
{

	public LayerMask layerMask; //make sure we aren't in this layer 
	public float skinWidth = 0.1f; //probably doesn't need to be changed 
	public bool antiPalyer = false;
	private float minimumExtent;
	private float partialExtent;
	private float sqrMinimumExtent;
	private Vector3 previousPosition;
	private Rigidbody myRigidbody;
	private GameObject Player;
	//initialize values 
	public void Start ()
	{ 
		
		myRigidbody = rigidbody;
		Player = GameObject.FindWithTag ("Player");
		if (Player == null) {
			Debug.Log ("CollisionChecker error");
			Application.Quit();
		}
		previousPosition = myRigidbody.position; 
		minimumExtent = Mathf.Min (Mathf.Min (collider.bounds.extents.x, collider.bounds.extents.y), collider.bounds.extents.z); 
		partialExtent = minimumExtent * (1.0f - skinWidth); 
		sqrMinimumExtent = minimumExtent * minimumExtent; 
		if (antiPalyer) {
			Physics.IgnoreCollision (Player.collider, collider);
		}
	}
	
	public void FixedUpdate ()
	{ 
		//have we moved more than our minimum extent? 
		if ((previousPosition - myRigidbody.position).sqrMagnitude > sqrMinimumExtent) { 
			Vector3 movementThisStep = myRigidbody.position - previousPosition; 
			float movementMagnitude = movementThisStep.magnitude; 
			RaycastHit hitInfo; 
			//check for obstructions we might have missed 
			if (Physics.Raycast (previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value)) 
				myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 
		} 
		previousPosition = myRigidbody.position; 
	}
	

}


