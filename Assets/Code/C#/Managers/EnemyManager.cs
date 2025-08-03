using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public static List<GameObject> enemylist = new();
    public static int enemyCount = 0;
    //最大敌人数量
    public int maxEnemyCount = 10;
    //一次生成敌人数量
    public int enemyGroupCount = 3;
    //敌人刷新时间
    public float respawnTime = 5;
    public Vector2Int respawnSizeMax;
    public Vector2Int respawnSizeMin;

    public class GameObjectEventArgs : System.EventArgs
    {
        public GameObject gameObject;
    }

    public event System.EventHandler<GameObjectEventArgs> OnEnemyDead;

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
    public GameObject GenerateEnemy(GameObject enemy, Vector3 postion)
    {
        bool takeenemy = false;
        GameObject ret = null;
        for (int i = 0; i < enemylist.Count; i++)
        {
            if (!enemylist[i].gameObject.activeSelf)
            {
                if ((enemylist[i].name) == enemy.name + "(Clone)")
                {
                    enemylist[i].transform.position = postion;
                    enemylist[i].SetActive(true);
                    takeenemy = true;
                    ret = enemylist[i];
                    enemyCount++;
                    break;
                }
            }
        }
        if (!takeenemy)
        {
            ret = Instantiate(enemy, postion, Quaternion.identity);
            enemylist.Add(ret);
            enemyCount++;
        }
        return ret;
    }

    public IEnumerator GenerateEnemyIEnumerator(GameObject enemy, Vector3 postion)
    {
        bool takeenemy = false;
        GameObject ret = null;
        for (int i = 0; i < enemylist.Count; i++)
        {
            if (!enemylist[i].gameObject.activeSelf)
            {
                if ((enemylist[i].name) == enemy.name + "(Clone)")
                {
                    enemylist[i].transform.position = postion;
                    enemylist[i].SetActive(true);
                    takeenemy = true;
                    ret = enemylist[i];
                    enemyCount++;
                    break;
                }
            }
        }
        if (!takeenemy)
        {
            ret = Instantiate(enemy, postion, Quaternion.identity);
            enemylist.Add(ret);
            enemyCount++;
        }
        yield return null;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        OnEnemyDead?.Invoke(this, new GameObjectEventArgs { gameObject = enemy });
        enemy.SetActive(false);
        enemyCount--;
    }

    private void Start()
    {
        //TODO:怪物生成时间调整
        StartCoroutine(RespawnEnemy());
    }

    public IEnumerator RespawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);
            //Debug.Log(enemyCount);
            if (enemyCount < maxEnemyCount)
            {
                for (int n = 0; n < enemyGroupCount; n++)
                {
                    yield return null;
                    int xr = UnityEngine.Random.Range(-100, 100);
                    int yr = UnityEngine.Random.Range(-100, 100);
                    xr = xr > 0 ? 1 : -1;
                    yr = yr > 0 ? 1 : -1;
                    Vector2Int playerPos = new Vector2Int((int)PlayerManager.Instance.GetPlayer().transform.position.x, (int)(int)PlayerManager.Instance.GetPlayer().transform.position.z);
                    Vector2Int respawnPos = new Vector2Int(
                       xr * UnityEngine.Random.Range(respawnSizeMin.x, respawnSizeMax.x),
                       yr * UnityEngine.Random.Range(respawnSizeMin.y, respawnSizeMax.y)
                    );
                    respawnPos += playerPos;
                    respawnPos.x = Mathf.Clamp(respawnPos.x, 0, MapManager.Instance.GetWorldSize().x - 1);
                    respawnPos.y = Mathf.Clamp(respawnPos.y, 0, MapManager.Instance.GetWorldSize().z - 1);
                    //Debug.Log("RespawnPos" + respawnPos);
                    if (CircleTilesIsNull(respawnPos, MapManager.TileLayer.Wall, 0))
                    {
                        PerlinMapSO perlinMapSO = MapManager.Instance.GetMapChunk();
                        if (perlinMapSO != null)
                        {
                            if (perlinMapSO.enemies.Length > 0)
                            {
                                for (int i = 0; i < perlinMapSO.enemies.Length; i++)
                                {
                                    if (UnityEngine.Random.Range(0, 100) < perlinMapSO.enemies[i].respawnRate
                                        )
                                    {
                                        //Debug.Log("Respawn");
                                        //Debug.Log(respawnPos);
                                        GenerateEnemy(perlinMapSO.enemies[i].enemy, new Vector3(respawnPos.x, 1.6f, respawnPos.y));
                                        yield return null;
                                    }
                                }
                            }
                        }

                    }
                }
            }

        }
    }

    /// <summary>
    /// 检查指定位置的圆形范围内的方块是否为空
    /// 避免敌人生成在有方块的地方
    /// </summary>
    /// <param name="centerPos"></param>
    /// <param name="layer"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public bool CircleTilesIsNull(Vector2Int centerPos, MapManager.TileLayer layer, int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {

                    if (MapManager.Instance.GetWorldData(layer)[centerPos.x + x, centerPos.y + y] != 0)
                    {
                        //Debug.Log("CircleTilesIsNull");
                        return false;
                    }

                }
            }
        }
        return true;
    }


    private void OnDestroy()
    {
        enemylist.Clear();
    }
}







