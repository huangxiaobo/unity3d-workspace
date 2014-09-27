using UnityEngine;
using System.Collections;

public class GrenadeLuancher : MonoBehaviour
{

	//手榴弹对象
	public Rigidbody explosive;
	//当前选择的榴弹的数量
	public int count = 10;
	//是否允许为空
	public bool neverEmpty = false;
	//
	public float ThrowRate = 0.5f;
	//默认速度
	public float Speed = 30 ;
	//最小速度
	public float MinSpeed = 5;
	//投影位置
	public Transform ThrowPos;		//投掷点
	//强度
	private float Power = 0;
	//是否已经投掷
	private bool isThrowing = false;

	private float throwTime = 1.0f;

	private float lastThrowTime = 0;

	// Use this for initialization
	void Start ()
	{
		
	}

	public void Awake ()
	{
		/////最小速度
		Power = MinSpeed;

	}
	
	public void Update ()
	{

		if (!isThrowing && Power < Speed)
			Power += MinSpeed * Time.deltaTime * 5;

		Game._Hud.GrenadeText = string.Format ("手雷数量: {0}", count);
	}
	
	public void Fire (float power)
	{
		Debug.Log ("GrenadeLuancher Fire--- count: " + count);
		if (count > 0 && (Time.time - lastThrowTime > throwTime)) {
			//已经投掷
			isThrowing = true;
			//投掷位置
			Vector3 pos = ThrowPos.transform.position + new Vector3 (0.0f, 0.0f, .01f);
			
			// 发射榴弹
			Rigidbody release = (Rigidbody)Instantiate (explosive, pos, ThrowPos.transform.rotation); 

			// 速度
			release.rigidbody.velocity = ThrowPos.transform.TransformDirection (new Vector3 (0, 0, power));

			StartCoroutine (WaitForSeconds (ThrowRate));
			Power = MinSpeed;
			isThrowing = false;
			
			if (!neverEmpty) {//如果允许为空，则数目减一
				count  -= 1;
			}

			lastThrowTime = Time.time;
		} else {
			isThrowing = false;
			Power = MinSpeed;	
		}
		Power = MinSpeed;
		isThrowing = false;
	}


	IEnumerator  WaitForSeconds (float time)
	{
		yield return new WaitForSeconds (time); 
	}

}
