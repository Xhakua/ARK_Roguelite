using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 库存里的物品格
/// </summary>
public class ItemSingleUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,IGetDescription
{
    //在Inventory中的位置
    private int indexNum;
    // 用于拖拽的临时物品
    private GameObject tempGameObject;
    //原始的GameObject，用于拖拽结束时恢复
    private GameObject oriGameObject;
    //自己所属的InventorySO
    private InventorySO inventory;
    private bool canDrag = false;
    //格子的状态
    [SerializeField] private InventoryManager.BagGridStateEnum bagGridState;
    public GameObject bagGrid;
    [SerializeField] private TextMeshProUGUI countText;

    public InventoryManager.BagGridStateEnum GetBagGridState()
    {
        return this.bagGridState;
    }
    public void SetBagGridState(InventoryManager.BagGridStateEnum bagGridState)
    {
        this.bagGridState = bagGridState;
    }
    public void SetInventory(InventorySO inventory)
    {
        this.inventory = inventory;
    }
    public void SetCountText(int count)
    {
        countText.text = count.ToString();
    }
    public void HideCountText(bool hide)
    {
        countText.gameObject.SetActive(!hide);
    }

    public InventorySO GetInventory()
    {
        return inventory;
    }
    public void SetIndex(int index)
    {
        indexNum = index;
    }
    public int GetIndex()
    {
        return indexNum;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (inventory.GetGameObject(indexNum) && (this.bagGridState == InventoryManager.BagGridStateEnum.normal || this.bagGridState == InventoryManager.BagGridStateEnum.toBeUnlocked))
        {
            tempGameObject = InventoryManager.Instance.tempGameObject.gameObject;
            tempGameObject.GetComponent<ItemSingleUI>().bagGrid.GetComponent<Image>().sprite = this.bagGrid.GetComponent<Image>().sprite;
            oriGameObject = eventData.pointerCurrentRaycast.gameObject;
            tempGameObject.SetActive(true);
            canDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            tempGameObject.transform.position = eventData.position + new Vector2(-20, 20);

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canDrag && eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject targetGameObject = eventData.pointerCurrentRaycast.gameObject;
            targetGameObject.TryGetComponent<ItemSingleUI>(out ItemSingleUI dirItemSingleUI);
            if (dirItemSingleUI == null)
            {
                tempGameObject.SetActive(false);
                return;
            }
            InventoryManager.BagGridStateEnum dirBagGridState = dirItemSingleUI.GetBagGridState();
            if (this.bagGridState == InventoryManager.BagGridStateEnum.normal && (dirBagGridState == InventoryManager.BagGridStateEnum.normal || dirBagGridState == InventoryManager.BagGridStateEnum.empty))
            {
                tempGameObject.SetActive(false);
                InventorySO dirinventory = dirItemSingleUI.GetInventory();
                int dirIndex = dirItemSingleUI.GetIndex();
                if (!dirinventory.IsNull(dirIndex) && inventory.GetID(indexNum) == dirinventory.GetID(dirIndex))
                {
                    dirinventory.AddItem(inventory.GetItem(indexNum));
                    InventoryManager.Instance.ItemChanged();
                    return;
                }
                else
                {
                    InventorySO.SwithItems(inventory, indexNum, dirinventory, dirIndex);
                }

                InventoryManager.Instance.ItemChanged();
            }
            if (this.bagGridState == InventoryManager.BagGridStateEnum.toBeUnlocked)
            {
                tempGameObject.SetActive(false);
                InventoryManager.Instance.SynthesisItem(inventory.GetItem(indexNum));
            }
            else
            {
                if (tempGameObject)
                    tempGameObject.SetActive(false);
            }
            canDrag = false;
        }


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.name == "ItemContainer")
                InventoryManager.Instance.ItemPressed(indexNum);
        }
        if (eventData.button == PointerEventData.InputButton.Middle
            && eventData.pointerCurrentRaycast.gameObject.transform.parent.name == "ItemContainer"
            )
        {
            InventoryManager.Instance.SearchSynthesisItem(indexNum);
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject dirGO = eventData.pointerCurrentRaycast.gameObject;
            if (dirGO.TryGetComponent<ItemSingleUI>(out ItemSingleUI itemSingleUI))
            {

                InventoryManager.Instance.Infobox.transform.position = dirGO.transform.position;
                InventoryManager.Instance.Infobox.GetComponent<SynthesisUI>().SetPos(dirGO.transform.position);
                //if (itemSingleUI.GetBagGridState() == InventoryManager.BagGridStateEnum.normal)
                //{
                //    InventoryManager.Instance.Infobox.GetComponent<SynthesisUI>().DisplayerInfo(inventory.GetItem(indexNum));
                //    return;
                //}
                if (itemSingleUI.GetBagGridState() == InventoryManager.BagGridStateEnum.locked || itemSingleUI.GetBagGridState() == InventoryManager.BagGridStateEnum.toBeUnlocked)
                {
                    InventoryManager.Instance.Infobox.SetActive(true);
                    InventoryManager.Instance.Infobox.GetComponent<SynthesisUI>().RefreshSynthesisInput(inventory.GetItem(indexNum));

                }

            }
        }
    }

    /// <summary>
    /// 设置是否可以合成
    /// </summary>
    /// <param name="can"></param>
    public void SetCanSynthesis(bool can)
    {
        if (can)
        {
            transform.GetChild(1).GetComponent<Image>().color = Color.white;
        }
        else
        {
            transform.GetChild(1).GetComponent<Image>().color = Color.black;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bagGridState == InventoryManager.BagGridStateEnum.normal)
        {

        }

        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<ItemSingleUI>(out ItemSingleUI itemSingleUI)
            && itemSingleUI.GetBagGridState() == InventoryManager.BagGridStateEnum.normal)
        {
            InventoryManager.Instance.Infobox.SetActive(false);
        }
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if (bagGridState == InventoryManager.BagGridStateEnum.normal)
        {

        }
    }

    public string GetDescription()
    {
        string description = inventory.GetItem(indexNum).itemDescription;
        return description;
    }


}



















