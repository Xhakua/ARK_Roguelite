
using UnityEngine;

public class GunGene : GeneBase
{
    [SerializeField] protected ItemSO itemSO;
    //后坐力最大角度
    protected float recoilMax;
    //后坐力最小角度
    protected float recoilMin;
    //后坐力数值
    protected float recoilforce;
    //子弹伤害倍率
    protected float damageMul;
    //子弹速度倍率
    protected float speedMul;
    // 击退倍率
    protected float hitbackMul;
    // 子弹生命周期倍率
    protected float lifeTimeMul;
    [SerializeField] protected InventorySO bulletList;
    // 子弹射击速率
    protected float fireRateTimerMax;
    protected float fireRateTimer = 100;
    // 射击点
    [SerializeField] protected Transform firePoint;
    // 子弹抛壳特效
    [SerializeField] protected ParticleSystem Effect_ThrowingShells;
    protected int BulletListNum = 0;
    private SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        recoilforce = itemSO.recoilforce;
        recoilMax = itemSO.recoilMax;
        recoilMin = itemSO.recoilMin;
        damageMul = itemSO.damageMul;
        speedMul = itemSO.speedMul;
        hitbackMul = itemSO.hitbackMul;
        lifeTimeMul = itemSO.lifeTimeMul;
        fireRateTimerMax = itemSO.fireRateTimerMax;
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
        if (fireRateTimer < fireRateTimerMax)
        {
            return;
        }

        if (bulletList != null)
        {
            // 查找非空子弹
            int startBulletListNum = BulletListNum;
            while (bulletList.IsNull(BulletListNum))
            {
                BulletListNum++;
                if (BulletListNum >= bulletList.items.Count)
                {
                    BulletListNum = 0;
                }
                if (BulletListNum == startBulletListNum)
                {
                    return;
                }
            }
            // 发射子弹
            transform.parent.GetComponent<AutoAim>().SetRecoil(recoilMin, recoilMax);
            firePoint.right = new Vector3(transform.right.x, 0, transform.right.y);
            GameObject temp = BulletManager.Instance.GenerateBullet(firePoint, bulletList.GetItem(BulletListNum));
            if (temp == null)
            {
                return;
            }
            temp.GetComponent<BaseBullet>().SetBulletData(damageMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteAttackAddition, speedMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier, lifeTimeMul, hitbackMul);
            AudioManager.Instance.PlayRangedAttackSound(transform.position);
            Effect_ThrowingShells.Play();

            BulletListNum++;
            fireRateTimer = 0;
            if (BulletListNum >= bulletList.items.Count)
            {
                BulletListNum = 0;
            }
        }
    }

    public override void StopEffect()
    {
        throw new System.NotImplementedException();
    }
}