using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour, ICanMapInteraction
{
    [SerializeField] private InventorySO inventory;
    public void OnMapInteraction()
    {
        inventory=GameDataManager.Instance.LoadInventory(inventory, inventory.GetInventory2DataName());
        UIManager.Instance.ShowBag();
        InventoryManager.Instance.RefreshSecondaryItems(inventory);
    }

}
