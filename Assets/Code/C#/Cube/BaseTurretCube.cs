using UnityEngine;

public class BaseTurretCube : BaseCube
{

    [SerializeField] protected InventorySO geneList;
    [SerializeField] private ItemSO oriGene;
    [SerializeField] protected CharacterDataSO characterDataSO;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Transform container;

    public override void PlacedTriggered()
    {
        isSet = true;
        Debug.Log("BaseTurretCube PlacedTriggered: " + gameObject.name);
        geneList.AddItem(oriGene);
        for (int i = 0; i < geneList.items.Count; i++)
        {
            if (geneList.IsNull(i))
            {
                continue;
            }
            GameObject item = Instantiate(geneList.items[i].itemPrefab, container);
        }

    }


}


//±¸ÍüÂ¼
//[SerializeField] protected Transform firePoint;
//[SerializeField] protected Transform Anchor;

//[SerializeField] protected LayerMask groundlayerMask;

//[SerializeField] protected float range;
//[SerializeField] protected float turnSpeed;
//[SerializeField] protected LayerMask layerMask;

//[SerializeField] protected ParticleSystem Effect_ThrowingShells;

//protected List<GameObject> deployedTurrets = new List<GameObject>();
//protected Transform target;
//protected Vector3 aimDir;
//protected float angle;
//protected int BulletListNum = 0;
//protected float fireRateTimer;
//protected float lifeTimer;
//protected bool isGround = false;
//protected bool isDeployed = false;

//protected float fireRateTimerMax;
//protected float offset_X;
//protected float offset_Y;
//protected float recoilforce;
//protected float damageMul;
//protected float speedMul;
//protected float hitbackMul;
//protected float lifeTimeMul;
//private void InitializeAttributes()
//{
//    fireRateTimerMax = itemSO.fireRateTimerMax;
//    recoilforce = itemSO.recoilforce;
//    damageMul = itemSO.damageMul;
//    speedMul = itemSO.speedMul;
//    hitbackMul = itemSO.hitbackMul;
//    lifeTimeMul = itemSO.lifeTimeMul;
//}

//private void OnEnable()
//{
//    InitializeAttributes();
//    if (transform.parent == null)
//    {
//        isDeployed = true;
//    }
//    else if (transform.parent.CompareTag("Player"))
//    {
//        isDeployed = false;
//    }
//}

//private void FixedUpdate()
//{
//    if (isDeployed)
//        Deploy();
//}
//protected void Deploy()
//{
//    SeekTarget();
//    if (target)
//    {
//        Fire();
//    }
//}
//protected void SeekTarget()
//{
//    Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);
//    float shortestDistance = Mathf.Infinity;
//    Transform nearestEnemy = null;

//    foreach (Collider collider in colliders)
//    {
//        float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
//        if (distanceToEnemy < shortestDistance)
//        {
//            shortestDistance = distanceToEnemy;
//            nearestEnemy = collider.transform;
//        }
//    }
//    target = nearestEnemy;
//    if (target)
//    {
//        aimDir = (target.position - Anchor.position).normalized;

//        angle = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg - 90 + Random.Range(-15f, 15f);


//        Quaternion lookRotation = Quaternion.Euler(0, angle, 0);
//        Debug.Log(angle);
//        Anchor.rotation = Quaternion.Slerp(Anchor.rotation, lookRotation, Time.fixedDeltaTime * turnSpeed);

//        // Anchor.localScale = Mathf.Abs(angle) < 90 ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
//    }
//}
//private void Fire()
//{
//    if (fireRateTimer < fireRateTimerMax)
//    {
//        fireRateTimer += Time.deltaTime;
//        return;
//    }
//    if (geneList == null)
//    {
//        return;
//    }

//    int startBulletListNum = BulletListNum;
//    while (geneList.IsNull(BulletListNum))
//    {
//        BulletListNum++;
//        if (BulletListNum >= geneList.items.Count)
//        {
//            BulletListNum = 0;
//        }
//        if (BulletListNum == startBulletListNum)
//        {
//            return;
//        }
//    }

//    GameObject bullet = BulletManager.Instance.GenerateBullet(firePoint, geneList.GetItem(BulletListNum));
//    if (bullet == null)
//    {
//        return;
//    }
//    bullet.GetComponent<BaseBullet>().SetBulletData(
//        damageMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteAttackMultiplier,
//        speedMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier,
//        lifeTimeMul,
//        hitbackMul * PlayerManager.Instance.GetPlayer().OccupationData.HitBackRsistance);

//    AudioManager.Instance.PlayRangedAttackSound(transform.position);
//    Effect_ThrowingShells.Play();
//    BulletListNum++;
//    fireRateTimer = 0;
//    if (BulletListNum >= geneList.items.Count)
//    {
//        BulletListNum = 0;
//    }
//}