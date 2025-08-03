using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Inventory", menuName = "InventorySO")]
public class InventorySO : ScriptableObject
{
    [SerializeField] private string inventory2DataName;
    public List<ItemSO> items = new List<ItemSO>();
    /// <summary>
    /// 和在UI上的显示位置有关
    /// </summary>
    public enum InventoryEnum
    {
        PlayerBag,
        Equipment,
        Other,
    }

    //[SerializeField] private bool isFixedDisplay;
    public InventoryEnum inventoryEnum;
    [Header("一定要加Empty，除非你想只进不出")]
    public List<ItemManager.ItemEnum> whatCanReceive;

    public override string ToString()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                Debug.Log(items[i].name);
            }
        }
        return "";
    }

    public void SetInventory2DataName(string name)
    {
        inventory2DataName = name;
    }

    public string GetInventory2DataName()
    {
        return inventory2DataName;
    }

    public void ClearName()
    {
        inventory2DataName = null;
    }



    public void SetSize(int size)
    {
        for (int i = 0; i < size; i++)
        {
            items.Add(null);
        }
    }

    public bool CanBagInteraction(int index)
    {
        if (index < items.Count)
        {
            return items[index].canBagInteraction;
        }
        return false;
    }



    public ItemManager.ItemEnum GetItemType(int index)
    {
        if(IsNull(index))
        {
            return ItemManager.ItemEnum.Empty;
        }
        if (index < items.Count)
        {
            return items[index].itemType;
        }
        return ItemManager.ItemEnum.Empty;
    }

    public ItemManager.ItemEnum GetItemType(ItemSO item)
    {
        if (items.Contains(item))
        {
            return item.itemType;
        }
        return ItemManager.ItemEnum.Empty;
    }

    public bool IsNull(int index)
    {
        if (index < items.Count)
        {
            return items[index] == null;
        }
        return true;
    }
    public bool IsNull()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
                return false;
        }
        return true;
    }

    public string GetName(int index)
    {
        if (index < items.Count)
        {
            return items[index].name;
        }
        return "";
    }



    public GameObject GetGameObject(int index)
    {
        if (IsNull(index))
            return null;
        if (index < items.Count)
        {
            return items[index].itemPrefab;
        }
        return null;
    }
    public Sprite GetSprite(int index)
    {
        if (!IsNull(index) && index < items.Count)
        {
            return items[index].itemSprite;
        }
        return null;
    }


    public static void SwithItems(InventorySO inventory, int index1, int index2)
    {
        if (index1 < inventory.items.Count && index2 < inventory.items.Count)
        {
            (inventory.items[index2], inventory.items[index1]) = (inventory.items[index1], inventory.items[index2]);
            InventoryManager.Instance.RefreshItems(inventory);
        }
    }

    //public static void SwithItems(InventorySO inventory1, ItemSO item1, InventorySO inventory2, ItemSO item2)
    //{
    //    if (inventory1.items.Contains(item1)
    //        && inventory2.items.Contains(item2)
    //        && inventory1.whatCanReceive.Contains(item2.itemType)
    //        && inventory2.whatCanReceive.Contains(item1.itemType)
    //        )
    //    {
    //        (inventory1.items[inventory1.items.IndexOf(item1)], inventory2.items[inventory2.items.IndexOf(item2)])
    //            = (inventory2.items[inventory2.items.IndexOf(item2)], inventory1.items[inventory1.items.IndexOf(item1)]);
    //        InventoryManager.Instance.RefreshItems(inventory1);
    //        InventoryManager.Instance.RefreshItems(inventory2);
    //    }
    //}
    public static void SwithItems(InventorySO inventory1, int index1, InventorySO inventory2, int index2)
    {
        Debug.Log($"SwithItems: {inventory1.name} {index1} <-> {inventory2.name} {index2}");
        Debug.Log(index1 < inventory1.items.Count);
        Debug.Log(index2 < inventory2.items.Count);
        Debug.Log(inventory1.whatCanReceive.Contains(inventory2.GetItemType(index2)));
        Debug.Log(inventory2.whatCanReceive.Contains(inventory1.GetItemType(index1)));
        if (index1 < inventory1.items.Count
            && index2 < inventory2.items.Count
            && inventory1.whatCanReceive.Contains(inventory2.GetItemType(index2))
            && inventory2.whatCanReceive.Contains(inventory1.GetItemType(index1))
            )
        {
            (inventory1.items[index1], inventory2.items[index2]) = (inventory2.items[index2], inventory1.items[index1]);
            InventoryManager.Instance.RefreshItems(inventory1);
            if (inventory1 != inventory2)
                InventoryManager.Instance.RefreshItems(inventory2);
        }
    }


    public void SwithItems(int index1, int index2)
    {
        if (index1 < items.Count && index2 < items.Count)
        {
            (items[index2], items[index1]) = (items[index1], items[index2]);
            InventoryManager.Instance.RefreshItems(this);
        }
    }

    public int AddItem(ItemSO item, bool stack = true)
    {
        if (!whatCanReceive.Contains(item.itemType))
        {
            return -1;
        }
        if (stack)
        {
            for (int i = 0; i < items.Count; i++)
            {

                if (items[i] != null && items[i].id == item.id)
                {
                    if (item.itemType == ItemManager.ItemEnum.Ammo)
                        PlayerManager.Instance.GetPlayer().ChangeResourceAmount(item);
                    else
                        GameDataManager.Instance.ChangeMaterialAmount(item.materialEnum, item.complexity);
                    InventoryManager.Instance.RefreshItems(this);
                    return i;
                }
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                //items[i] = Instantiate(item);
                items[i] = item;
                InventoryManager.Instance.RefreshItems(this);
                return i;
            }
        }
        return -1;

    }
    /// <summary>
    /// 数据转换专用
    /// </summary>
    /// <param name="item"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public int AddItem(ItemSO item, int index)
    {
        if (!whatCanReceive.Contains(item.itemType))
        {
            return -1;
        }
        if (index < items.Count)
        {
            items[index] = item;
            InventoryManager.Instance.RefreshItems(this);
            return index;
        }
        return -1;
    }


    public void AddNullItem(int index)
    {
        items[index] = null;
    }


    public void ClearInventory()
    {
        //InventoryManager.Instance.RefreshItems(this);
        items.Clear();
    }

    public int GetComplexity(int index)
    {
        if (index < items.Count)
        {
            return items[index].complexity;
        }
        return 0;
    }
    public int GetComplexity(ItemSO item)
    {
        if (items.Contains(item))
        {
            return item.complexity;
        }
        return 0;
    }

    public int GetID(int index)
    {
        if (index < items.Count)
        {
            return items[index].id;
        }
        return 0;
    }
    public int GetID(ItemSO item)
    {
        if (items.Contains(item))
        {
            return item.id;
        }
        return 0;
    }

    //public int GetCount(int index)
    //{
    //    if (index < items.Count)
    //    {
    //        return items[index].damageCount;
    //    }
    //    return 0;
    //}
    //public int GetCount(ItemSO item)
    //{
    //    if (items.Contains(item))
    //    {
    //        return items[items.IndexOf(item)].damageCount;
    //    }
    //    return 0;
    //}


    //public void DecreaseItem(int index, int dec = 1)
    //{
    //    if (index < items.Count)
    //    {
    //        items[index].damageCount -= dec;
    //        if (isBelongs2ThePlayer)
    //            InventoryManager.Instance.RefreshItems();
    //        else
    //            InventoryManager.Instance.RefreshSecondaryItems(this);
    //        if (items[index].damageCount <= 0)
    //        {
    //            items[index] = null;
    //            if (isBelongs2ThePlayer)
    //                InventoryManager.Instance.RefreshItems();
    //            else
    //                InventoryManager.Instance.RefreshSecondaryItems(this);
    //        }
    //    }
    //}

    //public void DecreaseItem(ItemSO item, int dec = 1)
    //{
    //    Debug.Log(123);
    //    if (items.Contains(item))
    //    {
    //        items[items.IndexOf(item)].damageCount -= dec;
    //        if (item.damageCount <= 0)
    //        {
    //            items[items.IndexOf(item)] = null;
    //            if (isBelongs2ThePlayer)
    //                InventoryManager.Instance.RefreshItems();
    //            else
    //                InventoryManager.Instance.RefreshSecondaryItems(this);
    //        }
    //        else
    //        {
    //            if (isBelongs2ThePlayer)
    //                InventoryManager.Instance.RefreshItems();
    //            else
    //                InventoryManager.Instance.RefreshSecondaryItems(this);
    //        }
    //    }
    //}

    //items[items.IndexOf(item)]****************************

    public ItemSO GetItem(int index)
    {
        if (index < items.Count)
        {
            return items[index];
        }
        return null;
    }


    public int GetLength()
    {
        return items.Capacity;
    }
    public void DynamicAddItem(ItemSO item, bool refresh = true)
    {
        items.Add(item);
        if (!refresh) { return; }
        InventoryManager.Instance.RefreshItems(this);
    }


}

