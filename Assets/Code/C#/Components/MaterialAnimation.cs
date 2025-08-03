using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 材质动画组件
/// 用于实现了 LineRenderer 的物体材质动画
/// </summary>
public class MaterialAnimation : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Texture2D> tex;
    public int fps;
    private float time;
    private int texHash;
    private int hdrHash;
    private int index = 0;
    private void Start()
    {
        texHash = Shader.PropertyToID("_MainTex");
        hdrHash = Shader.PropertyToID("_HDRTex");
        time = 1f / fps;

    }
    private void FixedUpdate()
    {
        if (tex.Count == 0)
        {
            return;
        }
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 1f / fps;
            index++;
            if (index >= tex.Count)
            {
                index = 0;
            }
            lineRenderer.material.SetTexture(texHash, tex[index]);
            lineRenderer.material.SetTexture(hdrHash, tex[index]);
        }
    }

}
