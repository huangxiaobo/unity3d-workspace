using UnityEngine;
using System.Collections;

public class GameObjectGlow : MonoBehaviour {
    public Shader GlowShader;
    private Material GlowMaterial;

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }
 
    private void CreateMaterial()
    {
        if (!GlowShader || GlowMaterial)
            return;
        GlowMaterial = new Material(GlowShader) {hideFlags = HideFlags.HideAndDontSave};
    }
 
    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CreateMaterial();
        Graphics.Blit(source, destination, GlowMaterial);
    }
}
