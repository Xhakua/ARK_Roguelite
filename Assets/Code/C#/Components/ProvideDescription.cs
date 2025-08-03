using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �ṩ�������
/// �����ͣʱ��ʾ������Ϣ
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
