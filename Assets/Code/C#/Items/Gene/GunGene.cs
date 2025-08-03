
using UnityEngine;

public class GunGene : GeneBase
{
    [SerializeField] protected ItemSO itemSO;
    //���������Ƕ�
    protected float recoilMax;
    //��������С�Ƕ�
    protected float recoilMin;
    //��������ֵ
    protected float recoilforce;
    //�ӵ��˺�����
    protected float damageMul;
    //�ӵ��ٶȱ���
    protected float speedMul;
    // ���˱���
    protected float hitbackMul;
    // �ӵ��������ڱ���
    protected float lifeTimeMul;
    [SerializeField] protected InventorySO bulletList;
    // �ӵ��������
    protected float fireRateTimerMax;
    protected float fireRateTimer = 100;
    // �����
    [SerializeField] protected Transform firePoint;
    // �ӵ��׿���Ч
    [SerializeField] protected ParticleSystem Effect_ThrowingShells;
    protected int BulletListNum = 0;
    private SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        recoilforce = itemSO.recoilforce;
        recoilMax = itemSO.recoilMax;
        recoilMin = itemSO.recoilMin;
        damageMul = itemSO.damageMul;
        speedMul = itemSO.speedMul;
        hitbackMul = itemSO.hitbackMul;
        lifeTimeMul = itemSO.lifeTimeMul;
        fireRateTimerMax = itemSO.fireRateTimerMax;
    }
    public float GetRateTimer()
    {
        fireRateTimer = fireRateTimerMax + 1; // ��һ����������������
        return fireRateTimerMax;
    }
    public override void Effect()
    {
        if (fireRateTimer < fireRateTimerMax)
        {
            fireRateTimer += Time.deltaTime * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier;
        }
        if (fireRateTimer >= fireRateTimerMax)
        {
            StartEffect();
        }
    }

    public override void StartEffect()
    {
        if (fireRateTimer < fireRateTimerMax)
        {
            return;
        }

        if (bulletList != null)
        {
            // ���ҷǿ��ӵ�
            int startBulletListNum = BulletListNum;
            while (bulletList.IsNull(BulletListNum))
            {
                BulletListNum++;
                if (BulletListNum >= bulletList.items.Count)
                {
                    BulletListNum = 0;
                }
                if (BulletListNum == startBulletListNum)
                {
                    return;
                }
            }
            // �����ӵ�
            transform.parent.GetComponent<AutoAim>().SetRecoil(recoilMin, recoilMax);
            firePoint.right = new Vector3(transform.right.x, 0, transform.right.y);
            GameObject temp = BulletManager.Instance.GenerateBullet(firePoint, bulletList.GetItem(BulletListNum));
            if (temp == null)
            {
                return;
            }
            temp.GetComponent<BaseBullet>().SetBulletData(damageMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteAttackAddition, speedMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier, lifeTimeMul, hitbackMul);
            AudioManager.Instance.PlayRangedAttackSound(transform.position);
            Effect_ThrowingShells.Play();

            BulletListNum++;
            fireRateTimer = 0;
            if (BulletListNum >= bulletList.items.Count)
            {
                BulletListNum = 0;
            }
        }
    }

    public override void StopEffect()
    {
        throw new System.NotImplementedException();
    }
}