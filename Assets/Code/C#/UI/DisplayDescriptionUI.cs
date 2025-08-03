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
    /// 设置位置，尽量不超出边界
    /// </summary>
    /// <param name="pos"></param>
    /// 
    public void SetPos(Vector2 pos)
    {
        RectTransform rect = transform.childCount > 0 ? transform.GetChild(0).GetComponent<RectTransform>() : GetComponent<RectTransform>();

        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;

        Vector2 pivot = new Vector2();

        if (pos.x + width <= Screen.width) // 优先靠右
        {
            pivot.x = 0;
        }
        else // 左
        {
            pivot.x = 1;
        }

        if (pos.y - height >= 0) // 优先靠下
        {
            pivot.y = 1;
        }
        else // 上
        {
            pivot.y = 0;
        }

        rect.pivot = pivot;
        rect.position = pos;
    }
}
