using UnityEngine;

public class Effect_Electric : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    [SerializeField] private float lifeTimerMax;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float dotTimerMax;
    private float dotTimer;

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
    public void SetLifeTime(float lifeTime)
    {
        lifeTimerMax = lifeTime;
    }
    public void SetReactionsBuff(ReactionsBuff reactionsBuff)
    {
        this.reactionsBuff = reactionsBuff;
        //Debug.Log("=========================");
        //Debug.Log("SetReactionsBuff");
        //Debug.Log(reactionsBuff);
        //Debug.Log(this.reactionsBuff);
        //Debug.Log("=========================");

    }
    private void FixedUpdate()
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

    //private void Update()
    //{
    //    LifeTimer();
    //    Hit();
    //}
    private void Hit()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D collider in colliders)
        {

            if (collider.CompareTag("Enemy"))
            {
                //Debug.Log("=========================");
                //Debug.Log(this.reactionsBuff);
                //Debug.Log(reactionsBuff);
                //Debug.Log("=========================");
                collider.GetComponent<IHurt>().Hurt(this.reactionsBuff,gameObject);
                dotTimer = 0;
            }

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
