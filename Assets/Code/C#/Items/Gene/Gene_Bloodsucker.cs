
using UnityEngine;

public class Gene_Bloodsucker : GeneBase
{
    [SerializeField] protected ItemSO itemSO;
    private int damage;                         //回血量基于该基因伤害
    private SpriteRenderer spriteRenderer;

    // 回血特效
    [SerializeField] protected ParticleSystem Effect_Heal;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        damage = itemSO.damage;

    }

    public override void EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource)
    {
        if (TakeDamagerTarget != null)
        {
            TakeDamagerSource.TryGetComponent(out IHeal heal);
            if (heal != null)
            {
                heal.Heal(damage);
                Effect_Heal.Play();
            }
        }
    }
}