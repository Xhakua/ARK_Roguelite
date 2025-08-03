using NUnit.Framework;
using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea]public string itemDescription;
    public Sprite itemSprite;
    public InventoryManager.MaterialEnum materialEnum;
    public int complexity;
    //public int damageCount;
    public GameObject itemPrefab;
    public ItemManager.ItemEnum itemType = ItemManager.ItemEnum.Empty;
    public bool canBagInteraction;

    [Header("Common")]
    public int damage;
    public ReactionsBuff.DamageEnum damageEnum;
    public int damageCount;
    public int hitBack;

    [Header("Remote")]
    public float damageMul;
    public float speedMul;
    public float hitbackMul;
    public float recoilMax;
    public float recoilMin;
    public float recoilforce;
    public float lifeTimeMul;

    [Header("Ammo")]
    public float lifeTimerMax;
    public float speed;

    [Header("Melee")]
    public float swingSpeed;
    public int attackCount;
    public float whackMul;
    public float whackTimerMax;

    [Header("Gene")]
    public CharacterDataSO occupationData;
    public GeneTypes geneType = GeneTypes.Constant;
    public int geneGrade;           //基因级别
    public enum GeneTypes
    {
        TriggerableOnTakeDamage,    //照成伤害触发基因
        TriggerableOnHurt,          //受伤触发基因
        TriggerableOnTime,          //时间触发基因
        TriggerableOnDeath,         //死亡触发基因
        Constant                    //恒定基因
    }
    [Header("基因增益数值")]
    public int Gene_healthMaxValue = 0;                 //生命最大值
    public int Gene_magicMaxValue = 0;                  //魔法最大值
    public int Gene_defenseValue = 0;                   //防御
    public float Gene_meleeAttackMultiplierRate = 1;    //近战攻击乘数
    public float Gene_meleeSpeedMultiplierRate = 1;     //近战速度乘数
    public float Gene_remoteAttackMultiplierRate = 1;   //远程攻击乘数
    public float Gene_remoteSpeedMultiplierRate = 1;    //远程速度乘数
    public float Gene_magicAttackMultiplierRate = 1;    //魔法攻击乘数
    public float Gene_magicSpeedMultiplierRate = 1;     //魔法速度乘数
    public int Gene_releaseMagicCostRate = 1;           //魔法消耗乘数
    public float Gene_moveSpeedMaxRate = 1;             //移动速度乘数
    public float Gene_SkillSizeRate = 1;                //技能大小乘数
    public float Gene_SkillLifeTimeRate = 1;            //技能持续时间乘数
    public float Gene_SkillTargetRangeRate = 1;         //索敌范围乘数
    [Header("基因固有数值")]
    public float Gene_AttackValue;                      //攻击数值
    public float fireRateTimerMax;                      //攻击冷却
    public float SkillSize;                             //技能大小
    public float SkillLifeTime;                         //技能持续时间
    public float SkillTargetRange;                      //索敌范围
    public bool NeedTarget;                             //是否需要索敌
}
