using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DisplayDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }


    /// <summary>
    /// ����λ�ã������������߽�
    /// </summary>
    /// <param name="pos"></param>
    /// 
    public void SetPos(Vector2 pos)
    {
        RectTransform rect = transform.childCount > 0 ? transform.GetChild(0).GetComponent<RectTransform>() : GetComponent<RectTransform>();

        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;

        Vector2 pivot = new Vector2();

        if (pos.x + width <= Screen.width) // ���ȿ���
        {
            pivot.x = 0;
        }
        else // ��
        {
            pivot.x = 1;
        }

        if (pos.y - height >= 0) // ���ȿ���
        {
            pivot.y = 1;
        }
        else // ��
        {
            pivot.y = 0;
        }

        rect.pivot = pivot;
        rect.position = pos;
    }
}