//public void OnEndDrag(PointerEventData eventData)
//{

//    if (canDrag && eventData.pointerCurrentRaycast.gameObject != null)
//    {
//        GameObject targetGameObject = eventData.pointerCurrentRaycast.gameObject;
//        if (targetGameObject.transform.parent.name == "ItemContainer" || targetGameObject.transform.parent.name == "bagInteraction_GridContainer")
//        {
//            tempGameObject.SetActive(false);
//            InventorySO dirinventory = targetGameObject.GetComponent<ItemSingleUI>().GetInventory();
//            int dirIndex = targetGameObject.GetComponent<ItemSingleUI>().GetIndex();

//            if (!dirinventory.IsNull(dirIndex) && inventory.GetID(indexNum) == dirinventory.GetID(dirIndex))
//            {
//                dirinventory.items[indexNum].damageCount += Mathf.Max(1, inventory.items[dirIndex].damageCount);
//                inventory.RemoveItem(indexNum);
//                InventoryManager.Instance.RefreshItems();
//                //oriGameObject.GetComponent<Image>().sprite = bagGrid;
//                if (targetGameObject.transform.parent.name == "ItemContainer")
//                    InventoryManager.Instance.ItemChanged();
//                return;
//            }
//            //拖到屏幕外丢弃,加油

//            //oriGameObject.GetComponent<Image>().sprite = targetGameObject.GetComponent<Image>().sprite;
//            InventorySO.SwithItems(inventory, indexNum, dirinventory, dirIndex);
//            //targetGameObject.GetComponent<Image>().sprite = tempGameObject.GetComponent<Image>().sprite;
//            InventoryManager.Instance.RefreshItems();
//            if (targetGameObject.transform.parent.name == "ItemContainer")
//                InventoryManager.Instance.ItemChanged();
//        }
//        else
//        {
//            if (tempGameObject)
//                tempGameObject.SetActive(false);
//        }
//        canDrag = false;
//    }


//}