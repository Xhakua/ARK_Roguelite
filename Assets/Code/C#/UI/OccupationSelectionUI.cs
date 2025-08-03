using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ÷∞“µ—°‘ÒUI
/// </summary>

public class OccupationSelectionUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject occupationSelectionUI_ButtonPrefab;
    private void OnEnable()
    {
        for (int i = 0; i < container.transform.childCount; i++)
            Destroy(container.transform.GetChild(i).gameObject);
        for (int i = 0; i < PlayerManager.Instance.GetOccupationListCount(); i++)
        {
            OccupationSelectionUI_Button occupationSelectionUI_Button = Instantiate(occupationSelectionUI_ButtonPrefab, container.transform).GetComponent<OccupationSelectionUI_Button>();
            occupationSelectionUI_Button.gameObject.SetActive(true);
            occupationSelectionUI_Button.SetOccupation(i);
        }
    }
}
