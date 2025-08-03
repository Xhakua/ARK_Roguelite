using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Broken : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    [SerializeField] private float lifeTimerMax;
    [SerializeField] private float radius;
    //Ó°Ïì²ã¼¶
    [SerializeField] private LayerMask layerMask;
    private float lifeTimer;
    private ReactionsBuff reactionsBuff;
    private void OnEnable()
    {
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
        Hit();
    }
    private void Hit()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D collider in colliders)
        {

            if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
            }
            gameObject.SetActive(false);
        }
    }

}
