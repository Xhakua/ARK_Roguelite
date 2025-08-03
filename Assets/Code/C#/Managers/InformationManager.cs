using UnityEngine;
/// <summary>
/// ��Ϣ������
/// </summary>
public class InformationManager : MonoBehaviour
{
    public static InformationManager Instance { get; private set; }

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
    /// <summary>
    /// ��ȡ��ʷ��Ϣ
    /// </summary>
    /// <returns></returns>
    public string GetHisMessage()
    {
        return "!!!!!!!!!!!!!";
    }
    /// <summary>
    /// �����Ϣ����ʷ��Ϣ��
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(string message)
    {
        //string hisMessage = GameDataManager.Instance.LoadHisMessage();
        //hisMessage += "\n" + DayNightManager.Instance.GetDateStr() + " " + DayNightManager.Instance.GetCurTimeStr() + "  " + message;
        //GameDataManager.Instance.SaveHisMessage(hisMessage);

        UIManager.Instance.AddMessage(message);
        //Debug.Log("AddMessage: " + message);
    }


}
