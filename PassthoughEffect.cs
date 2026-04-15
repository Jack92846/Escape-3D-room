using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthoughEffect : PostEffectBase
{
    [Range(0, 0.15f)]
    public float distortFactor = 0.1f;
    public Vector2 distortCenter = new Vector2(0.5f, 0.5f);
    public Texture2D NoiseTexture = null;
    [Range(0, 2.0f)]
    public float distortStrength = 1.0f;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            _Material.SetTexture("_NoiseTex", NoiseTexture);
            _Material.SetFloat("_DistortFactor", distortFactor);
            _Material.SetVector("_DistortCenter", distortCenter);
            _Material.SetFloat("_DistortStrength", distortStrength);
            Graphics.Blit(source, destination, _Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}