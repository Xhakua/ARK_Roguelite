using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 加载场景回调组件
/// </summary>
public class LoadSceneCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;
    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Invoke("Load", 0.5f);
        }
    }

    private void Load()
    {
        GameScenesManager.LoadSceneCallback();
    }
}
