
using System;
using UnityEngine;

public class GeneBase : MonoBehaviour, IGeneEffect
{
    public virtual void Effect()
    {
    }

    public virtual void StartEffect()
    {
    }

    public virtual void StopEffect()
    {
    }

    //处理照成伤害时触发的事件
    public virtual void EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource)
    {
    }
    public void EffectEventOnTakeDamage(object sender, Player.OnTakeDamageEventArgs e)
    {
        Transform damageTarget = e.target;
        Transform damagerSource = e.source;
        EffectOnTakeDamager(damageTarget, damagerSource);
    }

    //处理受伤时触发的事件
    public virtual void EffectOnHurt()
    {
    }
    public void EffectEventOnHurt(object sender, EventArgs e)
    {
        EffectOnHurt();
    }

    //处理死亡时触发的事件
    public virtual void EffectOnDeath()
    {
    }
    public void EffectEventOnDeath(object sender, EventArgs e)
    {
        EffectOnDeath();
    }
}