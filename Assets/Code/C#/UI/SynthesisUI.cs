using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// �ϳ�UI������
/// </summary>
public class SynthesisUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private InventorySO inventory;

    /// <summary>
    /// ����λ�ã������������߽�
    /// </summary>
    /// <param name="pos"></param>
    /// 
    public void SetPos(Vector2 pos)
    {
        ClearSynthesis();
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


    public void SetText(string text)
    {
        infoText.text = text;
    }

    private void OnEnable()
    {
        for (int i = 0; i < container.transform.childCount; i++)
            Destroy(container.transform.GetChild(i).gameObject);
    }

    public void ClearSynthesis()
    {
        for (int i = 0; i < container.transform.childCount; i++)
            Destroy(container.transform.GetChild(i).gameObject);
    }

    public void RefreshSynthesisInput(SynthesisSO synthesis)
    {
        ClearSynthesis();

        for (int i = 0; i < synthesis.inputDic.Length; i++)
        {
            inventory.DynamicAddItem(synthesis.inputDic[i].item, false);
            GameObject item = Instantiate(baseGameObject, container.transform);
            //Debug.Log(container.name);
            item.SetActive(true);
            item.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.locked);
            item.transform.GetChild(1).GetComponent<Image>().sprite = synthesis.inputDic[i].item.itemSprite;
            item.GetComponent<ItemSingleUI>().SetCountText(synthesis.inputDic[i].count);
            item.GetComponent<ItemSingleUI>().HideCountText(false);
            item.GetComponent<ItemSingleUI>().SetIndex(i);
            item.GetComponent<ItemSingleUI>().SetInventory(inventory);

        }
    }


    public void RefreshSynthesisInput(ItemSO item)
    {
        ClearSynthesis();
        foreach (var synthesis in InventoryManager.Instance.SynthesisListSO.synthesisList)
        {
            foreach (var output in synthesis.outputDic)
            {
                if (output.item.id == item.id)
                {
                    RefreshSynthesisInput(synthesis);
                }
            }
        }

    }

    //public void DisplayerInfo(ItemSO item)
    //{
    //    switch (item.itemType)
    //    {
    //        case ItemEnum.Remote:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + "�˺�����: " + item.damageMul + "\n"
    //               + "����: " + item.fireRateTimerMax + "\n"
    //               + "���˱���: " + item.hitbackMul + "\n"
    //               + "���ٱ���: " + item.speedMul + "\n"
    //               //+ "RecoilMax: " + item.offset_X + "\n"
    //               //+ "RecoilMin: " + item.offset_Y + "\n"
    //               //+ "RecoilForce: " + item.recoilforce + "\n"
    //               //+ "LifeTimeMult: " + item.lifeTimeMul + "\n"
    //               +item.itemDescription
    //                );
    //            break;
    //        case ItemEnum.Melee:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + item.damageEnum + " " + "�˺�: " + item.damage + "\n"
    //               + "��Ӧֵ: " + item.damageCount+"\n"
    //               + "����: " + item.swingSpeed + "\n"
    //               + "��������: " + item.attackCount + "\n"
    //               + "����: " + item.hitBack+"\n"
    //               + item.itemDescription
    //                 );
    //            break;
    //        case ItemEnum.Ammo:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + item.damageEnum + " " + "Damage: " + item.damage + "\n"
    //               + "��Ӧֵ: " + item.damageCount + "\n"
    //               + "����: " + item.speed + "\n"
    //               //+ "LifeTime: " + item.lifeTimerMax + "\n"
    //               + item.itemDescription
    //               );
    //            break;
    //        case ItemEnum.Item:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + item.itemDescription
    //               );
    //            break;
    //        case ItemEnum.Accessory:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + item.itemDescription
    //               );
    //            break;

    //    }
    //}


    //   +item.damageEnum+" "+ "Damage: " + item.damage + "\n"

}
