using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �����������
/// ��Ŀ�����������������ڣ��������ڽ������Զ�����Ŀ������
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
