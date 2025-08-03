using UnityEngine;

public class OathOfJudah_LightSpear : BaseBullet
{
    private Vector3 target;
    private Vector3 oriPos;
    private Vector3 ctrlPos;
    bool isMoving = false;
    private float distance;
    private float t = 0;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer trailRenderer;
    private new void OnEnable()
    {
        trailRenderer.Clear();
        damage = itemSO.damage;
        damageEnum = itemSO.damageEnum;
        count = itemSO.damageCount;
        hitBack = itemSO.hitBack;
        speed = itemSO.speed;
        lifeTimerMax = itemSO.lifeTimerMax;
        lifeTimer = lifeTimerMax;
        tempSpeed = speed;
        reactionsBuff = new ReactionsBuff(damageEnum, damage, count, hitBack, false);
        t = 0;
        oriPos = transform.position;
        ctrlPos = Vector2.zero;
        target = Vector2.zero;
        distance = 0;
        isMoving = false;

        InvokeRepeating(nameof(Hit), 0, 0.13f);
    }

    private void FixedUpdate()
    {

        LifeTime();

        if (isMoving)
        {
            t = Mathf.Lerp(t, 2, Time.deltaTime * speed / distance);


            if (t >= 0.9f)
            {
                //isMoving = false;
                //gameObject.SetActive(false);
                transform.position += transform.right * speed * Time.deltaTime;
                return;
            }

            transform.position = Bezier(oriPos, ctrlPos, this.target, t);
            transform.right = (Vector2)target - (Vector2)transform.position;

        }


    }

    private Vector2 CtrlPos(Vector2 p0, Vector2 p1)
    {
        float x = (p0.x + p1.x) * 0.5f;
        float y = Mathf.Max(p0.y, p1.y) + Random.Range(23, 33);
        Vector2 vector2 = new Vector2(x, y);
        return vector2;


    }

    private Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * t * (1 - t) * p1 + Mathf.Pow(t, 2) * p2;
    }

    public void SetTarget(Transform target)
    {
        if (!isMoving)
        {
            this.target = target.position;
            ctrlPos = CtrlPos(oriPos, target.position);
            distance = Vector2.Distance(oriPos, this.target);
            isMoving = true;
        }
    }
    public void Hit()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
                BuffManager.Instance.AddBuff(5, collider.transform.gameObject);
            }
            //gameObject.SetActive(false);
            //isMoving = false;
        }

        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(transform.position, radius * 0.5f, groundLayer);//* 0.5f后和地面碰撞
        if (groundColliders.Length > 0)
        {
            //gameObject.SetActive(false);
            isMoving = false;
        }

    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Hit));
    }


}
