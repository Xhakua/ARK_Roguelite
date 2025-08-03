using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �������������
/// Player ���봥����ʱ����ָ������
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
