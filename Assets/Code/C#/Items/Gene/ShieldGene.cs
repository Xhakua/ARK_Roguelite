
using System.Collections;
using UnityEngine;

public class ShieldGene : GeneBase
{
    [SerializeField] protected ItemSO itemSO;
    // 子弹射击速率
    protected float fireRateTimerMax;
    protected float fireRateTimer = 100;
    //护盾存在时间
    protected float lifeTimeMul;
    // 射击点
    [SerializeField] protected Transform firePoint;
    // 护盾特效
    [SerializeField] protected ParticleSystem Effect_Shield;
    protected int BulletListNum = 0;
    private SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        fireRateTimerMax = itemSO.fireRateTimerMax;
        lifeTimeMul = itemSO.lifeTimeMul;
    }
    public float GetRateTimer()
    {
        fireRateTimer = fireRateTimerMax + 1; // 加一让他可以立即攻击
        return fireRateTimerMax;
    }
    public override void Effect()
    {
        if (fireRateTimer < fireRateTimerMax)
        {
            fireRateTimer += Time.deltaTime * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier;
        }
        if (fireRateTimer >= fireRateTimerMax)
        {
            StartEffect();
        }
    }

    public override void StartEffect()
    {
        StartCoroutine(MyCoroutine());
    }

    //护盾展开协程
    //TODO：护盾暂时没有实际作用
    IEnumerator MyCoroutine()
    {
        Effect_Shield.Play();
        fireRateTimer = 0;
        yield return new WaitForSeconds(lifeTimeMul);
        Effect_Shield.Clear();
    }

    public override void StopEffect()
    {
        throw new System.NotImplementedException();
    }
}