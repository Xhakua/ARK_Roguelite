using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景触发器组件
/// Player 进入触发器时加载指定场景
/// </summary>
public class SenceTrigger : MonoBehaviour
{
    [SerializeField] private GameScenesManager.SceneEnum sceneEnum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameScenesManager.LoadScene(sceneEnum);
        }
    }
}
