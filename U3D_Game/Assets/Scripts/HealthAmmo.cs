using UnityEngine;
using System.Collections;

//类型 医药包 军火
public enum pickType {Health, Ammo}

public class HealthAmmo : MonoBehaviour {
	//类型
	public pickType Mode = pickType.Health;
	//是否能消灭
	public bool neverKill = false;
	//交替
	public bool Rotae = true;
	//速度
	public int Speed = 5;
	//回血或者增加武器数量
	public int Amount = 10;
	//击中声音
	public AudioClip hitSound;

	// Use this for initialization
	void Start () {
		if(gameObject.GetComponent("AudioSource") == null){
			gameObject.AddComponent("AudioSource");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Rotae){
			transform.Rotate(Vector3.up * (Time.deltaTime * Speed * 10));
		}
	}


	public void OnTriggerEnter (Collider Colide){
		//医药包
		if(Mode == pickType.Health){
			//如果与玩家碰撞 表示得到医药包
			if(Colide.collider.CompareTag("Player")){
				//Play Sound + add Health
				audio.PlayOneShot(hitSound);
				Colide.collider.SendMessageUpwards("ApplyHealth", Amount, SendMessageOptions.DontRequireReceiver);
				//医药包消失
				if(!neverKill){
					Kill();
				}
			}
		}
		//Ammo
		if(Mode == pickType.Ammo){
			if(Colide.collider.CompareTag("Player")){
				//Play Sound + add ammo
				audio.PlayOneShot(hitSound);
				Colide.collider.SendMessageUpwards("ApplyAmmo", Amount, SendMessageOptions.DontRequireReceiver);
				//子弹库消失
				if(!neverKill){
					Kill();
				}
			}
		}
		
		
	}
	
	
	private void Kill(){
		//Destroy 
		audio.PlayOneShot(hitSound);
		Destroy(gameObject,.1f);
		
	}
}



