using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeBonusGene : MonoBehaviour, IGeneEffect
{
    [SerializeField] protected ItemSO itemSO;
    private void OnEnable()
    {
        ApplyItemModifiers();
    }
    private void ApplyItemModifiers()
    {
    }

    public void StartEffect()
    {
        PlayerManager.Instance.GetPlayer().OccupationData.bonusList.Add(itemSO.occupationData);
    }

    public void Effect()
    {
        
    }

    public void StopEffect()
    {
        PlayerManager.Instance.GetPlayer().OccupationData.bonusList.Remove(itemSO.occupationData);
    }

    void IGeneEffect.EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource)
    {
        throw new System.NotImplementedException();
    }

    void IGeneEffect.EffectOnHurt()
    {
        throw new System.NotImplementedException();
    }

    void IGeneEffect.EffectOnDeath()
    {
        throw new System.NotImplementedException();
    }
}
