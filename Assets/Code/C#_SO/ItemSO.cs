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
    public float fireRateTimerMax; 
    [Header("Ammo")]
    public float lifeTimerMax;
    public float speed;

    [Header("Melee")]
    public float swingSpeed;
    public int attackCount;
    public float whackMul;
    public float whackTimerMax;

    [Header("Equip")]
    public CharacterDataSO occupationData;
}
