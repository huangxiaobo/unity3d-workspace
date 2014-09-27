using UnityEngine;
using System;
using System.Collections;

//类型
public class Enemy : MonoBehaviour
{

	public int eid;
	public int enemyTpye;
	public int Life = 50;
	private bool Dead = false;
	public float KillTime = 0.05f;
	public GameObject HurtFX;
	public GameObject ShatterFX;
	public GameObject ExplodeFX;

	private Transform target;

	
	// Use this for initialization
	void Start ()
	{

	}
	
	public void Awake ()
	{
	}
	
	public void Update ()
	{

	}

	//Collision With Object------------------------------------------------------------//
	public void OnCollisionEnter (Collision colide){
		if(enemyTpye == EnemyType.ENEMY_TYPE_AI && colide.gameObject.tag == "Player"){
			//UploadDamage(eid, 10);
		}
		else {
			//Patrol();
			//NextPos = -NextPos;
		}
	}

	
	private bool Kill ()
	{
		KillTime = KillTime * 2;
		if (ShatterFX != null) {
			GameObject Shatter = (GameObject)Instantiate (ShatterFX, transform.position, transform.rotation);
		}
		GameObject Hurt = (GameObject)Instantiate (HurtFX, transform.position, transform.rotation);
		//Hurt.transform.parent = gameObject.transform;

		//GameObject Explo = (GameObject)Instantiate(ExplodeFX, transform.position, transform.rotation);


		Destroy (gameObject, KillTime);



		Dead = true;
		
		return true;
	}
	
	//攻击伤害
	public void DownloadDamage (int damage)
	{
		Debug.Log ("DamageReciever====== damage=" + damage);
		GameObject Hurt = (GameObject)Instantiate (HurtFX, transform.position, transform.rotation);
		Life -= damage;
		if (Life < 0)
			Life = 0;
		
		Debug.Log ("DamageReciever====== Life:" + Life);
	}
	
	//爆炸受到伤害
	public void ApplyExplosion (int damage)
	{
		if (damage < 0)
			return;
		Life -= damage; 
		if (Life < 0)
			Life = 0;
	}

	public void DownloadPos(float x, float y, float z) {
		Quaternion Rot = Quaternion.LookRotation(new Vector3(x, y, z) - transform.position,Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation,Rot,Time.deltaTime * 1);

		transform.position = new Vector3 (x, y, z);
	}

	public void DownloadRot(float rotx, float roty, float rotz) {
		Quaternion rot = Quaternion.Euler (new Vector3(rotx, roty, rotz));
		transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime * 5);
		transform.Rotate(new Vector3(0f,1f,0f));
	}

	public void DownloadDie() {
		Kill ();
	}

	public void UploadDamage(int damange) {
		msg_cs_enemy_damage mced = new msg_cs_enemy_damage ();
		mced.eid = eid;
		mced.damage = damange;
		NetworkSocket.Instance.Send (mced.marshal ());
	}



}
