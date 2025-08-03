using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour, ICanMapInteraction, IAble2Left, IHandheldInEffect
{
    public CubeSO cubeSO;
    /// �ƻ�����
    protected int destroyCount;
    protected bool isLeft = false;
    protected bool isLeftUp = false;
    protected Vector3Int startPos;
    protected Vector3Int endPos;
    protected GameObject visusalCube;
    protected bool isSet = false;
    //�Ӿ��������ű���
    [SerializeField] protected Vector3 visusalCubeScale = new Vector3(0.75f, 0.75f, 0.75f);


    //���ֳ�״̬�£����λ�õ��Ӿ�Ч��
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


    // ����������ʱ����
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

    // ���¾��ο���ĸ�����
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
        selectionRect.SetPosition(4, p1); // �պ�
    }


    // ������̧��ʱ����
    protected void Instance_OnMouseUp(object sender, System.EventArgs e)
    {
        //Vector3Int mousePos = PlayerManager.Instance.GetPlayer().MousePos;
        //endPos = mousePos;

        //���ɷ���
        List<GameObject> gameObjects = MapDynamicLoadingManager.Instance.GenerateCubeBlock(startPos, endPos, cubeSO);
        //���������ݣ���ֹ����ռλ�޷����ɷ���
        MapManager.Instance.SetWorldDataBlock(startPos, endPos, MapManager.TileLayer.Wall, cubeSO.id);
        Destroy(visusalCube);
        visusalCube = null;
        // ���ؾ��ο�
        if (selectionRect != null)
        {
            selectionRect.gameObject.SetActive(false);
        }
        isLeft = false;
        isLeftUp = false;
        //���÷���״̬
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

    //��ͼ�����ӿ�ʵ��
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
