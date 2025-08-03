using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 子弹管理器
/// </summary>
public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }

    public static List<GameObject> bulletlist = new List<GameObject>();



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
    /// <summary>
    /// 生成子弹
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="transform"></param>
    /// <param name="itemSO"></param>
    /// <param name="hasCost"></param>
    /// <returns></returns>
    public GameObject GenerateBullet(GameObject bullet, Transform transform, ItemSO itemSO = null, bool hasCost = false)
    {
        bool takebullet = false;
        GameObject ret = null;
        if (itemSO != null && hasCost && !PlayerManager.Instance.GetPlayer().ChangeResourceAmount(-itemSO.complexity))
        {
            return null;
        }
        for (int i = 0; i < bulletlist.Count; i++)
        {
            if (!bulletlist[i].activeSelf)
            {

                if ((bulletlist[i].name) == bullet.name + "(Clone)")
                {

                    bulletlist[i].transform.position = transform.position;
                    bulletlist[i].transform.rotation = transform.rotation;

                    bulletlist[i].gameObject.SetActive(true);

                    takebullet = true;
                    ret = bulletlist[i];
                    break;
                }
            }
        }
        if (!takebullet)
        {

            ret = Instantiate(bullet, transform.position, transform.rotation);

            bulletlist.Add(ret);
        }
        return ret;
    }
    /// <summary>
    /// 生成子弹
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="itemSO"></param>
    /// <param name="hasCost"></param>
    /// <returns></returns>
    public GameObject GenerateBullet(Transform transform, ItemSO itemSO, bool hasCost = true)
    {
        bool takebullet = false;
        GameObject ret = null;
        if (itemSO.itemType == ItemManager.ItemEnum.Ammo && hasCost && !PlayerManager.Instance.GetPlayer().ChangeResourceAmount(-itemSO.complexity))
        {
            return null;
        }
        for (int i = 0; i < bulletlist.Count; i++)
        {
            if (!bulletlist[i].activeSelf)
            {

                if ((bulletlist[i].name) == itemSO.itemPrefab.name + "(Clone)")
                {

                    bulletlist[i].transform.position = transform.position;
                    bulletlist[i].transform.rotation = transform.rotation;

                    bulletlist[i].gameObject.SetActive(true);

                    takebullet = true;
                    ret = bulletlist[i];
                    break;
                }
            }
        }
        if (!takebullet)
        {

            ret = Instantiate(itemSO.itemPrefab, transform.position, transform.rotation);

            bulletlist.Add(ret);
        }
        return ret;
    }
    /// <summary>
    /// 生成子弹
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="transform"></param>
    /// <param name="quaternion"></param>
    /// <param name="itemSO"></param>
    /// <param name="hasCost"></param>
    /// <returns></returns>
    public GameObject GenerateBullet(GameObject bullet, Transform transform, Quaternion quaternion, ItemSO itemSO = null, bool hasCost = true)
    {
        bool takebullet = false;
        GameObject ret = null;
        if (itemSO.itemType == ItemManager.ItemEnum.Ammo && hasCost && !PlayerManager.Instance.GetPlayer().ChangeResourceAmount(-itemSO.complexity))
        {
            return null;
        }
        for (int i = 0; i < bulletlist.Count; i++)
        {
            if (!bulletlist[i].gameObject.activeSelf)
            {

                if ((bulletlist[i].name) == bullet.name + "(Clone)")
                {

                    bulletlist[i].transform.position = transform.position;
                    bulletlist[i].transform.rotation = transform.rotation * quaternion;

                    bulletlist[i].gameObject.SetActive(true);

                    takebullet = true;
                    ret = bulletlist[i];
                    break;
                }
            }
        }
        if (!takebullet)
        {

            ret = Instantiate(bullet, transform.position, transform.rotation * quaternion);

            bulletlist.Add(ret);
        }
        return ret;
    }

    /// <summary>
    /// 生成子弹
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="transform"></param>
    /// <param name="reactionsBuff"></param>
    /// <param name="itemSO"></param>
    /// <param name="hasCost"></param>
    /// <returns></returns>
    public GameObject GenerateBullet(GameObject bullet, Transform transform, ReactionsBuff reactionsBuff, ItemSO itemSO = null, bool hasCost = true)
    {
        bool takebullet = false;
        GameObject ret = null;
        if (itemSO.itemType == ItemManager.ItemEnum.Ammo && hasCost && !PlayerManager.Instance.GetPlayer().ChangeResourceAmount(-itemSO.complexity))
        {
            return null;
        }
        for (int i = 0; i < bulletlist.Count; i++)
        {
            if (!bulletlist[i].gameObject.activeSelf)
            {

                if ((bulletlist[i].name) == bullet.name + "(Clone)")
                {

                    bulletlist[i].transform.position = transform.position;
                    bulletlist[i].transform.rotation = transform.rotation;
                    bulletlist[i].GetComponent<BaseBullet>().SetReactionsBuff(reactionsBuff);
                    bulletlist[i].gameObject.SetActive(true);

                    takebullet = true;
                    ret = bulletlist[i];
                    break;
                }
            }
        }
        if (!takebullet)
        {
            bullet.GetComponent<BaseBullet>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(bullet, transform.position, transform.rotation);

            bulletlist.Add(ret);
        }
        return ret;
    }

    private void OnDestroy()
    {
        bulletlist.Clear();
    }




}
