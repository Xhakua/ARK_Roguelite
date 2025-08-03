using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Buff管理器
/// </summary>
public class BuffManager : MonoBehaviour
{
    public enum BuffEnum
    {
        Overlap,
        Extened,
        Refresh,
    }
    public const int BUFFCOUNTMAX = 99;

    public static BuffManager Instance { get; private set; }
    public GameObject buffPrefab_display;
    public GameObject buffPrefab;
    public Transform buffParent;
    public BuffListSO buffListSO;
    private GameObject buff;

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
    /// 添加Buff到目标对象
    /// </summary>
    /// <param name="buffID"></param>
    /// <param name="target目标"></param>
    public void AddBuff(int buffID, GameObject target)
    {
        Transform parentTransform = GetBuffParent(target);
        bool display = target == PlayerManager.Instance.GetPlayer().gameObject;
        List<int> buffArray = target.GetComponent<ISufferBuff>().GetBuffStructs();

        switch (buffListSO.buffList[buffID].buffEnum)
        {
            case BuffEnum.Overlap:
                HandleOverlapBuff(buffID, target, parentTransform, buffArray, display);
                break;
            case BuffEnum.Refresh:
                HandleRefreshBuff(buffID, target, parentTransform, buffArray, display);
                break;
            case BuffEnum.Extened:
                HandleExtendedBuff(buffID, target, parentTransform, buffArray, display);
                break;
        }
    }

    private Transform GetBuffParent(GameObject target)
    {

        return target == PlayerManager.Instance.GetPlayer().gameObject ? buffParent : target.GetComponent<ISufferBuff>().GetBuffParent();
    }
    /// <summary>
    /// 处理叠加Buff的逻辑
    /// </summary>
    /// <param name="buffID"></param>
    /// <param name="target"></param>
    /// <param name="parentTransform"></param>
    /// <param name="buffArray"></param>
    /// <param name="display"></param>
    private void HandleOverlapBuff(int buffID, GameObject target, Transform parentTransform, List<int> buffArray, bool display)
    {
        buff = Instantiate(display ? buffPrefab_display : buffPrefab, parentTransform);

        if (buffArray.Count >= BUFFCOUNTMAX)
        {
            buffArray.RemoveAt(0);
        }

        buffArray.Add(buffID);
        InstantiateAndStartBuff(buffID, target, display);
    }
    /// <summary>
    /// 处理刷新Buff的逻辑
    /// </summary>
    /// <param name="buffID"></param>
    /// <param name="target"></param>
    /// <param name="parentTransform"></param>
    /// <param name="buffArray"></param>
    /// <param name="display"></param>
    private void HandleRefreshBuff(int buffID, GameObject target, Transform parentTransform, List<int> buffArray, bool display)
    {

        if (buffArray.Contains(buffID))
        {
            ISufferBuff icanBuff = target.GetComponent<ISufferBuff>();
            if (buffArray.IndexOf(buffID) < icanBuff.GetBuffParent().childCount)
                icanBuff.GetBuffParent().GetChild(buffArray.IndexOf(buffID)).GetComponentInChildren<BaseBuff>().Refresh();
        }
        else
        {
            buff = Instantiate(display ? buffPrefab_display : buffPrefab, parentTransform);
            if (buffArray.Count >= BUFFCOUNTMAX)
            {
                buffArray.RemoveAt(0);
            }

            buffArray.Add(buffID);
            InstantiateAndStartBuff(buffID, target, display);
        }
    }
    /// <summary>
    /// 处理延时Buff的逻辑
    /// </summary>
    /// <param name="buffID"></param>
    /// <param name="target"></param>
    /// <param name="parentTransform"></param>
    /// <param name="buffArray"></param>
    /// <param name="display"></param>
    private void HandleExtendedBuff(int buffID, GameObject target, Transform parentTransform, List<int> buffArray, bool display)
    {
        if (buffArray.Contains(buffID))
        {
            ISufferBuff icanBuff = target.GetComponent<ISufferBuff>();
            if (buffArray.IndexOf(buffID) < icanBuff.GetBuffParent().childCount)
                icanBuff.GetBuffParent().GetChild(buffArray.IndexOf(buffID)).GetComponentInChildren<BaseBuff>().ExtendBuff(1);
        }
        else
        {
            buff = Instantiate(display ? buffPrefab_display : buffPrefab, parentTransform);
            if (buffArray.Count >= BUFFCOUNTMAX)
            {
                buffArray.RemoveAt(0);
            }

            buffArray.Add(buffID);
            InstantiateAndStartBuff(buffID, target, display);
        }
    }

    private void InstantiateAndStartBuff(int buffID, GameObject target, bool display)
    {
        GameObject baseBuff = Instantiate(buffListSO.buffList[buffID].buff, buff.transform);
        baseBuff.GetComponent<BaseBuff>().StartBuff(target, buffListSO.buffList[buffID], display, buffID);
    }
}
