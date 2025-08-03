using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ı����������
/// Player ���봥����ʱ��ʾָ���ı���Ϣ
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
