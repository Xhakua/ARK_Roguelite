
using UnityEngine;

public class BoomGene : GeneBase
{
    [SerializeField] protected ItemSO itemSO;
    // 子弹射击速率
    protected float fireRateTimerMax;
    protected float fireRateTimer = 100;
    // 爆炸碰撞体
    [SerializeField] protected GameObject BoomPoint;
    // 爆炸特效
    [SerializeField] protected ParticleSystem Effect_Boom;
    protected int BulletListNum = 0;
    private SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        fireRateTimerMax = itemSO.fireRateTimerMax;
    }
    public float GetRateTimer()
    {
        fireRateTimer = fireRateTimerMax + 1;
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

    //爆炸效果，BoomPoint为搭载Effect_Boom的伤害检测物体
    public override void StartEffect()
    {
        BoomPoint.SetActive(true);
        Effect_Boom.Play();
        fireRateTimer = 0;
    }

    public override void StopEffect()
    {
        throw new System.NotImplementedException();
    }
}