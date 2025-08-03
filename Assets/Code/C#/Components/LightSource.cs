using System.Collections;
using UnityEngine;

/// <summary>
/// 光源组件
/// 给目标物体添加光源效果
/// </summary>
public class LightSource : MonoBehaviour
{
    // 光源强度
    [SerializeField] private int lightIntensity;
    // 光源持续时间
    [SerializeField] private float lightTimerMax;
    // 光源暗淡时间
    [SerializeField] private float darkTimerMax;
    private int currentLightIntensity;
    private float lightTimer;
    private float darkTimer;
    private bool isLight = true;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private void OnEnable()
    {
        lightTimer = lightTimerMax;
        darkTimer = darkTimerMax;
        currentPos = new Vector2(transform.position.x, transform.position.z);
        currentLightIntensity = lightIntensity;
    }
    private void FixedUpdate()
    {
        if (lightIntensity == 0 || MapManager.Instance == null)
        {
            return;
        }
        float tempX = Mathf.Clamp(transform.position.x, 0, MapManager.Instance.GetWorldSize().x - 2);//为什么要减2？int转换有损失
        float tempY = Mathf.Clamp(transform.position.z, 0, MapManager.Instance.GetWorldSize().z - 2);
        if (isLight)
        {
            if (currentLightIntensity <= lightIntensity)
            {
                currentLightIntensity++;
            }
            targetPos = new Vector2(tempX, tempY);
            if (MapManager.Instance.GetWorldData(new Vector3Int((int)currentPos.x, (int)currentPos.y), MapManager.TileLayer.Light) <= 0)
            {
                MapManager.Instance.SetWorldData(new Vector3Int((int)currentPos.x, (int)currentPos.y, 3), MapManager.TileLayer.Light, currentLightIntensity);
            }
            if (Vector2.Distance(targetPos, currentPos) > 1)
            {
                MapManager.Instance.ExchangeLightData(currentLightIntensity, new Vector3Int((int)currentPos.x, (int)currentPos.y, 2), new Vector3Int((int)targetPos.x, (int)targetPos.y, 2));
                currentPos = targetPos;
            }
            if(darkTimerMax > 0)
            {
                lightTimer -= Time.deltaTime;
            }
            if (lightTimer <= 0)
            {
                isLight = false;
                darkTimer = darkTimerMax;
            }

        }
        else if (darkTimerMax > 0)
        {
            currentLightIntensity /= 2;
            darkTimer -= Time.deltaTime;
            if (darkTimer <= 0)
            {
                isLight = true;
                lightTimer = lightTimerMax;
            }
        }
    }
    private void OnDisable()
    {
        if (MapManager.Instance == null)
        {
            return;
        }
        MapManager.Instance.SetWorldData(new Vector3Int((int)currentPos.x, (int)currentPos.y, 3), MapManager.TileLayer.Light, -1);
    }

    public IEnumerator Exchange(int lightIntensity, Vector3Int currentPos, Vector3Int targetPos)
    {
        MapManager.Instance.SetWorldData(targetPos, MapManager.TileLayer.Light, lightIntensity);
        Vector3Int temp = currentPos;
        yield return new WaitForSeconds(0.1f);
        MapManager.Instance.SetWorldData(temp, MapManager.TileLayer.Light, -lightIntensity);

    }
}
