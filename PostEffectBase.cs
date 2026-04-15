using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectBase : MonoBehaviour
{
    protected Material _Material;

    public Shader shader = null;

    void Start()
    {
        if (shader != null && shader.isSupported)
        {
            _Material = new Material(shader);
        }
    }

    protected bool CheckResources()
    {
        if (_Material == null && shader != null)
        {
            _Material = new Material(shader);
        }
        return _Material != null;
    }
}
