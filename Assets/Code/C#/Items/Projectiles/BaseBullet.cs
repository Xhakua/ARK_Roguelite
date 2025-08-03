using UnityEngine;

public class BaseBullet : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    [SerializeField] protected ItemSO itemSO;
    /// �ӵ����������ʱ��
    protected float lifeTimerMax;
    protected ReactionsBuff reactionsBuff;
    protected float lifeTimer;
    // �ӵ����ٶ�
    protected float speed;
    protected float tempSpeed;
    //Ӱ��㼶
    [SerializeField] protected LayerMask layerMask;
    // �ӵ��İ뾶
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
