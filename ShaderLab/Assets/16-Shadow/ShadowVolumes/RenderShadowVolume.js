var objects : MeshFilter[];
var shadowLight : Light;
var extrusionDistance = 20.0;
var extrusionShader : Shader;
var alphaShader : Shader;

private var setAlphaMat : Material;
private var extrusionMat : Material;

function Start() {
	if (!shadowLight) {
		Debug.LogWarning ("no shadow casting light set, disabling script");
		enabled = false;
	}
	if (!extrusionShader) {
		Debug.LogWarning ("no shadow casting shader set, disabling script");
		enabled = false;
	}
	if (!GetComponent.<Camera>()) {
		Debug.LogWarning ("script must be attached to camera, disabling script");
		enabled = false;
	}
}

function OnPostRender() {
	if (!enabled)
		return;
		
    if (!setAlphaMat) {
        setAlphaMat = new Material (alphaShader);
        setAlphaMat.shader.hideFlags = HideFlags.HideAndDontSave;
        setAlphaMat.hideFlags = HideFlags.HideAndDontSave;
    }
    if (!extrusionMat) {
    	extrusionMat = new Material (extrusionShader);
    	extrusionMat.hideFlags = HideFlags.HideAndDontSave;
    }
    
	// clear screen's alpha to 1/4
    GL.PushMatrix ();
    GL.LoadOrtho ();
	setAlphaMat.SetPass (0);
	DrawQuad();
    GL.PopMatrix ();
    
    // setup extrusion shader properties
    extrusionMat.SetFloat ("_Extrusion", extrusionDistance);
    var lightPos : Vector4;
    if (shadowLight.type == LightType.Directional) {
    	var dir = -shadowLight.transform.forward;
    	dir = transform.InverseTransformDirection(dir);
    	lightPos = Vector4(dir.x,dir.y,-dir.z,0.0);
    } else {
    	var pos = shadowLight.transform.position;
    	pos = transform.InverseTransformPoint(pos);
    	lightPos = Vector4(pos.x,pos.y,-pos.z,1.0);
    }
    extrusionMat.SetVector("_LightPosition", lightPos);
    
    // render shadow volumes of all objects
    for( var filter : MeshFilter in objects ) {
    	var m : Mesh = filter.sharedMesh;
    	var tr : Transform = filter.transform;
    	extrusionMat.SetPass(0);
    	Graphics.DrawMeshNow (m, tr.localToWorldMatrix);
    	extrusionMat.SetPass(1);
    	Graphics.DrawMeshNow (m, tr.localToWorldMatrix);
    }
    
	// normalize and apply shadow mask
    GL.PushMatrix ();
    GL.LoadOrtho ();
	setAlphaMat.SetPass (1);
	DrawQuad();
	setAlphaMat.SetPass (2);
	DrawQuad();
	setAlphaMat.SetPass (3);
	DrawQuad();
	setAlphaMat.SetPass (4);
	DrawQuad();
    GL.PopMatrix ();
}

private static function DrawQuad() : void {
	GL.Begin (GL.QUADS);
	GL.Vertex3 (0, 0, 0.1);
	GL.Vertex3 (1, 0, 0.1);
	GL.Vertex3 (1, 1, 0.1);
	GL.Vertex3 (0, 1, 0.1);
	GL.End ();
}
