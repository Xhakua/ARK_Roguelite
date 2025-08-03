using UnityEngine;

public class Bullet : BaseBullet
{
    private new void OnEnable()
    {
        base.OnEnable();

    }
    public void Launch()
    {
        transform.position += tempSpeed * Time.deltaTime * transform.right;
    }
    public void Hit()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach (Collider collider in colliders)
        {
            Debug.Log(collider.name);
            if (collider.CompareTag("Enemy"))
            {
                if (damageEnum == ReactionsBuff.DamageEnum.electric)
                    BuffManager.Instance.AddBuff(2, collider.gameObject);
                if (damageEnum == ReactionsBuff.DamageEnum.lowTemperature)
                    BuffManager.Instance.AddBuff(1, collider.gameObject);
                if (damageEnum == ReactionsBuff.DamageEnum.highTemperature)
                    BuffManager.Instance.AddBuff(0, collider.gameObject);
                collider.GetComponent<IHurt>().Hurt(reactionsBuff, gameObject);

            }
            gameObject.SetActive(false);
            UniversalEffectsManager.Instance.GenerateBulletHit(transform, 0.2f, tempSpeed / 50f);
        }




    }



    private void Update()
    {
        Launch();
        LifeTime();
        Hit();
    }


}
