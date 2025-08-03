using System;
using System.Collections.Generic;
using UnityEngine;

public class GravityMagicGene : MonoBehaviour, IGeneEffect
{
    public ItemSO itemSO;
    protected LineRenderer selectionRect;
    protected Vector3Int startPos;
    protected Vector3Int endPos;
    protected List<GameObject> bullets = new List<GameObject>();
    protected List<GameObject> bulletShooting = new List<GameObject>();
    protected List<Vector2Int> bulletShootingPos = new List<Vector2Int>();
    [SerializeField] private float radiusMax = 5f;
    [SerializeField] private float radiusMin = 1f;
    [SerializeField] private float rotationSpeedMax = 10f;
    [SerializeField] private float rotationSpeedMin = 1f;
    [SerializeField] private float shootSpeed = 5f;
    public void Effect()
    {


        if (bullets != null && bullets.Count > 0)
        {
            foreach (var bullet in bullets)
            {
                if (bullet == null) continue;
                // ���㵱ǰ�븸�����ˮƽ����
                Vector3 center = this.transform.position;
                Vector3 bulletPos = bullet.transform.position;
                Vector3 offset = bulletPos - center;
                offset.y = 0; // ֻ��XZƽ����ת

                float distance = offset.magnitude;
                // ������ radiusMin ~ radiusMax ֮��
                if (distance > radiusMax)
                {
                    offset = offset.normalized * radiusMax;
                }
                else if (distance < radiusMin)
                {
                    offset = offset.normalized * radiusMin;
                }

                // ������ת�ٶ�
                float rotationSpeed = Mathf.Lerp(rotationSpeedMin, rotationSpeedMax, (distance - radiusMin) / (radiusMax - radiusMin));
                // Χ�Ƹ�����Y����ת
                offset = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * offset;

                // �����ӵ�λ��
                bullet.transform.position = center + offset;
                // ���ӵ���������
                bullet.transform.LookAt(center + Vector3.up * (bullet.transform.position.y - center.y));
            }

        }
        else
        {
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
            if (startPos != Vector3Int.zero)
            {
                selectionRect.gameObject.SetActive(true);
                endPos = PlayerManager.Instance.GetPlayer().MousePos;
                UpdateSelectionRect(startPos, endPos);
            }

        }

        // �����������Ƴ� bulletShooting �ķ�ʽ
        if (bulletShooting != null && bulletShooting.Count > 0)
        {
            for (int i = bulletShooting.Count - 1; i >= 0; i--)
            {
                var bullet = bulletShooting[i];
                if (bullet != null)
                {
                    int posIndex = i < bulletShootingPos.Count ? i : -1;
                    if (posIndex >= 0)
                    {
                        Vector2Int shootingPos = bulletShootingPos[posIndex];
                        Vector3 targetPos = new Vector3(shootingPos.x, bullet.transform.localPosition.y, shootingPos.y);
                        bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPos, shootSpeed * Time.deltaTime);
                        if (Vector3.Distance(bullet.transform.position, targetPos) < 0.1f)
                        {
                            MapManager.Instance.SetWorldData(new Vector3Int((int)bullet.transform.position.x, (int)bullet.transform.position.z, 0), MapManager.TileLayer.Wall, bullet.GetComponent<BaseCube>().cubeSO.id);
                            MapDynamicLoadingManager.Instance.GenerateCube(bullet.GetComponent<BaseCube>().cubeSO.cube, new Vector3Int((int)bullet.transform.position.x, 1, (int)bullet.transform.position.z));
                            bulletShooting.RemoveAt(i);
                            bulletShootingPos.RemoveAt(i);
                            Destroy(bullet);
                        }
                    }
                }
            }
        }
    }


    public void StartEffect()
    {
        GameInputManager.Instance.OnLeftDown += GameInputManager_OnLeftDown;
        GameInputManager.Instance.OnMouseUp += Instance_OnMouseUp;
        GameInputManager.Instance.OnRightDown += GameInputManager_OnRightDown;
    }

    private void GameInputManager_OnRightDown(object sender, EventArgs e)
    {
        startPos = Vector3Int.zero;
        //����һ���ӵ�
        if (bullets.Count > 0)
        {
            GameObject bullet = bullets[0];
            if (bullet != null)
            {
                Debug.Log("�����ӵ�");
                bullet.transform.SetParent(null);
                bulletShooting.Add(bullet);
                bullets.RemoveAt(0);
                bulletShootingPos.Add(new Vector2Int(PlayerManager.Instance.GetPlayer().MousePos.x, PlayerManager.Instance.GetPlayer().MousePos.z));
            }
        }

    }

    private void Instance_OnMouseUp(object sender, EventArgs e)
    {

        if (bullets.Count > 0 || startPos == Vector3Int.zero)
        {
            return;
        }
        selectionRect.gameObject.SetActive(false);
        MapManager.Instance.SetWorldDataBlock(startPos, endPos, MapManager.TileLayer.Wall, 0);
        bullets = MapDynamicLoadingManager.Instance.GetCubeBlock(new Vector2Int(startPos.x, startPos.z), new Vector2Int(endPos.x, endPos.z));
        foreach (GameObject bullet in bullets)
        {
            if (bullet != null)
            {
                bullet.transform.SetParent(this.transform);
                bullet.GetComponent<Collider>().enabled = false;
            }
        }
        startPos = Vector3Int.zero;
    }

    private void GameInputManager_OnLeftDown(object sender, EventArgs e)
    {
        startPos = PlayerManager.Instance.GetPlayer().MousePos;

    }

    public void StopEffect()
    {

        GameInputManager.Instance.OnLeftDown -= GameInputManager_OnLeftDown;
        GameInputManager.Instance.OnMouseUp -= Instance_OnMouseUp;
        GameInputManager.Instance.OnRightDown -= GameInputManager_OnRightDown;
        if (selectionRect != null)
        {
            Destroy(selectionRect.gameObject);
            selectionRect = null;
        }
    }
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

    void IGeneEffect.EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource)
    {
        throw new NotImplementedException();
    }

    void IGeneEffect.EffectOnHurt()
    {
        throw new NotImplementedException();
    }

    void IGeneEffect.EffectOnDeath()
    {
        throw new NotImplementedException();
    }
}
