using UnityEngine;

public class Effect_BW : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    [SerializeField] private float lifeTimerMax;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private SpriteRenderer sprite;
    private float lifeTimer;
    private ReactionsBuff reactionsBuff;
    private void OnEnable()
    {
        lifeTimer = 0;
    }
    public void LifeTime()
    {
        lifeTimer += Time.deltaTime;
        sprite.material.SetFloat("_threshold", 1 - lifeTimer / lifeTimerMax);
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
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
