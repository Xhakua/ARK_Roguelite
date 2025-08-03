using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public InventorySO AllITEMINVENTORY;

    public event EventHandler<int[]> OnMataerialAmountChanged;

    public bool UseMapData = true;
    public bool LoadPlayerInventoryOnStart = true;
    public bool LoadMaterialGradeAndAmontsOnStart = true;
    public bool OtherIventorySave = true;

    private int[] mataerialAmount = new int[5];


    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    Init();
    //}

    public void Init()
    {
        for (int i = 0; i < mataerialAmount.Length; i++)
        {
            mataerialAmount[i] = PlayerPrefs.GetInt("MaterialAmount" + (InventoryManager.MaterialEnum)i, 0);
        }
    }

    public int GetMaterialAmount(InventoryManager.MaterialEnum materialEnum)
    {
        int index = (int)materialEnum;
        PlayerPrefs.GetInt("MaterialAmount" + materialEnum, mataerialAmount[index]);
        return mataerialAmount[index];
    }

    public void ChangeMaterialAmount(InventoryManager.MaterialEnum materialEnum, int changeAmont)
    {
        int index = (int)materialEnum;
        mataerialAmount[index] = GetMaterialAmount(materialEnum) + changeAmont;
        PlayerPrefs.SetInt("MaterialAmount" + materialEnum, mataerialAmount[index]);
        OnMataerialAmountChanged?.Invoke(this, mataerialAmount);
    }

    public bool MaterialAmountEnough(InventoryManager.MaterialEnum materialEnum, int amount)
    {
        return GetMaterialAmount(materialEnum) >= amount;
    }

    public void SavePlayerInventory()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, "PlayerInventoryData.xk");
        string inventoryData = Iventory2Data(PlayerManager.Instance.GetPlayer().GetInventory());
        //string inventoryDataJson = JsonUtility.ToJson(inventoryData);
        File.WriteAllText(filePath, inventoryData);
    }


    public void LoadPlayerInventory()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "PlayerInventoryData.xk");
        if (File.Exists(filePath))
        {
            Debug.Log("Load Player Inventory");
            string inventoryData = File.ReadAllText(filePath);
            //string inventoryData = JsonUtility.FromJson<string>(inventoryDataJson);
            Data2Inventory(inventoryData, PlayerManager.Instance.GetPlayer().GetInventory());
        }
    }

    public void SaveInventory(InventorySO inventory, string name)
    {
        if (string.IsNullOrEmpty(name))
            return;
        //Debug.LogError("Inventory Name is Empty");
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string directoryPath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "InventoryData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string boxDataFilePath = Path.Combine(directoryPath, sceneName + "_" + name + ".xk");
        string inventoryData = Iventory2Data(inventory);
        //string boxDataJson = JsonUtility.ToJson(inventoryData);
        //string boxDataJson = JsonUtility.ToJson(inventory);
        File.WriteAllText(boxDataFilePath, inventoryData);
    }

    public InventorySO LoadInventory(InventorySO inventory, string name)
    {
        if (string.IsNullOrEmpty(name))
            return inventory;
        //Debug.LogError("Inventory Name is Empty");
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string boxDataFilePath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "InventoryData", sceneName + "_" + name + ".xk");
        if (File.Exists(boxDataFilePath))
        {
            string inventoryData = File.ReadAllText(boxDataFilePath);
            //string inventoryData = JsonUtility.FromJson<string>(boxDataJson);
            Data2Inventory(inventoryData, inventory);
            //JsonUtility.FromJsonOverwrite(boxDataJson, inventory);
        }
        else
        {
            SaveInventory(inventory, name);
        }
        return inventory;
    }

    public void SaveInventory(InventorySO inventory)
    {
        if (string.IsNullOrEmpty(inventory.GetInventory2DataName()))
            return;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string directoryPath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "InventoryData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string boxDataFilePath = Path.Combine(directoryPath, sceneName + "_" + inventory.GetInventory2DataName() + ".xk");

        string inventoryData = Iventory2Data(inventory);
        //string boxDataJson = JsonUtility.ToJson(inventoryData);

        File.WriteAllText(boxDataFilePath, inventoryData);
    }

    public InventorySO LoadInventory(InventorySO inventory)
    {
        if (string.IsNullOrEmpty(inventory.GetInventory2DataName()))
            return inventory;
        //Debug.LogError("Inventory Name is Empty");
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string boxDataFilePath
            = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "InventoryData", sceneName + "_" + inventory.GetInventory2DataName() + ".xk");
        if (File.Exists(boxDataFilePath))
        {
            string inventoryData = File.ReadAllText(boxDataFilePath);
            //string inventoryData = JsonUtility.FromJson<string>(boxDataJson);
            Data2Inventory(inventoryData, inventory);
            //JsonUtility.FromJsonOverwrite(boxDataJson, inventory);
        }
        else
        {
            SaveInventory(inventory);
        }
        return inventory;
    }









    private string Iventory2Data(InventorySO sourceInventory)//先只存内容吧,其他的类型不存出bug再说//都要存
    {
        List<int> data = new List<int>();

        string str = "";

        str += sourceInventory.GetLength() + " ";
        str += sourceInventory.inventoryEnum + " ";
        foreach (var item in sourceInventory.items)
        {
            if (item == null)
            {
                data.Add(0);
                continue;
            }
            data.Add(item.id);
        }
        foreach (var item in data)
        {
            str += item + " ";
        }
        //Debug.Log(str);
        return str;
    }

    private void Data2Inventory(string str, InventorySO targetInventory)
    {
        targetInventory.ClearInventory();
        string[] dataStr = str.Split(' ');
        List<int> data = new List<int>();
        int size = 0;
        for (int i = 0; i < dataStr.Length; i++)
        {
            if (i == 0)
            {
                targetInventory.SetSize(int.Parse(dataStr[i]));
                size = int.Parse(dataStr[i]);
                continue;
            }
            if (i == 1)
            {
                targetInventory.inventoryEnum = (InventorySO.InventoryEnum)System.Enum.Parse(typeof(InventorySO.InventoryEnum), dataStr[i]);
                continue;
            }
            if (string.IsNullOrEmpty(dataStr[i]))
                continue;
            data.Add(int.Parse(dataStr[i]));
        }
        for (int i = 0; i < size; i++)
        {
            if (data[i] == 0)
            {
                //targetInventory.AddItem(null);
                //targetInventory.AddNullItem(i);
                continue;
            }
            targetInventory.AddItem(AllITEMINVENTORY.GetItem(data[i] - 1), i);//-1是因为id是从1开始的
        }
    }











}
//public void SaveInventorySO2Data(Vector2Int pos, InventorySO inventory)
//{
//    string directoryPath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData");
//    if (!Directory.Exists(directoryPath))
//    {
//        Directory.CreateDirectory(directoryPath);
//    }

//    string boxDataFilePath = Path.Combine(directoryPath, "InventoryData_" + pos.x + "_" + pos.y + ".xk");
//    string boxDataJson = JsonUtility.ToJson(inventory);
//    File.WriteAllText(boxDataFilePath, boxDataJson);
//}

//public InventorySO LoadData2InventorySO(Vector2Int pos, InventorySO inventory)
//{
//    string boxDataFilePath = Path.Combine(Application.persistentDataPath, "DeeperDarkerGameData", "InventoryData_" + pos.x + "_" + pos.y + ".xk");
//    if (File.Exists(boxDataFilePath))
//    {
//        string boxDataJson = File.ReadAllText(boxDataFilePath);
//        JsonUtility.FromJsonOverwrite(boxDataJson, inventory);
//    }
//    else
//    {
//        SaveInventorySO2Data(pos, inventory);
//        //inventory.ClearInventory();
//    }
//    return inventory;
//}