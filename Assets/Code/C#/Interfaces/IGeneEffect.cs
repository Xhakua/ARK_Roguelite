using UnityEngine;

public interface IGeneEffect
{
    public void StartEffect();
    public void Effect();
    public void StopEffect();
    public void EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource);
    public void EffectOnHurt();
    public void EffectOnDeath();
}
