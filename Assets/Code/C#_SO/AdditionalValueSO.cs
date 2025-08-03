using System;
using UnityEngine;
[CreateAssetMenu(fileName = "AdditionalValueSO", menuName = "ScriptableObject/AdditionalValueSO")]
public class AdditionalValueSO : ScriptableObject
{
    [Header("血量")]
    public int healthMax;
    [Header("魔法值")]
    public int magicMax;
    [Header("防御")]
    public int defense;
    [Header("近战")]
    public float meleeAttackMultiplier;
    public float meleeSpeedMultiplier;
    [Header("远程")]
    public float remoteAttackMultiplier;
    public float remoteSpeedMultiplier;
    [Header("魔法")]
    public float magicAttackMultiplier;
    public float magicSpeedMultiplier;
    public int releaseMagicCost;
    [Header("移动")]
    public float moveSpeedMax;
    [Header("技能特点")]
    public float SkillSize;
    public float SkillLifeTime;
    [Header("索敌机制")]
    public float SkillTargetRange;

    //装备总属性出现计算
    public void UpdateAdditionalValue(InventorySO equipmentsInventory)
    {
        healthMax = 0;
        magicMax = 0;
        defense = 0;
        meleeAttackMultiplier = 1;
        meleeSpeedMultiplier = 1;
        remoteAttackMultiplier = 1;
        remoteSpeedMultiplier = 1;
        magicAttackMultiplier = 1;
        magicSpeedMultiplier = 1;
        releaseMagicCost = 0;
        moveSpeedMax = 1;
        SkillSize = 1;
        SkillLifeTime = 1;
        SkillTargetRange = 1;

        foreach (ItemSO item in equipmentsInventory.items)
        {
            if (item != null && item.itemType == ItemManager.ItemEnum.Equipment)
            {
                healthMax += item.Gene_healthMaxValue;
                magicMax += item.Gene_magicMaxValue;
                defense += item.Gene_defenseValue;
                meleeAttackMultiplier *= item.Gene_meleeAttackMultiplierRate;
                meleeSpeedMultiplier *= item.Gene_meleeSpeedMultiplierRate;
                remoteAttackMultiplier *= item.Gene_remoteAttackMultiplierRate;
                remoteSpeedMultiplier *= item.Gene_remoteSpeedMultiplierRate;
                magicAttackMultiplier *= item.Gene_magicAttackMultiplierRate;
                magicSpeedMultiplier *= item.Gene_magicSpeedMultiplierRate;
                releaseMagicCost += item.Gene_releaseMagicCostRate;
                moveSpeedMax *= item.Gene_moveSpeedMaxRate;
                SkillSize *= item.Gene_SkillSizeRate;
                SkillLifeTime *= item.Gene_SkillLifeTimeRate;
                SkillTargetRange *= item.Gene_SkillTargetRangeRate;
            }
        }
    }
}
