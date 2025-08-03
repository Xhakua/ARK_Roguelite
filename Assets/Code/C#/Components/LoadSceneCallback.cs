using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���س����ص����
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
