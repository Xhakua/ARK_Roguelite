using System.Collections.Generic;
using UnityEngine;


public class Effect_ElectricChains : MonoBehaviour, ISetLifeTime, ICauseDamage
{
    static public List<int> linkedInstanceIDs = new List<int>();
    public int[] selflinkedInstanceIDs = new int[2];
    public GameObject[] selflinked = new GameObject[2];
    [SerializeField] private float lifeTimerMax;
    [SerializeField] private float radius = 50;
    [SerializeField] private LayerMask layerMask;
    private float lifeTimer;
    private ReactionsBuff reactionsBuff;
    //private bool canChains = true;
    [SerializeField] private LineRenderer lineRenderer;

    private void OnEnable()
    {
        lifeTimer = 0;
        //canChains= true;
        lineRenderer.positionCount = 0;
        selflinkedInstanceIDs[1] = 0;
        selflinked[1] = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

        if (!linkedInstanceIDs.Contains(selflinkedInstanceIDs[0]))
        {
            linkedInstanceIDs.Add(selflinkedInstanceIDs[0]);
        }

        foreach (var item in colliders)
        {
            if (item.gameObject.GetComponent<BaseEnemy>().reactionsBuff.GetDamageEnum() == ReactionsBuff.DamageEnum.metal)
            {
                if (linkedInstanceIDs.Contains(item.gameObject.GetInstanceID()))
                {
                    continue;
                }
                //linkedInstanceIDs.Add(item.gameObject.GetInstanceID());
                //selflinkedInstanceIDs[1] = item.gameObject.GetInstanceID();
                selflinked[1] = item.gameObject;
                break;
            }
        }

        selflinked[0].GetComponent<BaseEnemy>().Hurt(reactionsBuff);


        if (selflinked[1] != null)
        {
            selflinked[1].GetComponent<BaseEnemy>().Hurt(reactionsBuff);
            lineRenderer.gameObject.SetActive(true);
        }
        //排除自己回来再写.ok了
    }

    private void OnDisable()
    {
        lineRenderer.gameObject.SetActive(false);
        foreach (var item in selflinkedInstanceIDs)
        {
            linkedInstanceIDs.Remove(item);
        }
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
    }


    //private void Update()
    //{
    //    LifeTime();
    //    Hit();
    //}


    private void FixedUpdate()
    {
        LifeTime();
        Hit();
    }


    private void Hit()
    {
        if (selflinked[1] == null || selflinked[0] == null)
        {
            gameObject.SetActive(false);
            return;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { selflinked[0].transform.position, selflinked[1].transform.position });
    }

}
