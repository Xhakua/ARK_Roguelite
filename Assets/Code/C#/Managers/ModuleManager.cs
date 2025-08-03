using System.Collections.Generic;
using UnityEngine;
//×Óµ¯±à¼­¹ÜÀíÆ÷
public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance { get; private set; }

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject GenerateModule(ItemSO itemSO, Transform targetTransform, bool hasCost = false)
    {
        bool takeModule = false;
        GameObject ret = null;
        if (hasCost)
        {
            if (!PlayerManager.Instance.GetPlayer().ChangeMagicAmount(-itemSO.complexity))
            {
                return null;
            }
        }

        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeSelf)
            {
                if ((pool[i].name) == itemSO.itemPrefab.name + "(Clone)")
                {
                    pool[i].transform.SetParent(targetTransform);
                    pool[i].gameObject.SetActive(true);
                    takeModule = true;
                    ret = pool[i];
                    break;
                }
            }
        }
        if (!takeModule)
        {
            ret = Instantiate(itemSO.itemPrefab, targetTransform);
            ret.GetComponent<SpriteRenderer>().enabled = false;

            pool.Add(ret);
        }



        return ret;
    }




}
