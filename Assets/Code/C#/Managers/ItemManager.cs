using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 掉落物品管理器
/// </summary>
public class ItemManager : MonoBehaviour
{
    public enum ItemEnum
    {
        Remote,
        Melee,
        Ammo,
        Item,
        Empty,
        Equipment,
        Magic
    }
    public static ItemManager Instance { get; private set; }
    //掉落物的层级
    public LayerMask layerMask;
    private List<GameObject> itemList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void GenerateItem(ItemSO item, Vector3 position)
    {
        Generate(item, position);
    }
    public void GenerateItem(ItemSO[] item, Vector3 position)
    {
        foreach (var i in item)
        {
            Generate(i, position);
        }
    }
    public void GenerateItem(SynthesisSO synthesis, Vector3 position)
    {
        foreach (var output in synthesis.outputDic)
        {
            GameObject tempGO = output.item.itemPrefab;
            //tempGO.GetComponent<DropLoot>().GetItemSO().damageCount = output.damageCount;
            Generate(output.item, position);

        }
    }

    private GameObject Generate(ItemSO item, Vector3 position)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].name == item.itemPrefab.name + "(Clone)" && !itemList[i].gameObject.activeSelf)
            {

                itemList[i].transform.position = position;
                itemList[i].transform.rotation = Quaternion.identity;
                itemList[i].gameObject.SetActive(true);
                take = true;
                ret = itemList[i];
                ret.GetOrAddComponent<DropLoot>().SetItemSO(item);
                ret.GetComponent<DropLoot>().SetLayerMask(layerMask);
                break;

            }
        }
        if (!take)
        {
            ret = Instantiate(item.itemPrefab, position, Quaternion.identity);
            ret.SetActive(true);
            ret.GetOrAddComponent<DropLoot>().SetItemSO(item);
            ret.GetComponent<DropLoot>().SetLayerMask(layerMask);
            itemList.Add(ret);
            //Generate(item, position);
        }
        return ret;
    }
    private void OnDestroy()
    {
        itemList.Clear();
    }
}
