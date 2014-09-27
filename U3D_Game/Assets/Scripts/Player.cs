using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
	public int uid = 0;	//玩家ID
	//是否是自己
	public bool isMine = false;
	public new Camera camera;
	//武器,枪
	private Gun gun;
	//榴弹
	private GrenadeLuancher grenade;
	//角色控制器
	public CharacterController controller;

	//人物模型
	public MonoBehaviour model;
	//人物摄像机
	public MonoBehaviour Cam;
	public Transform UpperBone;
	//摄像机武器节点
	public Transform CamRnd;
	//枪挂载点
	public Transform p_gun;

	//血
	public Texture2D[] BloodSplats;
	//人物渲染器
	public List<Renderer> playerRenders = new List<Renderer> ();
	//枪渲染器
	public Renderer[] gunRenders;


	//模型动作
	private Animation an { get { return model.animation; } }
	//休息动画
	private AnimationState idle { get { return an ["idle"]; } }
	//跑动动画
	private AnimationState run { get { return an ["run"]; } }
	//跳动动画
	private AnimationState jump { get { return an ["jump"]; } }
	//自定义动画
	public Animation customAnim { get { return animation; } }
	//动画变化速率
	public AnimationCurve SpeedAdd;
	//速度
	public Vector3 vel;
	//移动速度
	Vector3 move;
	//同步
	private bool needUploadPos = false;
	private bool needUploadRot = false;
	public float syncRotx;
	public float syncRoty;
	private bool syncRotUpdated = false;
	public Vector3 syncPos;
	private bool syncPosUpdated = false;
	public Vector3 syncVel;
	private bool syncVecUpdated = false;
	public Vector3 syncMove;
	private bool syncMovUpdated = false;
	//
	public int left = -65;
	//
	public int right = 95;
	//生命值
	public int Life = 100;
	//击杀怪物数目
	public int KillCount = 0;
	//id
	internal int id = 0;//original -1 (err out of range)

	//击中时间
	public float HitTime;
	public float yvel;
	//地面
	private float grounded;
	//速度
	public float speeadd;

	//玩家渲染激活
	private bool PlayerRenderersActive = true;
	//枪渲染激活
	private bool GunRenderersActive = true;
	public bool imortality;
	
	private void InitAnimations ()
	{
		//设置动画的相关模式
		idle.speed = .5f;
		run.wrapMode = WrapMode.Loop;
		jump.wrapMode = WrapMode.Once;

		foreach (var b in blends) {
			b.AddMixingTransform (UpperBone);
			b.layer = 2;
		}
		
		blends [5].layer = 1;

		jump.layer = 1;

	}

	public void Start ()
	{
		Debug.Log ("<color=red>Debug:</color> Player::Start");
		InitAnimations ();
		controller = GetComponent<CharacterController> ();
		Debug.Log ("Player Start==" + controller);


		if (uid == Game._User.uid) {
			StartCoroutine (UploadPosition (0.1f));
		}
	}

	//todo draw calls
	public void Awake ()
	{
		Debug.Log ("Awake");
		//得到枪
		gun = GetComponentInChildren<Gun> ();
		gun.pl = this;
		//得到榴弹发射器
		grenade = GetComponentInChildren<GrenadeLuancher> ();


		if (uid == Game._User.uid) {
			StartCoroutine (UploadPosition (0.1f));
		}
	}

	public void Update ()
	{
		//Debug.Log("<color=red>Fatal Debug:</color> Player::OnUpdate");
		//Debug.Log ("Player::OnUpdate");
		//todo add bomb
		//锁定屏幕鼠标

		if (syncMovUpdated) {
			move = syncMove;
			syncMovUpdated = false;
		} else {
			move = Vector3.zero;
		}
		
		
		//Screen.lockCursor = true;
		UpdateJump ();
		UpdateMove ();
		UpdateRotation ();

		if (isMine)
			UpdateInput ();
		UpdateOther ();

	}

	public void LateUpdate ()
	{
		if (uid != Game._User.uid)
			return;
		if (dead)
			return;
	}

	public void FixedUpdate ()
	{
		if (dead)
			return;

		//如果有移动 并且在地面上
		if (move.magnitude > 0 && isGrounded) {
			//移动
			//vel += rot * move * speeadd * (Time.time - HitTime < 1 ? .5f : 1);
			vel += transform.rotation * move;
		}

		if (isGrounded)
			vel *= .83f;

		//Debug.Log("<color=blue>Player::FixedUpdate :</color> vel = " + vel);
		controller.SimpleMove (vel);

		if (syncPosUpdated) {
			controller.Move(syncPos - transform.position);
			Fade (run);
			gun.AnimateRun ();
		} else {
			//空闲动画
			Fade (idle);
			gun.AnimateIdle ();
		}

		syncPosUpdated = false;

	}
	
	private void UpdateJump ()
	{
		//如果玩家落在地上，则grouned等于当前时间的嘀嗒数
		if (controller.isGrounded)
			grounded = Time.time;
		//如果已经着地，则跳跃的权值减少
		if (isGrounded && jump.weight > 0)
			jump.weight *= .86f;
	}
	
	private void UpdateMove ()
	{
//		Debug.Log ("UpdateMove");
		//如果有移动
		if (vel.magnitude > 0.1f) {
			Quaternion lookRot = Quaternion.LookRotation (vel);
			float deltaAngle = Mathf.DeltaAngle (lookRot.eulerAngles.y, transform.eulerAngles.y);
			if (Mathf.Abs (deltaAngle) > 110) {
				model.transform.rotation = Quaternion.LookRotation (vel * -1);
				run.speed = -12f * (vel.magnitude / 5);
			} else {
				model.transform.rotation = Quaternion.LookRotation (vel);
				run.speed = 12f * (vel.magnitude / 5);
			}
			//跑动动画
			Fade (run);
			gun.AnimateRun ();
		} else {
			//空闲动画
			Fade (idle);
			gun.AnimateIdle ();
		}
	}
	
	private void UpdateRotation ()
	{
		if ((uid != Game._User.uid)) {
			CamRotX = Mathf.LerpAngle (CamRotX, syncRotx, Time.deltaTime * 10);	//上下移动
			var nwroty = Mathf.LerpAngle (ModelRotY, syncRoty, Time.deltaTime * 10);//左右移动
			var d = nwroty - ModelRotY;
			Rotate (d);
		}

		float lroty = model.transform.localRotation.eulerAngles.y;
		Vector3 CamxModely = Utility.clampAngles (new Vector3 (CamRotX, lroty, 0));
		var numpad = new Vector3 (
			(Mathf.Clamp (CamxModely.y / -left, -1, 0) + Mathf.Clamp (CamxModely.y / right, 0, 1)),
			-(Mathf.Clamp (CamxModely.x / 45, -1, 0) + Mathf.Clamp (CamxModely.x / 45, 0, 1)));
		
		foreach (var b in blends)
		{
			if (gun.shooting)
				b.speed = 1;
			else
			{
				b.speed = 0;
				b.time = 0;
			}
		}
		SetWeight(blends[5], 1);
		SetWeight(blends[6], Mathf.Max(0, 1 - Vector3.Distance(numpad, Vector3.left)));
		SetWeight(blends[4], Mathf.Max(0, 1 - Vector3.Distance(numpad, Vector3.right)));
		SetWeight(blends[2], Mathf.Max(0, 1 - Vector3.Distance(numpad, Vector3.up)));
		SetWeight(blends[8], Mathf.Max(0, 1 - Vector3.Distance(numpad, Vector3.down)));
		SetWeight(blends[1], Mathf.Max(0, 1 - Vector3.Distance(numpad, (Vector3.right + Vector3.up))));
		SetWeight(blends[7], Mathf.Max(0, 1 - Vector3.Distance(numpad, (Vector3.right + Vector3.down))));
		SetWeight(blends[3], Mathf.Max(0, 1 - Vector3.Distance(numpad, (Vector3.left + Vector3.up))));
		SetWeight(blends[9], Mathf.Max(0, 1 - Vector3.Distance(numpad, (Vector3.left + Vector3.down))));

		
	}
	
	private void UpdateInput ()
	{
		//取得移动
		move = GetMove ();
			//得到鼠标状态
		Vector3	MouseDelta = GetMouse ();

		if (move.magnitude > 0)
			needUploadPos = true;
		if (MouseDelta.magnitude > 0)
			needUploadRot = true;


		//相机移动
		CamRotX = Mathf.Clamp (Utility.clampAngle (CamRotX) + MouseDelta.x, -85, 85);
		Rotate (MouseDelta.y);
		//鼠标左键按下,开枪
		if (Input.GetMouseButton (0) && Screen.lockCursor)
			gun.MouseDown ();

		//鼠标右键按下,发射榴弹
		if (Input.GetMouseButton (1) && Screen.lockCursor) {
			Debug.Log ("MouseButton 1 Pressed.");
			grenade.Fire (0.5f);
		}


	}


	//选择模型
	private void Rotate (float d)
	{
		Vector3 e = transform.rotation.eulerAngles; 
		e.y += d; 
		transform.rotation = Quaternion.Euler (e);


		e = model.transform.localRotation.eulerAngles;
		e.y = Mathf.Clamp (Utility.clampAngle (model.transform.localRotation.eulerAngles.y - d), left, right); 
		model.transform.localRotation = Quaternion.Euler (e);
	}

	private void UpdateOther ()
	{
		if (uid == Game._User.uid) {
			//更新面板中的现实状态
			Game._Hud.UserName = String.Format ("用户名: {0}({1})", Game._User.username, Game._User.uid);
			Game._Hud.KillText = String.Format ("击杀数: {0}", KillCount);
			Game._Hud.LifeText = String.Format ("生命值: {0}", Life);
		}
	}
	
	public void Fade (AnimationState s)
	{
		//动画过度
		an.CrossFade (s.name);
	}

	public void SetWeight (AnimationState s, float f)
	{
		an.Blend (s.name, f, 0);
	}

	AnimationState[] m_blends;

	internal AnimationState[] blends {
		get {
			if (m_blends == null)
				m_blends = new[] {
					an ["ak47_shoot_blend1"],
					an ["ak47_shoot_blend1"],
					an ["ak47_shoot_blend2"],
					an ["ak47_shoot_blend3"],
					an ["ak47_shoot_blend4"],
					an ["ak47_shoot_blend5"],
					an ["ak47_shoot_blend6"],
					an ["ak47_shoot_blend7"],
					an ["ak47_shoot_blend8"],
					an ["ak47_shoot_blend9"]
				};
			return m_blends;
		}
	}

	//是否在地面
	bool isGrounded { get { return Time.time - grounded < 0.1f; } }

	//是否死亡
	public bool dead;

	//旋转摄像机
	public float CamRotX { 
		get {
			return Cam.transform.rotation.eulerAngles.x; 
		} 
		set { 
			Vector3 e = Cam.transform.rotation.eulerAngles; 
			e.x = value; 
			Cam.transform.rotation = Quaternion.Euler (e);
		} 
	}



	//设置生命值
	private void SetLife (int life)
	{
		Life = life;
	}
	//伤害
	public void ApplyDamage (int damage)
	{
		if (damage < 0)
			return;
		if (damage > Life)
			Life = 0;
		else
			Life -= damage;
	}

	public void ApplyHealth (int amount)
	{
		Life += amount;
		if (Life > 100)
			Life = 100;
	}

	public void ApplyAmmo (int amout)
	{
		if (gun) {
			gun.AddPatrons (amout);
		}
	}

	public void AddKillCount ()
	{
		KillCount++;
	}

	public static Vector3 GetMove ()
	{
		if (!Screen.lockCursor)
			return Vector3.zero;
		Vector3 v = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W))
			v += Vector3.forward;
		if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S))
			v += Vector3.back;
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A))
			v += Vector3.left;
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D))
			v += Vector3.right;
		return v.normalized;
	}

	public static Vector3 GetMouse ()
	{
		return Screen.lockCursor ? new Vector3 (-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X"), 0) : Vector3.zero;
	}

	public void OnGUI ()
	{
		Rect position = new Rect (0, 0, Screen.width, Screen.height);
		if (Life < 30) {
			GUI.DrawTexture (position, BloodSplats [0], ScaleMode.ScaleAndCrop);
		}
		if (Life < 60 && Life > 30) {
			GUI.DrawTexture (position, BloodSplats [1], ScaleMode.ScaleAndCrop);
		}
		if (Life < 80 && Life > 60) {
			GUI.DrawTexture (position, BloodSplats [2], ScaleMode.ScaleAndCrop);
		}
		if (Life < 100 && Life > 80) {
			GUI.DrawTexture (position, BloodSplats [3], ScaleMode.ScaleAndCrop);
		}
		GUI.color = Color.red;
	}

	public void SyncUpPos ()
	{
		//PlayerService.
		//需要发送数据\
		Debug.Log ("<color=red>SyncUpPos</color>");
		PlayerService.UploadPlayerPos (uid, transform.position.x, transform.position.y, transform.position.z);
	}

	public void SyncDowPos (Vector3 pos)
	{
		Debug.Log ("<color=red>SyncDowPos</color>");
		syncPos = pos;
		syncPosUpdated = true;

	}

	public void SyncUpMov ()
	{
		//PlayerService.
		//需要发送数据\
		Debug.Log ("<color=red>SyncUpMov</color>");
		PlayerService.UploadPlayerMov (uid, move.x, move.y, move.z);
	}
	
	public void SyncDowMov (Vector3 pos)
	{
		Debug.Log ("<color=red>SyncDowMov</color>");
		syncMove = move;
		syncMovUpdated = true;
		
	}

	public void SyncUpVel ()
	{
		//PlayerService.
		//需要发送数据\
		Debug.Log ("<color=red>SyncUpVel</color>");
		PlayerService.UploadPlayerVel (uid, vel.x, vel.y, vel.z);
	}
	
	public void SyncDowVel (Vector3 velicty)
	{
		Debug.Log ("<color=red>SyncDowVel</color>");
		syncVel = velicty;
		syncVecUpdated = true;
		
	}
	
	public void SyncUpRot ()
	{
		Debug.Log ("<color=red>SyncUpRot</color>");
		PlayerService.UploadPlayerRot (uid, CamRotX, ModelRotY);
	}

	public void SyncDownRot (float rotx, float roty)
	{
		Debug.Log ("<color=red>SyncDownRot</color> rotx: " + rotx + " roty:" + roty);
		syncRotx = rotx;
		syncRoty = roty;
		syncRotUpdated = true;
	}

	public void SyncDownLife(int life) {
		Life = life;
	}

	public Quaternion rot { get { return transform.rotation; } set { transform.rotation = value; } }

	public float ModelRotY {
		get { return rot.eulerAngles.y; }
		set {
			var e = rot.eulerAngles;
			e.y = value;
			rot = Quaternion.Euler (e);
		}
	}



	IEnumerator UploadPosition (float time)
	{
		while (true) {
			yield return new WaitForSeconds (time);
			if (needUploadPos) {
				SyncUpPos ();
				needUploadPos = false;
			}
			if (needUploadRot) {
				SyncUpRot ();
				needUploadRot = false;
			}
		}
	}
}

