using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Íæ¼ÒHUD
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    private GameObject playerGameObject;



    [SerializeField] private Image hpBarImage;
    [SerializeField] private Image mpBarImage;


    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;

    [SerializeField] private TextMeshProUGUI resourceText;
    [SerializeField] private TextMeshProUGUI mataerialText;


    private ISetHealthUI hasProgress;
    private IReactionsUI hasBuff;
    private CharacterDataSO occupationDataSO;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        playerGameObject = PlayerManager.Instance.GetPlayer().gameObject;
        occupationDataSO = playerGameObject.GetComponent<Player>().OccupationData;
        hasProgress = playerGameObject.GetComponent<ISetHealthUI>();
        hasBuff = playerGameObject.GetComponent<IReactionsUI>();

        playerGameObject.GetComponent<Player>().OnResourceChanged += Player_OnResourceChanged;
        hasProgress.OnHealthUIChanged += HasProgress_OnProgressChanged;
        hasBuff.OnBuffChanged += HasBuff_OnBuffChanged;
        GameDataManager.Instance.OnMataerialAmountChanged += GameDataManager_OnMataerialAmountChanged;




        Debug.Log("HUDstart");
    }
    private void GameDataManager_OnMataerialAmountChanged(object sender, int[] e)
    {
        for (int i = 0; i < e.Length; i++)
        {
            mataerialText.text = mataerialText.text + "\n" + (InventoryManager.MaterialEnum)i + ":" + e[i];
        }
    }

    private void Player_OnResourceChanged(object sender, int e)
    {
        resourceText.text = e.ToString();
    }



    private void HasBuff_OnBuffChanged(object sender, IReactionsUI.OnBuffChangedEventArgs e)
    {
    }

    private void HasProgress_OnProgressChanged(object sender, ISetHealthUI.OnProgressChangedEventArgs e)
    {
        hpBarImage.fillAmount = e.hp / e.hpMax;
        mpBarImage.fillAmount = e.mp / e.mpMax;
        hpText.text = e.hp + "/" + e.hpMax;
        mpText.text = e.mp + "/" + e.mpMax;
    }
}

