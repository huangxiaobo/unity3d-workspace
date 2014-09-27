using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 

public class EnemyFlash : MonoBehaviour {
	//受到攻击就开始闪烁

	//Effects
	public bool Flash = false;
	private Renderer render;
	private Color origColor;
	public Color FlashColor;

	public float Duration = 3000;	//闪烁持续时间
	public float lastAttackedTime = 0;

	// Use this for initialization
	void Start () {
	
	}

	public void Awake (){
		// Find Self
		
		render = gameObject.renderer;
		origColor = render.material.color;		

	}
	
	// Update is called once per frame
	void Update () {
	
		if(Flash || (DateTime.Now.Ticks - lastAttackedTime < Duration)){
			float lerp = Mathf.PingPong (Time.time, .5f / .5f);
			render.material.color = Color.Lerp (origColor, FlashColor, lerp);
		}
	}

	public void StartFlash() {
		lastAttackedTime =  DateTime.Now.Ticks;
	}
}
