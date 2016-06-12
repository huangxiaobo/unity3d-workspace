using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TISSAO : MonoBehaviour 
{
    public Shader TISSAOShader;
    private Material SSAOMaterial;
 
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }
 
    private void CreateMaterial()
    {
        if (!TISSAOShader || SSAOMaterial)
            return;
        SSAOMaterial = new Material(TISSAOShader) {hideFlags = HideFlags.HideAndDontSave};
    }
 
    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CreateMaterial();
        Graphics.Blit(source, destination, SSAOMaterial);
    }
}