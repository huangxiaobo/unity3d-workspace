using UnityEngine;
using System;
using System.Collections;

//类型
public enum Type {Enemy, Item};

public class DamageReciever : MonoBehaviour {

	public Type Mode;

	public int Life = 50;

	private bool Dead = false;

	public float KillTime = 0.05f;

	public GameObject HurtFX;

	public GameObject ShatterFX;

	public GameObject ExplodeFX;

	private AudioClip DAudio = null;

	public AudioClip HurtSound;


	// Use this for initialization
	void Start () {
	}
	

	public void Awake ()
	{
		//DAudio = gameObject.AddComponent(AudioSource);
	}
	
	public void Update () 
	{
		if ( Life <= 0 && Dead == false){
			//杀死一个
			if (gameObject != GameObject.FindGameObjectWithTag("Player")) {
				GameObject player = (GameObject)GameObject.FindGameObjectWithTag("Player");
				if (player == null)
					return; //throw new NullReferenceException();
				Player pl = (Player) player.GetComponent(typeof(Player));
				pl.AddKillCount();
				Debug.Log("AddKillCount----------------------");
			}
			Kill ();
		}

	}
	
	private bool Kill ()
	{
		Debug.Log ("Killllllllll");
		if (Mode == Type.Enemy) {
			KillTime = KillTime * 2;
			if(ShatterFX == null){
				GameObject Shatter = (GameObject)Instantiate(ShatterFX, transform.position, transform.rotation);
			}
			GameObject Hurt = (GameObject)Instantiate(HurtFX, transform.position, transform.rotation);
			Hurt.transform.parent = gameObject.transform;
			Destroy (gameObject, KillTime);
		}
		
		if ( Mode == Type.Item) {
			GameObject Explo = (GameObject)Instantiate(ExplodeFX, transform.position, transform.rotation);
			Destroy (gameObject, KillTime);
		}
		Dead = true;

		return true;
	}

	//攻击伤害
	public void ApplyDamage (int damage){
		Debug.Log ("DamageReciever====== damage=" + damage);
		audio.PlayOneShot (HurtSound);
		GameObject Hurt = (GameObject)Instantiate(HurtFX, transform.position, transform.rotation);
		if (damage < 0)
			return;
		Life -= damage;
		if (Life < 0)
			Life = 0;

		Debug.Log ("DamageReciever====== Life:" + Life);
	}

	//爆炸受到伤害
	public void ApplyExplosion (int damage){
		audio.PlayOneShot (HurtSound);
		if (damage < 0)
			return;
		Life -= damage; 
		if (Life < 0)
			Life = 0;
	}
}
