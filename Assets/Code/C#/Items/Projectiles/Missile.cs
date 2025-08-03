using UnityEngine;

public class Missile : BaseBullet
{
    private Vector3 target;
    private Vector3 oriPos;
    private Vector3 ctrlPos;
    bool isMoving = false;
    private float distance;
    private float t = 0;
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
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            t = Mathf.Lerp(t, (1.4f + t) * (1.4f + t), Time.deltaTime * speed / distance);
            transform.position = Bezier(oriPos, ctrlPos, this.target, t);
            transform.right = (Vector2)target - (Vector2)transform.position;

            if (t >= 1)
            {
                isMoving = false;
                gameObject.SetActive(false);
            }
            LifeTime();
            Hit();
        }
    }

    private Vector2 CtrlPos(Vector2 p0, Vector2 p1)
    {
        Vector2 a = Vector2.Lerp(p0, p1, 0.2f);
        Vector2 n = Vector2.Perpendicular(p1 - p0).normalized;
        float r = Random.value - 0.5f;
        return a + (p0 - p1).magnitude * r * n;

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
            }
            gameObject.SetActive(false);
        }




    }
}
