using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 文本触发器组件
/// Player 进入触发器时显示指定文本信息
/// </summary>
public class TextTrigger : MonoBehaviour
{
    [TextArea][SerializeField] private string[] message;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (string m in message)
            {
                InformationManager.Instance.AddMessage(m);
            }
            UIManager.Instance.ShowNextMessage();
            Destroy(gameObject);
        }
    }
}
