using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
/// <summary>
/// UI管理器
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
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

    private void Start()
    {
        GameInputManager.Instance.OnMenu += BagPressed;
    }



    [SerializeField] private GameObject displayDescriptionUI;
    [SerializeField] private DisplayDescriptionUI displayDescriptionUIComponent;

    [SerializeField] private GameObject hisMessageUI;
    [SerializeField] private TMPro.TextMeshProUGUI hisMessageText;

    [SerializeField] private GameObject currentMessageUI;
    [SerializeField] private TMPro.TextMeshProUGUI currentMessageText;

    [SerializeField] private GameObject ReviviUI;
    [SerializeField] private TMPro.TextMeshProUGUI ReviviText;

    [SerializeField] private GameObject mapUI;
    [SerializeField] private RawImage mapImage;

    [SerializeField] private GameObject occupationSelectionUI;

    [SerializeField] private Queue<string> messageQueue = new Queue<string>();

    [SerializeField] private List<GameObject> DailyUIs = new List<GameObject>();

    private bool canShowMessage = true;

    private void BagPressed(object sender, EventArgs e)
    {
        canShowMessage = !InventoryManager.Instance.IsBagActive();
    }

    private void MapPressed(object sender, EventArgs e)
    {
        mapUI.SetActive(!mapUI.activeSelf);
    }

    private void MessagePressed(object sender, EventArgs e)
    {
        if (canShowMessage)
        {
            if (hisMessageUI.activeSelf)
            {
                HideHisMessageUI();
            }
            else
            {
                ShowHisMessageUI();
            }
            RefreshHisMessage();
        }
    }

    public void RefreshHisMessage()
    {
        //if (hisMessageUI.activeSelf)
        //hisMessageText.text = GameDataManager.Instance.LoadHisMessage();
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    public void ShowNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            currentMessageText.text = messageQueue.Dequeue();
            HideHisMessageUI();
            ShowCurrentMessageUI();
        }
    }

    public void ConfirmMessage()
    {
        HideCurrentMessageUI();
        RefreshHisMessage();
        ShowNextMessage();
    }

    private int reviviCountdown = 0;

    public void StartRevivi(int countdown)
    {
        reviviCountdown = countdown;
        ReviviUI.SetActive(true);
        ReviviText.text = ">重构中...";
        //ReviviText.text += "\n" + ">" + damageCount.ToString();
        InvokeRepeating(nameof(CountDown), 1, 1);
    }

    private void CountDown()
    {
        reviviCountdown--;
        ReviviText.text += "\n" + ">" + reviviCountdown.ToString();

        if (reviviCountdown <= 0)
        {
            CancelInvoke(nameof(CountDown));
            HideReviviUI();
        }
    }

    public void ShowHisMessageUI()
    {
        hisMessageUI.SetActive(true);
    }

    public void ShowCurrentMessageUI()
    {
        currentMessageUI.SetActive(true);
    }

    public void HideHisMessageUI()
    {
        hisMessageUI.SetActive(false);
    }

    public void HideCurrentMessageUI()
    {
        currentMessageUI.SetActive(false);
    }

    public void ShowDescription(string description, Vector3 pos)
    {
        displayDescriptionUI.SetActive(true);
        displayDescriptionUIComponent.SetPos(pos);
        displayDescriptionUIComponent.SetDescription(description);
    }

    public void HideDescription()
    {
        displayDescriptionUI.SetActive(false);
    }

    public void ShowReviviUI()
    {
        ReviviUI.SetActive(true);
    }

    public void HideReviviUI()
    {
        ReviviUI.SetActive(false);
    }

    public void ShowOccupationSelectionUI()
    {
        Debug.Log("ShowOccupationSelectionUI");
        // occupationSelectionUI.SetActive(true);
    }

    public void HideOccupationSelectionUI()
    {
        occupationSelectionUI.SetActive(false);
    }

    public void ShowBag()
    {
        GameInputManager.Instance.Menu_performed(new InputAction.CallbackContext());
    }

    public void ChangeDailyUI(int UIIndex)
    {
        foreach (GameObject DailyUI in DailyUIs)
        {
            DailyUI.SetActive(false);
        }
        DailyUIs[UIIndex].SetActive(true);
    }

    private void OnDestroy()
    {
        GameInputManager.Instance.OnMenu -= BagPressed;
    }
}
