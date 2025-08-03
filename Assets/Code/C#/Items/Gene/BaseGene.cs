using System;
using System.Threading;
using UnityEngine;

public class BaseGene : MonoBehaviour, IGeneEffect
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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        ApplyItemModifiers();
        //TODO:方法一，通过PlayerManager.Instance.GetPlayer()直接订阅，不确定两种方法那种正确
        switch (itemSO.geneType)
        {
            case ItemSO.GeneTypes.TriggerableOnTime:
                //时间触发式
                NeedRateTimeSet = true;
                activeRateTimer = activeRateTimerMax + 1;
                break;
            case ItemSO.GeneTypes.TriggerableOnTakeDamage:
                //伤害触发式
                PlayerManager.Instance.GetPlayer().OnTakeDamage += ActiveEventOnTakeDamage;
                break;
                //受伤触发式
            case ItemSO.GeneTypes.TriggerableOnHurt:
                PlayerManager.Instance.GetPlayer().OnHurt += ActiveEventOnHurt;
                break;
                //死亡触发式
            case ItemSO.GeneTypes.TriggerableOnDeath:
                PlayerManager.Instance.GetPlayer().OnDeath += ActiveEventOnDeath;
                break;
            default:
                break;
        }
    }

    public void Update()
    {
        //时间触发式
        if (NeedRateTimeSet)
        {
            if (activeRateTimer < activeRateTimerMax)
            {
                activeRateTimer += Time.deltaTime
                    * PlayerManager.Instance.GetPlayer().OccupationData.remoteSpeedMultiplier           //人物自身攻速倍率
                    * PlayerManager.Instance.GetPlayer().GeneAdditionalValueData.remoteSpeedMultiplier; //额外攻速倍率
            }
            else
            {
                ActiveEvent();
            }
        }
    }

    private void FixedUpdate()
    {
        if (needTarget)
            SeekTarget();
    }

    //数据初始化
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
    public void Effect()
    {
        if (fireRateTimer < fireRateTimerMax)
    {
            fireRateTimer += Time.deltaTime * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier;
    }
        if (fireRateTimer >= fireRateTimerMax)
    {
            StartEffect();
    }
    //死亡触发式订阅签名
    private void ActiveEventOnDeath(object sender, EventArgs e)
    {
        ActiveEvent();
    }
    #endregion

    public void StartEffect()
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
    #endregion

    public void StopEffect()
        {
        throw new System.NotImplementedException();
    }

}
