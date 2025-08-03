using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "ScriptableObject/CharacterDataSO")]
public class CharacterDataSO : ScriptableObject
{
    [Header("ְҵ����")]
    public string CharacterName { get; set; } = "Default Character Name";

    [Header("ģ��")]
    public GameObject Model;
    [Header("ְҵ����")]
    [TextArea]
    public string CharacterDescription { get; set; } = "Default Character Description";

    [Header("Ѫ��")]
    [SerializeField] private int _healthMax = 100;
    public int HealthMax => Additive(_healthMax, bonus => bonus.HealthMax);

    [SerializeField] private int _heal = 0;
    public int Heal => Additive(_heal, bonus => bonus.Heal);

    [Header("ħ��ֵ")]
    [SerializeField] private int _magicMax;
    public int MagicMax => Additive(_magicMax, bonus => bonus.MagicMax);

    [SerializeField] private int _magicheal;
    public int Magicheal => Additive(_magicheal, bonus => bonus.Magicheal);

    [Header("����")]
    [SerializeField] private int _defense;
    public int Defense => Additive(_defense, bonus => bonus.Defense);

    [SerializeField] private int _reactionCountAttenuation;
    public int ReactionCountAttenuation => Additive(_reactionCountAttenuation, bonus => bonus.ReactionCountAttenuation);

    [Range(0, 1)][SerializeField] private float _magicResistance;
    public float MagicResistance => Multiplicative(_magicResistance, bonus => bonus.MagicResistance);

    [SerializeField] private float _invincibleTime;
    public float InvincibleTime => Additive(_invincibleTime, bonus => bonus.InvincibleTime);

    [Range(0, 1)][SerializeField] private float _hitBackRsistance;
    public float HitBackRsistance => Multiplicative(_hitBackRsistance, bonus => bonus.HitBackRsistance);

    [Header("��ս")]
    [SerializeField] private float _meleeAttackMultiplier = 1f;
    public float MeleeAttackMultiplier => Multiplicative(_meleeAttackMultiplier, bonus => bonus.MeleeAttackMultiplier);

    [SerializeField] private int _meleeAttackAddition;
    public int MeleeAttackAddition => Additive(_meleeAttackAddition, bonus => bonus.MeleeAttackAddition);

    [SerializeField] private float _meleeSpeedMultiplier = 1f;
    public float MeleeSpeedMultiplier => Multiplicative(_meleeSpeedMultiplier, bonus => bonus.MeleeSpeedMultiplier);

    [Header("Զ��")]
    [SerializeField] private float _remoteAttackMultiplier = 1f;
    public float RemoteAttackMultiplier => Multiplicative(_remoteAttackMultiplier, bonus => bonus.RemoteAttackMultiplier);

    [SerializeField] private int _remoteAttackAddition;
    public int RemoteAttackAddition => Additive(_remoteAttackAddition, bonus => bonus.RemoteAttackAddition);

    [SerializeField] private float _remoteSpeedMultiplier = 1f;
    public float RemoteSpeedMultiplier => Multiplicative(_remoteSpeedMultiplier, bonus => bonus.RemoteSpeedMultiplier);

    [SerializeField] private int _resource;
    public int Resource => Additive(_resource, bonus => bonus.Resource);

    [Header("ħ��")]
    [SerializeField] private float _magicAttackMultiplier = 1f;
    public float MagicAttackMultiplier => Multiplicative(_magicAttackMultiplier, bonus => bonus.MagicAttackMultiplier);

    [SerializeField] private int _magicAttackAddition;
    public int MagicAttackAddition => Additive(_magicAttackAddition, bonus => bonus.MagicAttackAddition);

    [SerializeField] private float _magicSpeedMultiplier = 1f;
    public float MagicSpeedMultiplier => Multiplicative(_magicSpeedMultiplier, bonus => bonus.MagicSpeedMultiplier);

    [SerializeField] private int _releaseMagicCost;
    public int ReleaseMagicCost => Additive(_releaseMagicCost, bonus => bonus.ReleaseMagicCost);

    [SerializeField] private int _reactionsBuffCountAddition;
    public int ReactionsBuffCountAddition => Additive(_reactionsBuffCountAddition, bonus => bonus.ReactionsBuffCountAddition);

    [Header("�ƶ�")]
    [SerializeField] private float _moveSpeedMax;
    public float MoveSpeedMax => Additive(_moveSpeedMax, bonus => bonus.MoveSpeedMax);

    [SerializeField] private float _moveSpeedMultiplier = 1f;
    public float MoveSpeedMultiplier => Multiplicative(_moveSpeedMultiplier, bonus => bonus.MoveSpeedMultiplier);

    [SerializeField] private float _acceleration;
    public float Acceleration => Additive(_acceleration, bonus => bonus.Acceleration);

    [Header("�ӳ��б�")]
    public List<CharacterDataSO> bonusList;

    // �ӷ��ۼ�
    private int Additive(int baseValue, Func<CharacterDataSO, int> selector)
    {
        int total = baseValue;
        if (bonusList != null)
        {
            foreach (var bonus in bonusList)
            {
                if (bonus != null)
                    total += selector(bonus);
            }
        }
        return total;
    }
    private float Additive(float baseValue, Func<CharacterDataSO, float> selector)
    {
        float total = baseValue;
        if (bonusList != null)
        {
            foreach (var bonus in bonusList)
            {
                if (bonus != null)
                    total += selector(bonus);
            }
        }
        return total;
    }

    // �˷�����
    private float Multiplicative(float baseValue, Func<CharacterDataSO, float> selector)
    {
        float total = baseValue;
        if (bonusList != null)
        {
            foreach (var bonus in bonusList)
            {
                if (bonus != null)
                    total *= selector(bonus);
            }
        }
        return total;
    }

    public void Copy(CharacterDataSO occupationData)
    {
        CharacterName = occupationData.CharacterName;
        Model = occupationData.Model;
        CharacterDescription = occupationData.CharacterDescription;
        _healthMax = occupationData._healthMax;
        _heal = occupationData._heal;
        _magicMax = occupationData._magicMax;
        _magicheal = occupationData._magicheal;
        _defense = occupationData._defense;
        _reactionCountAttenuation = occupationData._reactionCountAttenuation;
        _magicResistance = occupationData._magicResistance;
        _invincibleTime = occupationData._invincibleTime;
        _hitBackRsistance = occupationData._hitBackRsistance;
        _meleeAttackMultiplier = occupationData._meleeAttackMultiplier;
        _meleeAttackAddition = occupationData._meleeAttackAddition;
        _meleeSpeedMultiplier = occupationData._meleeSpeedMultiplier;
        _remoteAttackMultiplier = occupationData._remoteAttackMultiplier;
        _remoteAttackAddition = occupationData._remoteAttackAddition;
        _remoteSpeedMultiplier = occupationData._remoteSpeedMultiplier;
        _resource = occupationData._resource;
        _magicAttackMultiplier = occupationData._magicAttackMultiplier;
        _magicAttackAddition = occupationData._magicAttackAddition;
        _magicSpeedMultiplier = occupationData._magicSpeedMultiplier;
        _releaseMagicCost = occupationData._releaseMagicCost;
        _reactionsBuffCountAddition = occupationData._reactionsBuffCountAddition;
        _moveSpeedMax = occupationData._moveSpeedMax;
        _moveSpeedMultiplier = occupationData._moveSpeedMultiplier;
        _acceleration = occupationData._acceleration;
        // bonusList ������
    }
}
