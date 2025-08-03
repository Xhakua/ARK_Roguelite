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
    public int geneGrade;           //���򼶱�
    public enum GeneTypes
    {
        TriggerableOnTakeDamage,    //�ճ��˺���������
        TriggerableOnHurt,          //���˴�������
        TriggerableOnTime,          //ʱ�䴥������
        TriggerableOnDeath,         //������������
        Constant                    //�㶨����
    }
    [Header("����������ֵ")]
    public int Gene_healthMaxValue = 0;                 //�������ֵ
    public int Gene_magicMaxValue = 0;                  //ħ�����ֵ
    public int Gene_defenseValue = 0;                   //����
    public float Gene_meleeAttackMultiplierRate = 1;    //��ս��������
    public float Gene_meleeSpeedMultiplierRate = 1;     //��ս�ٶȳ���
    public float Gene_remoteAttackMultiplierRate = 1;   //Զ�̹�������
    public float Gene_remoteSpeedMultiplierRate = 1;    //Զ���ٶȳ���
    public float Gene_magicAttackMultiplierRate = 1;    //ħ����������
    public float Gene_magicSpeedMultiplierRate = 1;     //ħ���ٶȳ���
    public int Gene_releaseMagicCostRate = 1;           //ħ�����ĳ���
    public float Gene_moveSpeedMaxRate = 1;             //�ƶ��ٶȳ���
    public float Gene_SkillSizeRate = 1;                //���ܴ�С����
    public float Gene_SkillLifeTimeRate = 1;            //���ܳ���ʱ�����
    public float Gene_SkillTargetRangeRate = 1;         //���з�Χ����
    [Header("���������ֵ")]
    public float Gene_AttackValue;                      //������ֵ
    public float fireRateTimerMax;                      //������ȴ
    public float SkillSize;                             //���ܴ�С
    public float SkillLifeTime;                         //���ܳ���ʱ��
    public float SkillTargetRange;                      //���з�Χ
    public bool NeedTarget;                             //�Ƿ���Ҫ����
}
