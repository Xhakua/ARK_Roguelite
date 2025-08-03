using System;
using UnityEngine;

public class BaseSword : MonoBehaviour, ICauseDamage, IAble2Left, IAble2Flip, IAble2Right
{
    [SerializeField] protected ItemSO itemSO;
    [SerializeField] protected Animator anim;
    //[SerializeField] protected AnimationClip slashClip;
    protected int damage;
    protected ReactionsBuff.DamageEnum damageEnum;
    protected int count;
    protected float swingSpeed = 1;
    protected int attackCount = 1;
    protected float timer = 0;
    protected int hitBack;

    protected bool canATK = true;
    protected ReactionsBuff reactionsBuff;

    protected bool isCharge = false;
    protected float chaegeStartTime;

    protected bool isWhack = false;

    protected void Start()
    {
        //GameInputManager.Instance. += OnInteractionReleased;
    }



    public void Flip(int dir)
    {
        if (dir > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (dir < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public float GetRateTimer()
    {
        return 1f / swingSpeed;
    }

    public void OnLeft()
    {
        if (isCharge)
        {
            return;
        }
        if (canATK)
        {
            anim.speed = swingSpeed * PlayerManager.Instance.GetPlayer().OccupationData.MeleeSpeedMultiplier;
            anim.SetTrigger("slash");
            canATK = false;
            AudioManager.Instance.PlayMeleeAttackSound(transform.position);
        }


    }
    protected void Update()
    {
        if (!canATK)
        {
            timer += Time.deltaTime * PlayerManager.Instance.GetPlayer().OccupationData.MeleeSpeedMultiplier;
            if (timer >= 1f / swingSpeed)
            {
                canATK = true;
                timer = 0;
            }
        }

    }

    public void SetReactionsBuff(ReactionsBuff reactionsBuff)
    {
        this.reactionsBuff = reactionsBuff;
    }

    protected void OnEnable()
    {
        swingSpeed = itemSO.swingSpeed;
        damageEnum = itemSO.damageEnum;
    }
    protected ReactionsBuff ResetReactionsBuff()
    {
        damage = (int)((itemSO.damage + PlayerManager.Instance.GetPlayer().OccupationData.MeleeAttackAddition) * PlayerManager.Instance.GetPlayer().OccupationData.MeleeAttackMultiplier);
        count = itemSO.damageCount + PlayerManager.Instance.GetPlayer().OccupationData.ReactionsBuffCountAddition;
        attackCount = itemSO.attackCount;
        hitBack = itemSO.hitBack/* * (int)PlayerManager.Instance.GetPlayer().OccupationData.HitbackMultiplier*/;
        return new ReactionsBuff(damageEnum, damage, count, hitBack);
    }

    protected ReactionsBuff WhackReactionsBuff()
    {
        float mul=Mathf.Clamp((Time.time - chaegeStartTime) / itemSO.whackTimerMax, 0, 1);

        float whackMul = itemSO.whackMul*mul;

        damage = (int)((itemSO.damage + PlayerManager.Instance.GetPlayer().OccupationData.MeleeAttackAddition) * PlayerManager.Instance.GetPlayer().OccupationData.MeleeAttackMultiplier * whackMul);
        count = itemSO.damageCount + (int)PlayerManager.Instance.GetPlayer().OccupationData.ReactionsBuffCountAddition;
        attackCount = itemSO.attackCount/* + (int)PlayerManager.Instance.GetPlayer().OccupationData.MeleeAttackCount*/;
        hitBack = (int)(itemSO.hitBack /** PlayerManager.Instance.GetPlayer().OccupationData.HitbackMultiplier */* whackMul);
        return new ReactionsBuff(damageEnum, damage, count, hitBack);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("damage"))
        {
            for (int i = 0; i < attackCount; i++)
            {
                collision.gameObject.TryGetComponent<IHurt>(out IHurt damage);
                if (damage != null)
                {

                        reactionsBuff = ResetReactionsBuff();
 
                    damage.Hurt(reactionsBuff, gameObject);
                    //UniversalEffectsManager.Instance.GenerateSlash(collision.transform);
                }

            }
            isWhack = false;
        }
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("whack") && isWhack)
        {
            for (int i = 0; i < attackCount; i++)
            {
                collision.gameObject.TryGetComponent<IHurt>(out IHurt damage);
                if (damage != null)
                {
                    reactionsBuff = WhackReactionsBuff();
                    damage.Hurt(reactionsBuff, gameObject);
                    //UniversalEffectsManager.Instance.GenerateSlash(collision.transform);
                }

            }
            isWhack = false;
        }
    }

    public void OnRight()
    {
        if (!isCharge)
        {
            isCharge = true;
            anim.SetBool("Charge", true);
            chaegeStartTime = Time.time;
        }

    }


    protected void OnInteractionReleased(object sender, EventArgs e)
    {
        if (!isCharge)
        {
            return;
        }
        anim.SetBool("Charge", false);
        isWhack = true;
        isCharge = false;
    }

    protected void OnDestroy()
    {
       // GameInputManager.Instance.OnRightUp -= OnInteractionReleased;
    }
}
