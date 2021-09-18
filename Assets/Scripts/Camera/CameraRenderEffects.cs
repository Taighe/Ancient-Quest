using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRenderEffects : MonoBehaviour
{
    private Material _skybox;
    private float _offsetX;
    void Awake()
    {
        //_skybox = RenderSettings.skybox;
    }

    // Update is called once per frame
    void Update()
    {
        //_offsetX += 100;
        //_skybox.SetTextureOffset("_MainTex", new Vector2(_offsetX, 0));
    }
}
