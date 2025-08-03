using UnityEngine;
/// <summary>
/// �������Ĺ�����
/// </summary>
public class ManagersManager : MonoBehaviour
{
    private void Awake()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.Init();
        }
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.Init();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Init();
        }

        if (MapManager.Instance != null)
        {
            MapManager.Instance.Init();
        }

        if (LightManager.Instance != null)
        {
            LightManager.Instance.Init();
        }

    }



}
