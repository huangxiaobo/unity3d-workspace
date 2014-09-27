using UnityEngine;
using System.Collections;

public class LaserShoot : MonoBehaviour {
	
	public float scrollSpeed = 0.5f;
	public float pulseSpeed = 1.5f;
	
	public float noiseSize = 1.0f;
	
	public float maxWidth = 0.5f;
	public float minWidth = 0.2f;
	
	public GameObject pointer = null;
	
	private LineRenderer lRenderer;
	private float aniTime = 0.0f;
	private float aniDir = 1.0f;

	
	private Vector2 materialOffset;
	private Vector2 textureScale;

	Enemy enemy;
	private Vector3 eulerAngleVector = new Vector3(90.0f,0.0f,0.0f);

	void Start () {
		lRenderer = gameObject.GetComponent<LineRenderer>() as LineRenderer;
		aniTime = 0.0f;
		ChoseNewAnimationTargetCoroutine();
		materialOffset = renderer.material.mainTextureOffset;
		textureScale = renderer.material.mainTextureScale;


	}

	void Awake() {
		//enemy = (Enemy)transform.parent.GetComponent (typeof(Enemy));
		Transform tr = transform.parent;
		if (tr) {
			enemy = (Enemy)tr.gameObject.GetComponent (typeof(Enemy));
		}
	}
	
	IEnumerator ChoseNewAnimationTargetCoroutine ()
	{
		while (true) {
			aniDir = aniDir * 0.9f + Random.Range (0.5f, 1.5f) * 0.1f;
			yield return null;
			minWidth = minWidth * 0.8f + Random.Range (0.1f, 1.0f) * 0.2f;
			yield return new WaitForSeconds (1.0f + Random.value * 2.0f - 1.0f);
		}
	}

	private bool flag = false;	//扫过一次 计算一次伤害

	void Update () {
		materialOffset.x += Time.deltaTime * aniDir * scrollSpeed;
		renderer.material.mainTextureOffset = materialOffset;
		renderer.material.SetTextureOffset ("_NoiseTex", new Vector2 (-Time.time * aniDir * scrollSpeed, 0.0f));
		
		float aniFactor  = Mathf.PingPong (Time.time * pulseSpeed, 1.0f);
		aniFactor = Mathf.Max (minWidth, aniFactor) * maxWidth;
		lRenderer.SetWidth (aniFactor, aniFactor);
		
		RaycastHit hitInfo;
		Physics.Raycast(transform.position, transform.forward, out hitInfo);
		if (hitInfo.transform) {
			lRenderer.SetPosition (1, (hitInfo.distance * Vector3.forward));
			textureScale.x = 0.1f * (hitInfo.distance);
			renderer.material.mainTextureScale = textureScale;
			renderer.material.SetTextureScale ("_NoiseTex", new Vector2 (0.1f * hitInfo.distance * noiseSize, noiseSize));		
			
			if (pointer) {
				pointer.renderer.enabled = true;
				pointer.transform.position = hitInfo.point + (transform.position - hitInfo.point) * 0.01f;
				pointer.transform.rotation = Quaternion.LookRotation (hitInfo.normal, transform.up);
				pointer.transform.eulerAngles = eulerAngleVector;
			}

			//扫过玩家
			if (hitInfo.transform.CompareTag ("Player") && flag == false && enemy != null) {
				Player player = (Player)hitInfo.collider.gameObject.GetComponent(typeof(Player));
				if (player.uid == Game._User.uid) {
					UploadPlayerDamange(player.uid, enemy.eid, 1);
				}
				//hitInfo.transform.SendMessageUpwards("ApplyDamage", 1, SendMessageOptions.DontRequireReceiver); 
				flag = true;
			} else {
				flag = false;
			}
		}
		else {
			if (pointer)pointer.renderer.enabled = false;
			float maxDist = 200.0f;
			lRenderer.SetPosition (1, (maxDist * Vector3.forward));
			textureScale.x =  0.1f * (maxDist);
			renderer.material.mainTextureScale = textureScale;
			renderer.material.SetTextureScale ("_NoiseTex", new Vector2 (0.1f * (maxDist) * noiseSize, noiseSize));
		}
		
		transform.Rotate(new Vector3(0f,1f,0f));
	}

	private void UploadPlayerDamange(int uid, int eid, int damage) {
		//Enemy en = (Enemy)hit.collider.gameObject.GetComponent(typeof(Enemy));

		msg_cs_player_damage mcpd = new msg_cs_player_damage ();
		mcpd.uid = uid;
		mcpd.eid = eid;
		mcpd.damage = damage;
				
		NetworkSocket.Instance.Send (mcpd.marshal ());
	}
}