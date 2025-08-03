using UnityEngine;

public class BaseBullet : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    [SerializeField] protected ItemSO itemSO;
    /// 子弹的最大生命时间
    protected float lifeTimerMax;
    protected ReactionsBuff reactionsBuff;
    protected float lifeTimer;
    // 子弹的速度
    protected float speed;
    protected float tempSpeed;
    //影响层级
    [SerializeField] protected LayerMask layerMask;
    // 子弹的半径
    [SerializeField] protected float radius;
    protected int damage;
    protected ReactionsBuff.DamageEnum damageEnum;
    protected int count;
    protected int hitBack;
    protected void OnEnable()
    {
        damage = itemSO.damage+PlayerManager.Instance.GetPlayer().OccupationData.RemoteAttackAddition;
        damageEnum = itemSO.damageEnum;
        count = itemSO.damageCount+ (int)PlayerManager.Instance.GetPlayer().OccupationData.ReactionsBuffCountAddition;
        hitBack = itemSO.hitBack;
        speed = itemSO.speed;
        lifeTimerMax = itemSO.lifeTimerMax;
        lifeTimer = lifeTimerMax;
        tempSpeed = speed;
        reactionsBuff = new ReactionsBuff(damageEnum, damage,count,hitBack);
    }
    public void SetBulletData(float damageMul, float speedMul, float lifeTimeMul, float hitbackMul)
    {
        this.reactionsBuff = new ReactionsBuff(damageEnum, (int)(damageMul * damage), (int)(damageMul * damage), hitBack * hitbackMul);
        this.speed = speed * speedMul;
        this.lifeTimer = lifeTimerMax * lifeTimeMul;
    }
    public void SetReactionsBuff(ReactionsBuff reactionsBuff)
    {
        this.reactionsBuff = reactionsBuff;
    }

    public void LifeTime()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0)
        {
            lifeTimer = lifeTimerMax;
            gameObject.SetActive(false);
        }
    }

    public void SetLifeTime(float lifeTime)
    {
        lifeTimerMax = lifeTime;
    }


}
