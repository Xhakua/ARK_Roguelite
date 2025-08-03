using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 生命周期组件
/// 给目标物体设置生命周期，生命周期结束后自动禁用目标物体
/// </summary>
public class LifeTimer : MonoBehaviour,ISetLifeTime
{
    [SerializeField] private float lifeTimerMax;
    private float lifeTimer;
    public void SetLifeTime(float lifeTime)
    {
        lifeTimerMax = lifeTime;
    }

    public void LifeTime()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTimerMax)
        {
            lifeTimer = 0;
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        lifeTimer = 0;
    }
    private void Update()
    {
        LifeTime();

    }
}
