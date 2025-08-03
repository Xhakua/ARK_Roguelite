using UnityEngine;
/// <summary>
/// 信息管理器
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
    /// 获取历史消息
    /// </summary>
    /// <returns></returns>
    public string GetHisMessage()
    {
        return "!!!!!!!!!!!!!";
    }
    /// <summary>
    /// 添加消息到历史消息中
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
