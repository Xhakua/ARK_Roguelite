using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 提供描述组件
/// 鼠标悬停时显示描述信息
/// </summary>
public class ProvideDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        string description = gameObject.GetComponentInChildren<IGetDescription>().GetDescription();
        UIManager.Instance.ShowDescription(description, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HideDescription();
    }
}
