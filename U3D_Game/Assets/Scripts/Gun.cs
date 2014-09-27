using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
	//玩家
	internal Player pl;
	//武器摄像机挂载点
	public Transform CamRnd { get { return pl.CamRnd; } }

	//时间
	Timer timer = new Timer ();
	//手
	public Transform Hands;

	//枪火显示平面
	public GameObject BulletHolePlanePrefab;
	//击中物体的小狗
	public GameObject SparkPrefab;
	//枪口的火花
	public GameObject MuzzleFlash;
	//第三视角枪火
	public GameObject MuzzleFlash2;
	//弹孔材质
	public Material[] BulletHoleMaterials;
	//枪火材质
	public Material[] MuzzleFlashMaterials;
	//绘制
	private AnimationState handsDraw { get { return handsAn ["draw"]; } }
	//跑动
	private AnimationState handsRun { get { return handsAn ["v_run"]; } }
	//空闲
	private AnimationState handsIdle { get { return handsAn ["v_idle"]; } }
	//重装
	public AnimationState handsReload { get { return handsAn ["reload"]; } }
	
	//伤害
	public int damage = 14;

	//手部动画
	internal Animation handsAn { get { return Hands.animation; } }

	AnimationState[] m_handsShoot;

	internal AnimationState[] handsShoot {
		get {
			if (m_handsShoot == null)
				m_handsShoot = new[] {
					handsAn ["shoot1"],
					handsAn ["shoot2"],
					handsAn ["shoot3"]
				};
			return m_handsShoot;
		}
	}

	public ParticleEmitter Capsules;		//第一人看到的弹壳发射效果
	public ParticleEmitter Capsules2;		//外人看到的弹壳发射效果
	public float shootBump = 1;
	public float shootCursor = 5;
	public float shootTime = .1f;
	float lastShoot;
	float cursorOffset;
	public int patrons = 30;		//子弹数量
	internal bool shooting;
	
	public void Start ()
	{
		m_handsShoot = new[] {
			handsAn ["shoot1"],
			handsAn ["shoot2"],
			handsAn ["shoot3"]
		};
		handsRun.wrapMode = WrapMode.Loop;
		handsIdle.wrapMode = WrapMode.Loop;
		handsDraw.layer = 1;
		handsReload.layer = 1;        
		handsAn.Play (handsDraw.name);
	}

	public void Update ()
	{
		Ray ray = pl.camera.ScreenPointToRay (new Vector3 (Screen.width / 2f, Screen.height / 2f, 0));
		Debug.DrawRay (ray.origin, ray.direction, Color.green, 2);	//画出线

		timer.Update ();
		//射击间隔
		shooting = Time.time - lastShoot < shootTime * 2;
		//摄像机位置
		CamRnd.localRotation = Quaternion.Slerp (CamRnd.localRotation, Quaternion.identity, Time.deltaTime * 2);
		//鼠标偏移
		cursorOffset = Mathf.MoveTowards (cursorOffset, 0, Time.deltaTime * 20);


		//更新Generade数量
		Game._Hud.PatronText = string.Format ("子弹数量: {0}", patrons);
	}
	
	public void MouseDown ()
	{
		//判断射击间隔
		if (Time.time - lastShoot > shootTime) {
			//不在重装状态
			if (!handsReload.enabled) {
				//有子弹
				if (patrons > 0)
					shoot ();//射击
				else
					Reload ();//重装
			}
		}
		if (Input.GetKeyDown (KeyCode.R) && !handsReload.enabled && patrons != 30)
			Reload ();
	}

	private void Reload ()
	{
		patrons = 30;
		handsAn.Play (handsReload.name);
	}

	public void shoot ()
	{
		//射击时间
		lastShoot = Time.time;
		//判断击中位置
		Ray ray = pl.camera.ScreenPointToRay (new Vector3 (Screen.width / 2f, Screen.height / 2f, 0));
		Debug.DrawRay (ray.origin, ray.direction);	//画出线
		CamRnd.localRotation = Quaternion.Euler (CamRnd.localRotation.eulerAngles + (Random.insideUnitSphere + Vector3.left) * shootBump);
		ray.direction +=
			(Random.insideUnitSphere * cursorOffset * 0.005f) +
			(Random.insideUnitSphere * 0.01f * pl.controller.velocity.magnitude) +
			(Time.time - pl.HitTime < 1f ? Random.insideUnitSphere * .01f : Vector3.zero);
		ProcessShoot (ray.origin, ray.direction);
		UploadShootStatus (pl.uid, ray.origin, ray.direction);

	}
	//判断击中
	private IEnumerable<RaycastHit> RaycastAll (Ray ray, int dist, int layer)
	{
		RaycastHit h;
		while (Physics.Raycast(ray, out h, dist, layer)) {
			ray.origin = h.point - h.normal * .1f;
			yield return h;
		}
	}

	//射击
	//应为摄像机在玩家的后方,所以碰撞总是碰撞到角色
	private void ProcessShoot (Vector3 rayOrg, Vector3 rayDir)
	{
		var rand = new System.Random ();
		patrons--;
		if (pl.uid != Game._User.uid) {
			//第三人视角
			MuzzleFlash2.GetComponentInChildren<Animation> ().Play ();
			Capsules2.Emit ();
		} else {
			//第一人视角
			MuzzleFlash.renderer.material = MuzzleFlashMaterials [rand.Next (MuzzleFlashMaterials.Length)];
			MuzzleFlash.animation.Play ();
			Capsules.Emit ();
		}

		//子弹射线
		Ray ray = new Ray (rayOrg, rayDir);
		//画出这条线
		Debug.DrawRay (rayOrg, rayDir, Color.white, 2);

		cursorOffset = Mathf.Min (cursorOffset + shootCursor, 15);


		foreach (RaycastHit hit in RaycastAll(ray, 1000, 1 << LayerMask.NameToLayer("Level") | 1 << LayerMask.NameToLayer("Enemy"))) {
			Debug.DrawRay (ray.origin, ray.direction, Color.red);

			if (hit.collider == null) {
				continue;
			}

			if (hit.collider.transform.gameObject.layer == Utility.LayerLevel ("Level")) {	//碰撞到地面
				Debug.Log ("<color=red>BulletHole:</color> GBulletHoleMaterials");
				((GameObject)Instantiate (SparkPrefab, hit.point, Quaternion.LookRotation (hit.normal))).transform.parent = Game.Instance.Fx;

				///弹孔
				//创建弹孔平面, 在击中点的平面上前移 hit.normal * 0.1f
				var g = (GameObject)Instantiate (BulletHolePlanePrefab, hit.point + hit.normal * .1f, Quaternion.LookRotation (hit.normal));
				//弹孔大小不缩放
				g.transform.localScale = Vector3.one * 1.0f;
				//弹孔挂载点
				g.transform.parent = Game.Instance.Fx;
				//选择一种弹孔材质
				g.renderer.material = BulletHoleMaterials [rand.Next (BulletHoleMaterials.Length)];

			} else if (hit.collider.transform.CompareTag ("Enemy")) {//碰撞到敌人
				if (hit.rigidbody != null) {
					//hit.rigidbody.AddForceAtPosition (ray.direction, hit.point);		//不要冲力
				}
				GameObject Spark  = (GameObject)Instantiate (SparkPrefab, hit.point, transform.rotation);

				//hit.collider.SendMessageUpwards ("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);
				//伤害
				if (pl.uid == Game._User.uid) {
					Enemy en = (Enemy)hit.collider.gameObject.GetComponent(typeof(Enemy));
					if (en) {
						UploadEnemyDamange(en.eid, pl.uid, 10);
					}
				}

				var g = (GameObject)Instantiate (BulletHolePlanePrefab, hit.point + hit.normal * .1f, Quaternion.LookRotation (hit.normal));
				//弹孔大小不缩放
				g.transform.localScale = Vector3.one * 1.0f;
				//弹孔挂载点
				g.transform.parent = hit.collider.transform;		//将弹孔挂在在射击对象上,不然对象消失后,弹孔还在
				//选择一种弹孔材质
				g.renderer.material = BulletHoleMaterials [rand.Next (BulletHoleMaterials.Length)];
			} else {
			}


		}
	

		// Did we hit
		/*
		RaycastHit hit;
		if (Physics.Raycast (ray.origin, ray.direction, out hit, 1000, 1 << LayerMask.NameToLayer("Level") | 1 << LayerMask.NameToLayer("Enemy")))
		{	
			Debug.DrawLine (transform.position, hit.point, Color.blue);
			//hit.collider.SendMessageUpwards("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);

			
			// Apply hit to Rigid Body
			if (hit.rigidbody ){ 
				hit.rigidbody.AddForceAtPosition(ray.direction , hit.point);
			}
			//Bullet Hole && Smoke
			if (hit.collider.transform.CompareTag ("Level")) {
				var rotation =  Quaternion.FromToRotation(Vector3.up, hit.normal);
				

			}
			// Spawn Particles: Sparks			
			if (hit.collider.transform.CompareTag ("Enemy")){
				Debug.Log("Hit Enemy.........");
				hit.collider.SendMessageUpwards("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);

			}

			Debug.Log ("<color=red>BulletHole:</color> GBulletHoleMaterials");
			((GameObject)Instantiate(pl.sparks, hit.point, Quaternion.LookRotation(hit.normal))).transform.parent = _Game.Fx;
			var g = (GameObject)Instantiate(pl.Plane, hit.point + hit.normal * .04f, Quaternion.LookRotation(hit.normal));
			g.transform.localScale = Vector3.one * 1f;
			g.transform.parent = _Game.Fx;
			g.renderer.material = pl.BulletHoleMaterials[rand.Next(pl.BulletHoleMaterials.Length)];
		}
		*/

		var a = handsShoot [rand.Next (handsShoot.Length)];
		a.time = 0;
		handsAn.Play (a.name, PlayMode.StopSameLayer);
	}

	//跑动动画
	public void AnimateRun ()
	{
		handsAn.CrossFade (handsRun.name);
	}
	//空闲动画
	public void AnimateIdle ()
	{
		handsAn.CrossFade (handsIdle.name);
	}

	public void AddPatrons (int amout)
	{
		if (amout < 0)//不允许增加负的子弹量
			return;
		if (amout > int.MaxValue - patrons)//防止溢出
			return;
		patrons += amout;
		if (patrons > 30)
			patrons = 30;
	}


	private void UploadEnemyDamange(int eid, int uid, int damage) {
		msg_cs_enemy_damage mced = new msg_cs_enemy_damage ();
		mced.eid = eid;
		mced.uid = uid;
		mced.damage = damage;

		//string hex = BitConverter.ToString(mced.marshal()).Replace("-", string.Empty);
		//Debug.Log ("rcvBuf: " + hex);
		
		NetworkSocket.Instance.Send (mced.marshal ());
	}

	private void UploadShootStatus(int eid, Vector3 ray_org, Vector3 ray_dir) {
		Debug.Log ("UploadShootStatus!!!!!!!!!!!!!!!");
		msg_cs_shoot_status mcss = new msg_cs_shoot_status ();
		mcss.uid = pl.uid;
		mcss.ray_org_x = ray_org.x;
		mcss.ray_org_y = ray_org.y;
		mcss.ray_org_z = ray_org.z;
		mcss.ray_dir_x = ray_dir.z;
		mcss.ray_dir_y = ray_dir.y;
		mcss.ray_dir_z = ray_dir.z;

		NetworkSocket.Instance.Send (mcss.marshal());
	}

	public void DownloadShootStatus( Vector3 ray_org, Vector3 ray_dir) {
		ProcessShoot (ray_org, ray_dir);
	}
}
