using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SynthesisList", menuName = "SynthesisListSO")]
public class SynthesisListSO : ScriptableObject
{
    public List<SynthesisSO> synthesisList = new List<SynthesisSO>();
    public bool IsSynthesisPossible(ItemSO item, InventorySO inventory)
    {
        foreach (var synthesis in synthesisList)
        {
            foreach (var output in synthesis.outputDic)
            {
                if (output.item.id == item.id)
                {
                    bool isPossible = true;
                    foreach (var input in synthesis.inputDic)
                    {
                        //没有这个物品或者物品数量不够
                        if (inventory.items.Contains(input.item) == false || !GameDataManager.Instance.MaterialAmountEnough(input.item.materialEnum, input.item.complexity * input.count))
                        {
                            isPossible = false;
                            break;
                        }
                    }
                    if (isPossible)
                        return true;
                }
            }
        }
        return false;
    }

    public bool IsSynthesisPossible(SynthesisSO synthesis, InventorySO inventory)
    {

        bool isPossible = true;
        foreach (var input in synthesis.inputDic)
        {
            if (inventory.items.Contains(input.item) == false || !GameDataManager.Instance.MaterialAmountEnough(input.item.materialEnum, input.item.complexity * input.count))
            {
                isPossible = false;
                break;
            }
        }
        if (isPossible)
            return true;

        return false;
    }





    public InventorySO AllSynthesis2Item(ItemSO item)
    {
        InventorySO tempInventory = ScriptableObject.CreateInstance<InventorySO>();
        tempInventory.inventoryEnum = InventorySO.InventoryEnum.Other;
        foreach (var synthesis in synthesisList)
        {
            foreach (var input in synthesis.inputDic)
            {
                if (input.item.id == item.id)
                {
                    for (int i = 0; i < synthesis.outputDic.Length; i++)
                    {
                        ItemSO temp = Instantiate(synthesis.outputDic[i].item);
                        tempInventory.DynamicAddItem(temp);
                    }


                    break;
                }
            }
        }
        return tempInventory;
    }








    public void StartSynthesis(SynthesisSO synthesis, InventorySO inventory, Vector3 postion)
    {
        foreach (var input in synthesis.inputDic)
        {
            GameDataManager.Instance.ChangeMaterialAmount(input.item.materialEnum, -input.item.complexity * input.count);
        }
        ItemManager.Instance.GenerateItem(synthesis, postion);
    }


    public void StartSynthesis(ItemSO item, InventorySO inventory, Vector3 postion)
    {

        foreach (var synthesis in synthesisList)
        {
            foreach (var output in synthesis.outputDic)
            {
                if (output.item.id == item.id)
                {
                    foreach (var input in synthesis.inputDic)
                    {
                        GameDataManager.Instance.ChangeMaterialAmount(input.item.materialEnum, -input.item.complexity * input.count);
                    }
                    ItemManager.Instance.GenerateItem(synthesis, postion);
                    return;
                }
            }
        }
    }



}
