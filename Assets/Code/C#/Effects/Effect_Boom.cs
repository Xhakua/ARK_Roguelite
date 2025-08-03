using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Boom : MonoBehaviour,ISetLifeTime,ICauseDamage
{
    [SerializeField] private float lifeTimerMax;
    [SerializeField] private float radius;
    //影响层级
    [SerializeField] private LayerMask layerMask;
    private float lifeTimer;
    //触发间隔
    [SerializeField] private float dotTimerMax;
    private float dotTimer;
    private ReactionsBuff reactionsBuff;
    private void OnEnable()
    {
        dotTimer = 0;
        lifeTimer = 0;
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

    public void SetReactionsBuff(ReactionsBuff reactionsBuff)
    {
        this.reactionsBuff = reactionsBuff;
    }

    public void SetLifeTime(float lifeTime)
    {
        lifeTimerMax = lifeTime;
    }
    private void Update()
    {
        LifeTime();
        if (dotTimer > dotTimerMax)
        {
            Hit();
        }
        else
        {
            dotTimer += Time.deltaTime;
        }
    }
    private void Hit()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D collider in colliders)
        {

            if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
                //dotTimer = 0;
            }
            //gameObject.SetActive(false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}

