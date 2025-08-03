using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Íæ¼Ò¹ÜÀíÆ÷
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    [SerializeField] private OccupationDataListSO occupationDataListSO;

    private int selectedOccupationIndex = 0;
    [SerializeField] private GameObject playerPrefab;

    private Transform playerSpawnPoint;
    protected List<Player> playerList = new List<Player>();
    [SerializeField] private Unity.Cinemachine.CinemachineVirtualCameraBase virtualCamera;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public void Init()
    {
        playerSpawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
        Debug.Log("PlayerManager Init" + playerSpawnPoint.position);

        CreatePlayer(playerSpawnPoint, occupationDataListSO.occupationDataList[selectedOccupationIndex]);
        UIManager.Instance.ShowOccupationSelectionUI();
        virtualCamera.Follow = playerList[0].transform;
    }
    public Player GetPlayer(int index = 0)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] == null)
            {
                playerList.RemoveAt(i);
                break;
            }
        }
        if (index >= playerList.Count)
        {
            CreatePlayer(playerSpawnPoint, occupationDataListSO.occupationDataList[selectedOccupationIndex]);
        }
        else if (playerList[index] == null)
        {
            CreatePlayer(playerSpawnPoint, occupationDataListSO.occupationDataList[selectedOccupationIndex]);
        }
        return playerList[index];
    }

    public void CreatePlayer(Transform spawnPoint, CharacterDataSO occupationData)
    {

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        playerList.Add(player.GetComponent<Player>());
        player.GetComponent<Player>().SetOccupationDataSO(occupationData);
        player.SetActive(true);
    }
    public void ChangePlayerOccupation(int index)
    {
        selectedOccupationIndex = index;
        playerList[0].SetOccupationDataSO(occupationDataListSO.occupationDataList[selectedOccupationIndex]);
    }

    public CharacterDataSO GetOccupationDataSO(int index)
    {
        return occupationDataListSO.occupationDataList[index];
    }
    public int GetOccupationListCount()
    {
        return occupationDataListSO.occupationDataList.Count;
    }

    private void OnDestroy()
    {
        playerList.Clear();
        Debug.Log("PlayerManager Destroy");
    }


}
