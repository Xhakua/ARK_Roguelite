using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour, ICanMapInteraction, IAble2Left, IHandheldInEffect
{
    public CubeSO cubeSO;
    /// 破坏计数
    protected int destroyCount;
    protected bool isLeft = false;
    protected bool isLeftUp = false;
    protected Vector3Int startPos;
    protected Vector3Int endPos;
    protected GameObject visusalCube;
    protected bool isSet = false;
    //视觉方块缩放比例
    [SerializeField] protected Vector3 visusalCubeScale = new Vector3(0.75f, 0.75f, 0.75f);


    //在手持状态下，鼠标位置的视觉效果
    public void HandheldInEffect()
    {
        if (isLeft)
        {
            return;
        }
        if (visusalCube == null)
        {
            Vector3Int mousePos = PlayerManager.Instance.GetPlayer().MousePos;

            visusalCube = Instantiate(cubeSO.cube, mousePos, Quaternion.identity);
            visusalCube.GetComponent<Collider>().enabled = false;
            visusalCube.transform.localScale = visusalCubeScale;

        }
        else
        {

            visusalCube.transform.position = PlayerManager.Instance.GetPlayer().MousePos;
        }
    }

    protected LineRenderer selectionRect;


    // 鼠标左键按下时触发
    public void OnLeft()
    {
        if (!isLeft)
        {
            isLeft = true;
        }
        Vector3Int mousePos = PlayerManager.Instance.GetPlayer().MousePos;
        if (!isLeftUp)
        {
            GameInputManager.Instance.OnMouseUp += Instance_OnMouseUp;
            isLeftUp = true;
            startPos = mousePos;


            if (selectionRect == null)
            {
                GameObject rectObj = new GameObject("SelectionRect");
                selectionRect = rectObj.AddComponent<LineRenderer>();
                selectionRect.positionCount = 5;
                selectionRect.loop = false;
                selectionRect.widthMultiplier = 0.1f;
                selectionRect.material = new Material(Shader.Find("Sprites/Default"));
                selectionRect.startColor = Color.green;
                selectionRect.endColor = Color.green;
            }
            selectionRect.gameObject.SetActive(true);
        }

        endPos = PlayerManager.Instance.GetPlayer().MousePos;
        UpdateSelectionRect(startPos, endPos);
    }

    // 更新矩形框的四个顶点
    protected void UpdateSelectionRect(Vector3Int start, Vector3Int end)
    {
        if (selectionRect == null) return;
        Vector3 p1 = new Vector3(start.x, start.y, start.z);
        Vector3 p2 = new Vector3(end.x, start.y, start.z);
        Vector3 p3 = new Vector3(end.x, end.y, end.z);
        Vector3 p4 = new Vector3(start.x, end.y, end.z);

        selectionRect.SetPosition(0, p1);
        selectionRect.SetPosition(1, p2);
        selectionRect.SetPosition(2, p3);
        selectionRect.SetPosition(3, p4);
        selectionRect.SetPosition(4, p1); // 闭合
    }


    // 鼠标左键抬起时触发
    protected void Instance_OnMouseUp(object sender, System.EventArgs e)
    {
        //Vector3Int mousePos = PlayerManager.Instance.GetPlayer().MousePos;
        //endPos = mousePos;

        //生成方块
        List<GameObject> gameObjects = MapDynamicLoadingManager.Instance.GenerateCubeBlock(startPos, endPos, cubeSO);
        //后设置数据，防止数据占位无法生成方块
        MapManager.Instance.SetWorldDataBlock(startPos, endPos, MapManager.TileLayer.Wall, cubeSO.id);
        Destroy(visusalCube);
        visusalCube = null;
        // 隐藏矩形框
        if (selectionRect != null)
        {
            selectionRect.gameObject.SetActive(false);
        }
        isLeft = false;
        isLeftUp = false;
        //设置放置状态
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.GetComponent<BaseCube>().PlacedTriggered();
                Debug.Log("PlacedTriggered: " + obj.name);
            }
        }
    }

    public virtual void PlacedTriggered()
    {
        isSet = true;
        Debug.Log("BaseCube PlacedTriggered: " + gameObject.name);
    }

    //地图交互接口实现
    public void OnMapInteraction()
    {
        destroyCount += 1;
        Debug.Log("destroyCount" + destroyCount);
        if (destroyCount >= cubeSO.destroyCount)
        {
            gameObject.SetActive(false);
            destroyCount = 0;
            Vector3Int pos = new Vector3Int((int)transform.position.x, (int)transform.position.z, 0);
            MapManager.Instance.SetWorldData(pos, MapManager.TileLayer.Wall, 0);
        }
    }

    public void SetDestroyCount(int count)
    {
        destroyCount += count;
        if (destroyCount >= cubeSO.destroyCount)
        {
            gameObject.SetActive(false);
            destroyCount = 0;
            Vector3Int pos = new Vector3Int((int)transform.position.x, (int)transform.position.z, 0);
            MapManager.Instance.SetWorldData(pos, MapManager.TileLayer.Wall, 0);
        }
    }
    protected void OnDestroy()
    {
        if (visusalCube != null)
        {
            Destroy(visusalCube);
            visusalCube = null;
        }
        if (selectionRect != null)
        {
            Destroy(selectionRect.gameObject);
            selectionRect = null;
        }
    }
}
