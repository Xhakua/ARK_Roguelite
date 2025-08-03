using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IHurt, ISetHealthUI, IReactionsUI, ISetDeltaTimeScale, ISufferBuff
{
    public bool canMove = true;
    [SerializeField] protected GameObject enemyMainBodyVisual;
    [SerializeField] protected Transform buffContainer;
    [SerializeField] protected LayerMask atkLayer;
    [SerializeField] protected Collider2D atkBox;
    [SerializeField] protected float healthMax = 100;
    [SerializeField] protected float damage;
    [SerializeField] protected float defense;
    //反应计数衰减
    [SerializeField] protected int reactionCountAttenuation;
    [SerializeField] protected float speedMax;
    [SerializeField] protected float acceleration;
    [SerializeField] protected float magicResistance = 50;

    //关于怪物技能
    [SerializeField] protected bool HaveSkill = false;
    [SerializeField] protected float SkillColdTimeMax = 1;
    protected float currSkillColdTime = 0;
    protected bool CanReleaseSkill = false;

    public ReactionsBuff reactionsBuff;
    protected ReactionsBuff.DamageEnum lastDamageEnum;
    protected float health;
    protected Rigidbody rb;

    protected bool isHitbacking = false;
    protected Vector3 hitBackDir;
    protected float hitBackForce;
    protected float hitBackTime;
    protected bool hasDebuff;
    protected GameObject buffEffect;
    protected float deltaTimeMultiplier = 1;
    [Serializable]
    public struct LootsList
    {
        public ItemSO loot;
        public float dropRate;
    }
    [SerializeField] protected LootsList[] lootsList;
    protected List<int> buffList = new List<int>();

    protected Vector2Int lastpos;
    protected CubeSO wallData;

    public Vector2Int dir;

    public void Init(CharacterDataSO characterData)
    {
        enemyMainBodyVisual = characterData.Model;
        healthMax = characterData.HealthMax;
        damage = characterData.MeleeAttackAddition;
        defense = characterData.Defense;
        reactionCountAttenuation = characterData.ReactionCountAttenuation;
        speedMax = characterData.MoveSpeedMax;
        magicResistance = characterData.MagicResistance;
        acceleration = characterData.Acceleration;


    }

    public event EventHandler<ISetHealthUI.OnProgressChangedEventArgs> OnHealthUIChanged;
    public event EventHandler<IReactionsUI.OnBuffChangedEventArgs> OnBuffChanged;

    protected void OnEnable()
    {
        health = healthMax;
        currSkillColdTime = SkillColdTimeMax;
        reactionsBuff = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, 0);
        lastDamageEnum = reactionsBuff.GetDamageEnum();
        SetDeltaTime(1);
    }
    virtual protected void Start()
    {
        health = healthMax;

        reactionsBuff = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, 0);
        lastDamageEnum = reactionsBuff.GetDamageEnum();
        rb = GetComponent<Rigidbody>();
        TickManager.Instance.OnTick_1 += ReactionCountAttenuation;
    }


    private void Die()
    {
        EnemyManager.Instance.RemoveEnemy(gameObject);
        //TODO:该处功能是否重复
        //gameObject.SetActive(false);
    }

    private void ReactionCountAttenuation(object sender, EventArgs e)
    {
        if (reactionsBuff.GetCount() > 100)
        {
            reactionsBuff.SetCount(0);
            reactionsBuff.SetDamageEnum(ReactionsBuff.DamageEnum.normal);
            OnBuffChanged?.Invoke(this, new IReactionsUI.OnBuffChangedEventArgs { buff = reactionsBuff });
            return;
        }
        if (reactionsBuff.GetCount() > 0)
        {
            reactionsBuff.SetCount(reactionsBuff.GetCount() - reactionCountAttenuation);
            if (reactionsBuff.GetCount() <= 0)
            {
                reactionsBuff.SetCount(0);
                reactionsBuff.SetDamageEnum(ReactionsBuff.DamageEnum.normal);
            }
            OnBuffChanged?.Invoke(this, new IReactionsUI.OnBuffChangedEventArgs { buff = reactionsBuff });
        }
    }

    protected void ContactATK()
    {
        ContactFilter2D contactFilter2D = new()
        {
            useLayerMask = true,
            layerMask = atkLayer
        };
        List<Collider2D> collider2Ds = new List<Collider2D>();
        Physics2D.OverlapCollider(atkBox, contactFilter2D, collider2Ds);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Player"))
            {
                ReactionsBuff reactionsBuff = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, (int)damage, hitback: damage);
                collider.transform.root.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);
            }
        }
    }

    public List<int> GetBuffStructs()
    {
        return buffList;
    }

    public Transform GetBuffParent()
    {
        return buffContainer;
    }

    public void ClearBuff()
    {
        foreach (Transform child in buffContainer)
        {
            Destroy(child.gameObject);
        }
    }

    virtual protected void HitEffect()
    {
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return healthMax;
    }

    public ReactionsBuff Hurt(ReactionsBuff reactionsBuffTakeDamage, GameObject source = null)
    {
        if (source != null)
        {
            hitBackDir = transform.position - source.transform.position;
            hitBackForce = reactionsBuffTakeDamage.GetHitback();
            if (!isHitbacking)
            {
                rb.AddForce(2 * hitBackForce * hitBackDir.normalized, ForceMode.Impulse);//2这个数值是还不错
                isHitbacking = true;
            }
        }
        HitEffect();

        if (reactionsBuffTakeDamage.GetIsAdditionalDamage())
        {
            PlayerManager.Instance.GetPlayer().OnTakeDamageEvent(transform);//受到伤害时触发的事件
        }


        //受击效果
        float strength;
        float angle = Vector2.SignedAngle(Vector2.right, hitBackDir);


        float tempDamage;
        if (reactionsBuffTakeDamage.GetDamageEnum() != ReactionsBuff.DamageEnum.honkai)
        {
            tempDamage = Mathf.Clamp(reactionsBuffTakeDamage.GetDamage() - defense, 1, 99999);
        }
        else
        {
            tempDamage = reactionsBuffTakeDamage.GetDamage() * (1 - magicResistance);
        }

        reactionsBuff.AddBuff(reactionsBuffTakeDamage);
        reactionsBuff.SetTransform(transform);

        //受击效果
        strength = tempDamage / health;


        health -= tempDamage;
        ReactionsBuff tempReactionsBuff = new ReactionsBuff(reactionsBuffTakeDamage.GetDamageEnum(), (int)tempDamage);
        UniversalEffectsManager.Instance.GenerateDamageNum(transform, tempReactionsBuff);//其他伤害反应就用反应后伤害数值

        UniversalEffectsManager.Instance.GenerateHonkaiBloodSpatter(transform, 1f, strength, angle);
        AudioManager.Instance.PlayHitMeatSound(transform.position);

        reactionsBuff.SetDamage(0);
        HealthChange();

        OnBuffChanged?.Invoke(this, new IReactionsUI.OnBuffChangedEventArgs { buff = reactionsBuff });

        if (health <= 0)
        {
            DropLoots();
            ClearBuff();
            EnemyManager.Instance.RemoveEnemy(gameObject);
            return reactionsBuff;
        }

        return reactionsBuff;
    }

    protected void HitbackMovement()//，优化击退
    {
        if (isHitbacking)
        {
            hitBackTime += Time.deltaTime;
            float tempSpeed = hitBackForce - hitBackForce * hitBackTime / 0.2f;
            if (tempSpeed < 0)
            {
                isHitbacking = false;
                hitBackTime = 0;
                return;
            }
        }
    }

    private EnemyState enemyState = EnemyState.Move;
    virtual protected void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Move:
                if(canMove) Move();
                break;
            case EnemyState.Skill:
                if(CanReleaseSkill) Skill();
                break;
            default:
                break;
        }
        //if (canMove && enemyState == EnemyState.Move)
        //{
        //    Move();
        //}
        if (HaveSkill)
        {
            SkillColdTimeUpdate();          //技能冷却刷新
        }

        HitbackMovement();
        AtkCube();
    }

    public int atkCount = 0;
    public float AttackSpeed = 1;
    public float wallDamageCount = 0;
    public float Damage = 1;

    public void AtkCube()
    {
        if (enemyState == EnemyState.AttackWall)
        {
            if (atkCount >= 60 / AttackSpeed)
            {
                if (wallData != null)
                {
                    wallDamageCount += Damage;
                    enemyState = EnemyState.AttackWall;
                    //Debug.Log(wallDamageCount);
                    if (wallDamageCount >= wallData.destroyCount)
                    {
                        try
                        {
                            MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[GetPos().x + dir.x, GetPos().y + dir.y] = 0;
                        }
                        catch (Exception e)
                        {
                            Destroy(gameObject);
                        }
                        wallDamageCount = 0;
                        wallData = null;
                        MapDynamicLoadingManager.Instance.DestroyCube(new Vector2Int(GetPos().x + dir.x, GetPos().y + dir.y));
                        //if (PathManager.Instance.IsPosInChunk(GetPos(), PathManager.Instance.TargetPos))
                        //{
                        //    StartCoroutine(PathManager.Instance.UpdateDijkstraPath(GetPos(), basicAttributes.Block));
                        //}
                        enemyState = EnemyState.Move;
                    }
                }
                else
                {
                    enemyState = EnemyState.Move;
                }
                atkCount = 0;
            }
            else
            {
                atkCount++;
            }

        }
    }

    public void Move()
    {
        Vector2Int currentPos = GetPos();
        if (Heuristic(currentPos, PathManager.Instance.TargetPos) <= 1.1)
        {
            //TODO：关于怪物是否到达就死亡？
            Die();
        }

        if (lastpos != currentPos)
        {
            lastpos = currentPos;
            dir = PathManager.Instance.GetMoveDir(currentPos);
            Vector2Int targetPos = currentPos + dir;

            if (MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[targetPos.x, targetPos.y] != 0)
            {
                Debug.Log("Wall");
                wallData = MapManager.Instance.mapAtlas.GetCube(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[targetPos.x, targetPos.y] - 1);
                if (wallData != null) { enemyState = EnemyState.AttackWall; }
            }
        }

        //判断是否需要释放技能
        if (CanReleaseSkill)
        {
            Debug.Log("EnemySkill");
            enemyState = EnemyState.Skill;
            return;
        }

        if (enemyState == EnemyState.Move)
        {
            if (dir == Vector2Int.zero)
            {
                try
                {
                    StartCoroutine(AStar());
                }
                catch (Exception e)
                {
                    EnemyManager.Instance.RemoveEnemy(gameObject);
                }

                dir = PathManager.Instance.GetMoveDir(currentPos);
                //Debug.Log("AStar");
            }

            Vector3 pos = new Vector3Int(dir.x, 1, dir.y);
            rb.MovePosition(rb.position + pos * speedMax * Time.fixedDeltaTime);
        }
    }



    public float GetHPMax()
    {
        return healthMax;
    }
    public float GetHonkaiImpactPatience()
    {
        return magicResistance;
    }

    public void SetDeltaTime(float deltaTime)
    {
        deltaTimeMultiplier = deltaTime;
        deltaTimeMultiplier = Mathf.Clamp(deltaTimeMultiplier, 0, 1);
    }

    protected void DropLoots()
    {
        foreach (LootsList loot in lootsList)
        {
            if (UnityEngine.Random.value < loot.dropRate)
            {
                ItemManager.Instance.GenerateItem(loot.loot, transform.position);
            }
        }
    }

    public GameObject GetMainVisual()
    {
        return enemyMainBodyVisual;
    }

    protected void HealthChange()
    {
        OnHealthUIChanged?.Invoke(this, new ISetHealthUI.OnProgressChangedEventArgs
        {
            hp = (int)health,
            hpMax = (int)healthMax,
            //healthNormalized = health / HealthMax,
            //honkaiImpactPatienceNormalized = MagicResistance / HealthMax,
            //lostHealthNormalized = lostHealthMax / HealthMax,
            //honkaiImpactNormalized = honkaiImpact / (HealthMax + MagicResistance)
        });
    }
    private Vector2Int GetPos()
    {
        Vector2Int pos = new Vector2Int((int)Math.Round(transform.position.x, MidpointRounding.AwayFromZero), (int)Math.Round(transform.position.z, MidpointRounding.AwayFromZero));
        return pos;
    }
    private IEnumerator AStar()
    {
        if (PathManager.Instance.DistanceField[GetPos().x, GetPos().y] != int.MaxValue)
        {
            yield break;
        }
        //acount = 0;
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0)
        };

        HashSet<Vector2Int> openList = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closeList = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();

        Vector2Int targetPos = PathManager.Instance.TargetPos;
        Vector2Int startPos = GetPos();

        openList.Add(startPos);
        gScore[startPos] = 0;

        while (openList.Count > 0)
        {
            //acount++;
            //Debug.Log(acount);
            //yield return null;
            Vector2Int currentPos = openList.OrderBy(pos => gScore[pos] + Heuristic(pos, targetPos)).First();
            if (PathManager.Instance.DistanceField[currentPos.x, currentPos.y] != int.MaxValue)
            {
                ReconstructPath(cameFrom, currentPos, PathManager.Instance.DistanceField[currentPos.x, currentPos.y]);
                openList.Clear();
                closeList.Clear();
                yield break;
            }

            openList.Remove(currentPos);
            closeList.Add(currentPos);

            for (int i = 0; i < 4; i++)
            {
                //yield return null;
                Vector2Int nextPos = currentPos + directions[i];

                if (closeList.Contains(nextPos)) continue;
                int tentativeGScore = gScore[currentPos];
                int addGScore = 1;
                if (MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[nextPos.x, nextPos.y] != 0)
                {
                    addGScore = PathManager.Instance.allCubes.cubes[MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[nextPos.x, nextPos.y] - 1].destroyCount;
                }

                tentativeGScore += addGScore;
                if (tentativeGScore > 100)
                {
                    EnemyManager.Instance.RemoveEnemy(gameObject);
                }
                if (!openList.Contains(nextPos))
                {
                    openList.Add(nextPos);
                }

                else if (tentativeGScore >= gScore[nextPos])
                {
                    continue;
                }

                cameFrom[nextPos] = currentPos;
                gScore[nextPos] = tentativeGScore;
            }
        }
    }

    private int Heuristic(Vector2Int pos, Vector2Int targetPos)
    {
        return Math.Abs(pos.x - targetPos.x) + Math.Abs(pos.y - targetPos.y);
    }

    private void ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int currentPos, int currentDistance)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (cameFrom.ContainsKey(currentPos))
        {
            path.Add(currentPos);
            currentPos = cameFrom[currentPos];
        }
        path.Reverse();
        PathManager.Instance.SetPath(path, currentDistance);
        //Debug.Log("A* " + acount);
    }


    #region 关于技能
    //技能冷却刷新,不一定仅有时间刷新
    protected virtual void SkillColdTimeUpdate()
    {
        if (currSkillColdTime >= 0)
        {
            currSkillColdTime -= Time.deltaTime;
        }else
        {
            CanReleaseSkill = true;
        }
    }

    //怪物技能释放
    protected virtual void Skill()
    {
        CanReleaseSkill = false;
        currSkillColdTime = SkillColdTimeMax;
        enemyState = EnemyState.Move;
    }
    #endregion
}
public enum EnemyState
{
    Idle,
    Move,
    AttackWall,
    Skill,
}