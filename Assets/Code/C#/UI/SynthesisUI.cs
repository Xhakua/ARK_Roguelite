using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 合成UI管理器
/// </summary>
public class SynthesisUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private InventorySO inventory;

    /// <summary>
    /// 设置位置，尽量不超出边界
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
    //               + "伤害倍率: " + item.damageMul + "\n"
    //               + "攻速: " + item.fireRateTimerMax + "\n"
    //               + "击退倍率: " + item.hitbackMul + "\n"
    //               + "初速倍率: " + item.speedMul + "\n"
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
    //               + item.damageEnum + " " + "伤害: " + item.damage + "\n"
    //               + "反应值: " + item.damageCount+"\n"
    //               + "攻速: " + item.swingSpeed + "\n"
    //               + "攻击次数: " + item.attackCount + "\n"
    //               + "击退: " + item.hitBack+"\n"
    //               + item.itemDescription
    //                 );
    //            break;
    //        case ItemEnum.Ammo:
    //            SetText(item.materialEnum + " " + item.complexity + "\n"
    //               + item.itemName + "\n"
    //               + item.damageEnum + " " + "Damage: " + item.damage + "\n"
    //               + "反应值: " + item.damageCount + "\n"
    //               + "初速: " + item.speed + "\n"
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
