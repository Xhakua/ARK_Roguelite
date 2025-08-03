using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 库存管理器
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public enum BagGridStateEnum
    {
        normal,
        toBeUnlocked,
        locked,
        empty,
    }
    public enum MaterialEnum
    {
        i,
        ii,
        iii,
        iv,
        v,
    }
    public static InventoryManager Instance { get; private set; }
    [SerializeField] private InventorySO inventory;
    [SerializeField] private InventorySO equipmentsInventory;

    private InventorySO inventorySecondary;
    //private InventorySO inventoryTemp;
    [SerializeField] private SynthesisListSO synthesisList;
    [Header("Containers")]
    [SerializeField] private GameObject itemContainer;
    [SerializeField] private GameObject handItemContainer;
    [SerializeField] private GameObject equipmentContainer;
    [SerializeField] private GameObject bagInteractionItemContainer;
    [SerializeField] private GameObject bagInteraction_OtherContainer;
    [Header("Prefabs")]
    [SerializeField] private GameObject bagInteraction_DropDownPrefab;
    [SerializeField] private GameObject bagInteraction_ScrollbarPrefab;
    [SerializeField] private TextMeshProUGUI bagInteraction_ScrollbarTextPrefab;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private GameObject playerBag;
    [SerializeField] private GameObject clickBlock;

    public GameObject tempGameObject;
    public GameObject Infobox;


    public event EventHandler OnItemChanged;
    public event EventHandler<int> OnItemPressed;


    /// <summary>
    /// 所有合成列表
    /// </summary>
    public SynthesisListSO SynthesisListSO { get => synthesisList; set => synthesisList = value; }
    public void ItemPressed(int index)
    {
        OnItemPressed?.Invoke(this, index);
    }
    public void ItemChanged()
    {
        OnItemChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        GameInputManager.Instance.OnMenu += BagPressed;
        PlayerManager.Instance.GetPlayer().OnHandleItemChanged += SwitchHandleItems;
        InventoryManager.Instance.OnItemPressed += ItemManager_OnItemPressed;
        RefreshItems();
        RefreshHandItem();
    }

    private void SwitchHandleItems(object sender, EventArgs e)
    {
        OutLineHandleItem(PlayerManager.Instance.GetPlayer().GetInventoryIndex());
    }

    private void ItemManager_OnItemPressed(object sender, int e)
    {
        if (inventory.IsNull(e) == false && inventory.CanBagInteraction(e))
        {
            if (inventory.GetGameObject(e).TryGetComponent<IAble2BagInteraction_Grid>(out IAble2BagInteraction_Grid grid))
            {
                RefreshSecondaryItems(grid.BagInteraction());
            }
            if (inventory.GetGameObject(e).TryGetComponent<IAble2BagInteraction_DropDown>(out IAble2BagInteraction_DropDown dropDown))
            {
                RefreshBagInteraction_DropDown(dropDown.BagInteraction(), e);
            }
            if (inventory.GetGameObject(e).TryGetComponent<IAble2BagInteraction_Scrollbar>(out IAble2BagInteraction_Scrollbar scrollbar))
            {
                RefreshBagInteraction_Scrollbar(scrollbar.GetScrollbarMultipliers(), e);
            }
        }

    }

    private void RefreshBagInteraction_Scrollbar(Dictionary<string, float> dictionary, int e)
    {
        GameObject[] gameObjects = new GameObject[dictionary.Count];
        float[] values = new float[dictionary.Count];

        for (int j = 0; j < dictionary.Count; j++)
        {
            var item = dictionary.ElementAt(j);
            GameObject go = Instantiate(bagInteraction_ScrollbarPrefab, bagInteraction_OtherContainer.transform);
            go.SetActive(true);
            TextMeshProUGUI textMeshPro = go.GetComponentInChildren<TextMeshProUGUI>();


            Scrollbar scrollbar = go.GetComponentInChildren<Scrollbar>();
            scrollbar.value = PlayerPrefs.GetFloat(item.Key);
            textMeshPro.text = item.Key + ":" + (int)(item.Value * scrollbar.value);
            int index = j;

            scrollbar.onValueChanged.AddListener(delegate
            {
                PlayerPrefs.SetFloat(item.Key, scrollbar.value);
                PlayerPrefs.Save();
                values[index] = scrollbar.value * item.Value;
                textMeshPro.text = item.Key + ":" + (int)values[index];
                inventory.GetGameObject(e).GetComponent<IAble2BagInteraction_Scrollbar>().SetScrollbarValues(values);
            });

            gameObjects[j] = go;
        }
    }


    public void RefreshBagInteraction_DropDown(Enum @enum, int e)
    {
        bagInteractionItemContainer.SetActive(false);
        for (int i = 0; i < bagInteraction_OtherContainer.transform.childCount; i++)
        {
            Destroy(bagInteraction_OtherContainer.transform.GetChild(i).gameObject);
        }
        GameObject go = Instantiate(bagInteraction_DropDownPrefab, bagInteraction_OtherContainer.transform);
        go.SetActive(true);
        TMP_Dropdown dropdown = go.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string>(System.Enum.GetNames(@enum.GetType())));
        dropdown.value = PlayerPrefs.GetInt(inventory.GetName(e));//用这个名字真的没问题吗?很有问题,有时会有"(Clone)"
        dropdown.onValueChanged.AddListener(delegate
        {
            inventory.GetGameObject(e).GetComponent<IAble2BagInteraction_DropDown>().SetEnum((dropdown.value));
            //Debug.Log(inventory.GetName(e));
            PlayerPrefs.SetInt(inventory.GetName(e), dropdown.value);
            PlayerPrefs.Save();
        });

    }

    public void RefreshSecondaryItems(InventorySO inventory)//这里换了东西无法刷新,不好随便调用这个,考虑在inventory上加一个player唯一标识(下策);快想想别的办法;最后还是用了这个方法
    {
        if (playerBag.activeSelf == false)
        {
            return;
        }
        bagInteractionItemContainer.SetActive(true);


        if (inventory == null)
            return;

        inventory = GameDataManager.Instance.LoadInventory(inventory, inventory.GetInventory2DataName());

        SetInventorySecondary(inventory);




        for (int i = 0; i < bagInteractionItemContainer.transform.childCount; i++)
            Destroy(bagInteractionItemContainer.transform.GetChild(i).gameObject);
        for (int i = 0; i < bagInteraction_OtherContainer.transform.childCount; i++)
        {
            Destroy(bagInteraction_OtherContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory.items.Count; i++)
        {

            GameObject item = Instantiate(baseGameObject, bagInteractionItemContainer.transform);
            item.SetActive(true);
            if (inventory.IsNull(i) == false)
            {
                ItemSingleUI itemSingleUI = item.transform.GetComponent<ItemSingleUI>();
                itemSingleUI.SetBagGridState(InventoryManager.BagGridStateEnum.normal);
                itemSingleUI.bagGrid.GetComponent<Image>().sprite = inventory.GetSprite(i);
                itemSingleUI.bagGrid.GetComponent<Image>().color = Color.white;
                itemSingleUI.HideCountText(true);
            }
            else
            {
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.empty);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(inventory);

        }
    }

    public void RefreshSecondaryItems_Deprecated()//这里换了东西无法刷新,不好随便调用这个,考虑在inventory上加一个player唯一标识(下策);快想想别的办法;最后还是用了这个方法
    {
        if (playerBag.activeSelf == false)
        {
            return;
        }
        bagInteractionItemContainer.SetActive(true);
        for (int i = 0; i < bagInteractionItemContainer.transform.childCount; i++)
            Destroy(bagInteractionItemContainer.transform.GetChild(i).gameObject);
        if (inventorySecondary == null)
            return;
        for (int i = 0; i < inventorySecondary.items.Count; i++)
        {

            GameObject item = Instantiate(baseGameObject, bagInteractionItemContainer.transform);
            item.SetActive(true);
            if (inventorySecondary.IsNull(i) == false)
            {
                ItemSingleUI itemSingleUI = item.transform.GetComponent<ItemSingleUI>();
                itemSingleUI.SetBagGridState(InventoryManager.BagGridStateEnum.normal);
                itemSingleUI.bagGrid.GetComponent<Image>().sprite = inventorySecondary.GetSprite(i);
                itemSingleUI.bagGrid.GetComponent<Image>().color = Color.white;
                itemSingleUI.HideCountText(true);
            }
            else
            {
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.empty);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(inventorySecondary);

        }
    }


    private void BagPressed(object sender, EventArgs e)
    {
        bagInteractionItemContainer.SetActive(false);
        for (int i = 0; i < bagInteractionItemContainer.transform.childCount; i++)
            Destroy(bagInteractionItemContainer.transform.GetChild(i).gameObject);
        for (int i = 0; i < bagInteraction_OtherContainer.transform.childCount; i++)
            Destroy(bagInteraction_OtherContainer.transform.GetChild(i).gameObject);
        playerBag.SetActive(!playerBag.activeSelf);
        handItemContainer.SetActive(!handItemContainer.activeSelf);
        clickBlock.SetActive(!clickBlock.activeSelf);
        if (inventorySecondary != null && playerBag.activeSelf == false)
        {
            if (GameDataManager.Instance.OtherIventorySave)
            {
                GameDataManager.Instance.SaveInventory(inventorySecondary, inventorySecondary.GetInventory2DataName());
                //Debug.Log(inventorySecondary.name);
                //Debug.Log(inventorySecondary.GetInventory2DataName());
                //Debug.Log(inventorySecondary);
                //Debug.Log("SaveInventory");
            }

            inventorySecondary = null;
        }
        RefreshItems();

        RefreshItems(equipmentsInventory);
        RefreshHandItem();
        PlayerManager.Instance.GetPlayer().SetEquipment();
    }

    public void RefreshHandItem()
    {
        Debug.Log("RefreshHandItem");
        if (playerBag.activeSelf == true)
        {
            return;
        }
        int count = 0;
        for (int i = 0; i < handItemContainer.transform.childCount; i++)
        {

            if (handItemContainer.transform.GetChild(i).GetComponent<ItemSingleUI>().GetBagGridState() == InventoryManager.BagGridStateEnum.normal)
            {
                count++;
            }
        }
        if (count == 2)
        {
            return;
        }
        for (int i = 0; i < handItemContainer.transform.childCount; i++)
        {
            Destroy(handItemContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < PlayerManager.Instance.GetPlayer().GetHandInventorySizeSize(); i++)
        {

            GameObject item = Instantiate(baseGameObject, handItemContainer.transform);
            item.SetActive(true);
            if (inventory.IsNull(i) == false)
            {
                ItemSingleUI itemSingleUI = item.transform.GetComponent<ItemSingleUI>();
                itemSingleUI.SetBagGridState(InventoryManager.BagGridStateEnum.normal);
                itemSingleUI.bagGrid.GetComponent<Image>().sprite = inventory.GetSprite(i);
                itemSingleUI.bagGrid.GetComponent<Image>().color = Color.white;
                itemSingleUI.HideCountText(true);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(inventory);

        }
    }




    public void OutLineHandleItem(int index)
    {
        for (int i = 0; i < handItemContainer.transform.childCount; i++)
        {
            handItemContainer.transform.GetChild(i).transform.GetChild(0).GetComponent<Outline>().enabled = false;
        }
        handItemContainer.transform.GetChild(index).transform.GetChild(0).GetComponent<Outline>().enabled = true;//有bug,还没有发现为什么要加一
        //哪里有什么加一,现在为什么看不见加一了
    }
    public void RefreshItems()
    {
        if (playerBag.activeSelf == false)
        {
            return;
        }

        for (int i = 0; i < itemContainer.transform.childCount; i++)
        {
            Destroy(itemContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory.items.Count; i++)
        {

            GameObject item = Instantiate(baseGameObject, itemContainer.transform);
            item.SetActive(true);
            if (inventory.IsNull(i) == false)
            {
                ItemSingleUI itemSingleUI = item.transform.GetComponent<ItemSingleUI>();
                itemSingleUI.SetBagGridState(InventoryManager.BagGridStateEnum.normal);
                itemSingleUI.bagGrid.GetComponent<Image>().sprite = inventory.GetSprite(i);
                itemSingleUI.bagGrid.GetComponent<Image>().color = Color.white;
                itemSingleUI.HideCountText(true);
            }
            else
            {
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.empty);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(inventory);

        }
    }
    public void RefreshItems(InventorySO targetInventorySO)
    {
        if (playerBag.activeSelf == false)
        {
            return;
        }
        Transform targetTransform;
        switch (targetInventorySO.inventoryEnum)
        {
            case InventorySO.InventoryEnum.PlayerBag:
                targetTransform = itemContainer.transform;
                break;
            case InventorySO.InventoryEnum.Equipment:
                targetTransform = equipmentContainer.transform;
                break;
            case InventorySO.InventoryEnum.Other:
                bagInteractionItemContainer.SetActive(true);
                targetTransform = bagInteractionItemContainer.transform;
                break;
            default:
                targetTransform = bagInteractionItemContainer.transform;
                break;
        }
        SetInventorySecondary(targetInventorySO);

        for (int i = 0; i < targetTransform.childCount; i++)
        {
            Destroy(targetTransform.GetChild(i).gameObject);
        }
        for (int i = 0; i < targetInventorySO.items.Capacity; i++)
        {

            GameObject item = Instantiate(baseGameObject, targetTransform);
            item.SetActive(true);
            if (targetInventorySO.IsNull(i) == false)
            {
                ItemSingleUI itemSingleUI = item.transform.GetComponent<ItemSingleUI>();
                itemSingleUI.SetBagGridState(InventoryManager.BagGridStateEnum.normal);
                itemSingleUI.bagGrid.GetComponent<Image>().sprite = targetInventorySO.GetSprite(i);
                itemSingleUI.bagGrid.GetComponent<Image>().color = Color.white;
                itemSingleUI.HideCountText(true);
            }
            else
            {
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.empty);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(targetInventorySO);

        }
    }
    public InventorySO GetInventory()
    {
        return inventory;
    }

    public void SearchSynthesisItem(int index)
    {
        if (inventory.IsNull(index) == false/*&&synthesisList.IsSynthesisPossible(inventory)*/)
        {
            RefreshSynthesisItems(synthesisList.AllSynthesis2Item(inventory.GetItem(index)));
            //读取临时背包,并且生成ui
        }
    }

    public void SynthesisItem(SynthesisSO synthesis)
    {
        if (synthesisList.IsSynthesisPossible(synthesis, this.inventory))
        {
            synthesisList.StartSynthesis(synthesis, this.inventory, PlayerManager.Instance.GetPlayer().transform.position);
        }
    }

    public void SynthesisItem(ItemSO item)
    {
        InventorySO[] tempInventory = new InventorySO[2];
        tempInventory[0] = inventory;
        tempInventory[1] = inventorySecondary;
        if (synthesisList.IsSynthesisPossible(item, this.inventory))
        {
            synthesisList.StartSynthesis(item, this.inventory, PlayerManager.Instance.GetPlayer().transform.position);
        }
    }




    public void RefreshSynthesisItems(InventorySO inventory)
    {
        bagInteractionItemContainer.SetActive(true);
        SetInventorySecondary(inventory);
        for (int i = 0; i < bagInteractionItemContainer.transform.childCount; i++)
            Destroy(bagInteractionItemContainer.transform.GetChild(i).gameObject);

        for (int i = 0; i < inventory.items.Count; i++)
        {

            GameObject item = Instantiate(baseGameObject, bagInteractionItemContainer.transform);
            item.SetActive(true);
            if (inventory.IsNull(i) == false)
            {
                item.transform.GetComponent<ItemSingleUI>().bagGrid.GetComponent<Image>().sprite = inventory.GetSprite(i);
            }

            item.transform.GetComponent<ItemSingleUI>().SetIndex(i);
            item.transform.GetComponent<ItemSingleUI>().SetInventory(inventory);

            if (synthesisList.IsSynthesisPossible(inventory.GetItem(i), this.inventory))
            {
                item.transform.GetComponent<ItemSingleUI>().SetCanSynthesis(true);
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.toBeUnlocked);
            }
            else
            {
                item.transform.GetComponent<ItemSingleUI>().SetCanSynthesis(false);
                item.transform.GetComponent<ItemSingleUI>().SetBagGridState(InventoryManager.BagGridStateEnum.locked);
            }


        }






    }


    public bool IsBagActive()
    {
        return playerBag.activeSelf;
    }

    private void SetInventorySecondary(InventorySO inventory)
    {
        if (inventorySecondary == null)
            inventorySecondary = inventory;
        else
        {
            if (GameDataManager.Instance.OtherIventorySave)
            {
                GameDataManager.Instance.SaveInventory(inventorySecondary, inventorySecondary.GetInventory2DataName());
                //Debug.Log(inventorySecondary.name);
                //Debug.Log(inventorySecondary.GetInventory2DataName());
                //Debug.Log(inventorySecondary);
                //Debug.Log("SaveInventory");
            }

            inventorySecondary = inventory;

        }
    }

}
