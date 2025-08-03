using UnityEngine;

public class ShotGun : BaseGun
{
    [SerializeField] private int numberOfProjectiles;
    [SerializeField] private float scatteringAngle;

    public override void OnLeft()
    {
        if (fireRateTimer < fireRateTimerMax)
        {
            return;
        }
        if (bulletList != null)
        {
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
                    // 回到了起始位置，说明没有可用子弹
                    return;
                }
            }

            Quaternion originalRotation = firePoint.rotation;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                firePoint.rotation = Quaternion.Euler(0, 0, originalRotation.eulerAngles.z + Random.Range(-scatteringAngle, scatteringAngle));
                GameObject temp = BulletManager.Instance.GenerateBullet(firePoint, bulletList.GetItem(BulletListNum));
                if (i == 0 && temp == null)
                {
                    return;
                }
                else if (temp == null)
                {
                    break;
                }
                temp.GetComponent<BaseBullet>().SetBulletData(damageMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteAttackMultiplier,
                                                             speedMul * PlayerManager.Instance.GetPlayer().OccupationData.RemoteSpeedMultiplier,
                                                             lifeTimeMul,
                                                             hitbackMul);

            }

            firePoint.rotation = originalRotation;

            transform.parent.GetComponent<TurnTheMouse_PlayerHand>().SetRecoil(recoilMin, recoilMax, recoilforce,PlayerManager.Instance.GetPlayer().OccupationData.HitBackRsistance);
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
}
