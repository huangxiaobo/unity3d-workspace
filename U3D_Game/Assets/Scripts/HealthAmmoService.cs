using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

enum HA_TYPE
{
	PACKAGE_HEALTH = 1,
	PACKAGE_AMMO = 2,
};

public class HealthAmmoService : MonoBehaviour
{

	public GameObject haPrefab;		//余肢体

	private Dictionary<int, GameObject> haDict = new Dictionary<int, GameObject> ();


	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Handler (int msg_type, byte[] msg)
	{
		Debug.Log ("HealthAmmoService---msg_type: " + BitConverter.GetBytes (msg_type).ToString ());
		switch (msg_type) {
		case Events.MSG_SC_HA_CREATE:
			{
				Debug.Log ("MSG_SC_HA_CREATE!!!!!!!!!!!!!!!!!!!!");
				msg_sc_ha_create mshc = new msg_sc_ha_create ();
				mshc.unmarshal (msg);

				GameObject go = (GameObject)Instantiate (haPrefab, new Vector3 (mshc.x, mshc.y, mshc.z), transform.rotation);

			Debug.Log("MSG_SC_HA_CREATE-----------go: " + go);

				go.transform.parent = this.transform;

				haDict.Add (mshc.hid, go);

			}
			break;
		case Events.MSG_SC_HA_PICK:
			Debug.Log ("MSG_SC_HA_PICK!!!!!!!!!!!!!!!!!!!!");
			{
				msg_sc_ha_pick mshp = new msg_sc_ha_pick ();
				mshp.unmarshal (msg);
				if (haDict.ContainsKey (mshp.hid)) {
					GameObject go = haDict [mshp.uid];
					go.transform.parent = null;
					Destroy (go);
					haDict.Remove (mshp.uid);
				}
			}
			
			break;
		case Events.MSG_SC_PLAYER_MOV:
			break;
		default:
			break;
		}
	}

}
