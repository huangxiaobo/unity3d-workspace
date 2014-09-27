using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyService : MonoBehaviour {

	public GameObject[] enemyPrefabs;

	private Dictionary<int, Enemy> enemyDict = new Dictionary<int, Enemy>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void Handler(int msg_type, byte[] msg) {
		Debug.Log ("PlayerService---msg_type: " + BitConverter.GetBytes (msg_type).ToString ());
		switch (msg_type) {
		case Events.MSG_SC_ENEMY_CREATE: {
			msg_sc_enemy_create msec = new msg_sc_enemy_create();
			msec.unmarshal(msg);
			CreateEnemy(msec.eid, msec.x, msec.y, msec.z, msec.life, msec.enemy_type);
		}
			break;
		case Events.MSG_SC_ENEMY_MOVE:
		{
			
		}
			break;
		case Events.MSG_SC_ENEMY_POS:
		{
			msg_sc_enemy_pos msep = new msg_sc_enemy_pos();
			msep.unmarshal(msg);
			DownloadEnemyPos(msep.eid, msep.x, msep.y, msep.z);
		}
			break;
		case Events.MSG_SC_ENEMY_ROT:
		{
			msg_sc_enemy_rot mser = new msg_sc_enemy_rot();
			mser.unmarshal(msg);
			DownloadRot(mser.eid, mser.rotx, mser.roty, mser.rotz);
		}
			break;
		case Events.MSG_SC_ENEMY_DIE:{
			msg_sc_enemy_die msed = new msg_sc_enemy_die();
			msed.unmarshal(msg);
			DownloadDie(msed.eid);
		}
			break;
		case Events.MSG_SC_ENEMY_DAMAGE:
		{
			msg_sc_enemy_damage msed = new msg_sc_enemy_damage();
			msed.unmarshal(msg);
			DownloadDamage(msed.eid, msed.damage);
		}
			break;
		default:
			break;
		}
	}

	private void CreateEnemy(int eid, float x, float y, float z, int life, int enemy_type) {
		Debug.Log ("CreateEnemy.............. eid: " + eid);
		GameObject enemy  = (GameObject)Instantiate (enemyPrefabs[enemy_type - 1], new Vector3(x, y, z), transform.rotation);

		enemy.transform.parent = this.transform;

		Enemy en = (Enemy) enemy.GetComponent(typeof(Enemy));
		en.eid = eid;
		en.Life = life;
		en.enemyTpye = enemy_type;

		enemyDict.Add (eid, en);
	}

	public void DownloadEnemyPos(int eid, float x, float y, float z) {
		if (enemyDict.ContainsKey (eid)) {
			//
			//enemyDict[eid].transform.position = new Vector3(x, y, z);
			//enemyDict[eid].transform.rigidbody.MovePosition(new Vector3(x, y, z));
			enemyDict[eid].DownloadPos(x, y, z);
		}

	}

	public static void UploadEnemyPos() {

	}

	public void DownloadRot(int eid, float rotx, float roty, float rotz) {
		//Debug.Log ("DownloadRot....eid: " + eid + " roty: " + roty);
		//enemyDict [eid].transform.rotation = Quaternion.Euler (new Vector3(0, roty, 0));
		//enemyDict [eid].transform.Rotate(new Vector3(0, roty, 0));
		if (enemyDict.ContainsKey(eid)) {
			enemyDict[eid].DownloadRot(rotx, roty, rotz);
		}
	}

	public void DownloadDie(int eid) {
		if (enemyDict.ContainsKey (eid)) {
			enemyDict[eid].DownloadDie();
			enemyDict.Remove(eid);
		}
	}

	
	public void DownloadDamage(int eid, int damage) {
		if (enemyDict.ContainsKey (eid)) {
			enemyDict[eid].DownloadDamage(damage);
		}
	}
	


}
