using UnityEngine;
/// <summary>
/// 伤害反应类
/// </summary>
public class ReactionsBuff
{
    public enum DamageEnum
    {
        honkai,
        metal,
        highTemperature,
        lowTemperature,
        electric,
        normal,
    }
    //伤害来源
    private Transform thisTransform;
    //伤害类型
    private DamageEnum damageEnum;
    //元素伤害反应积蓄值
    private int count;
    //伤害值
    private int damage;
    //击退值
    private float hitback;
    //高温护盾系数
    private float highTemperatureShieldFactor = 5f;
    //低温护盾系数
    private float lowTemperatureShieldFactor = 5f;
    //温度转金属反应系数
    private float Temperature2MetalCountMultiplier = 2;

    //是否可以附加伤害
    private bool canAdditionalDamage = true;



    public void SetTransform(Transform transform)
    {
        thisTransform = transform;
    }
    public void SetDamageEnum(ReactionsBuff.DamageEnum damageEnum)
    {
        this.damageEnum = damageEnum;
    }
    public ReactionsBuff.DamageEnum GetDamageEnum() { return this.damageEnum; }
    public void SetCount(int count)
    {
        this.count = count;
    }
    public int GetCount() { return this.count; }
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
    public int GetDamage() { return this.damage < 1 ? 1 : damage; ; }
    public void SetHitback(float hitback)
    {
        this.hitback = hitback;
    }
    public float GetHitback() { return this.hitback; }
    public void SetIsAdditionalDamage(bool isAdditionalDamage)
    {
        this.canAdditionalDamage = isAdditionalDamage;
    }
    public bool GetIsAdditionalDamage() { return this.canAdditionalDamage; }
    public override string ToString()
    {
        return "damageEnum: " + damageEnum + " damageCount: " + count + " damage: " + damage + " hitback: " + hitback;
    }



    public ReactionsBuff(ReactionsBuff.DamageEnum damageEnum, int damage, int count = -1, float hitback = 1, bool canAdditionalDamage = true)
    {
        this.damageEnum = damageEnum;
        if (count == -1)
        {
            count = damage;
        }

        this.count = count;
        this.damage = damage;
        this.hitback = hitback;
        this.canAdditionalDamage = canAdditionalDamage;
    }

    //switch伤害反应

    public void AddBuff(ReactionsBuff reactionsBuff)
    {
        if (this.GetDamageEnum() == reactionsBuff.GetDamageEnum())
        {
            this.damage += reactionsBuff.GetDamage();
            this.count += reactionsBuff.GetCount();
        }

        switch (this.damageEnum)
        {
            case ReactionsBuff.DamageEnum.normal:
                this.damage = reactionsBuff.damage;
                this.damageEnum = reactionsBuff.damageEnum;
                this.count = reactionsBuff.count;
                break;
            case ReactionsBuff.DamageEnum.metal:
                switch (reactionsBuff.damageEnum)
                {
                    case ReactionsBuff.DamageEnum.highTemperature:
                        HighTemperature2Metal(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.lowTemperature:
                        LowTemperature2Metal(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.electric:
                        Electric2Metal(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.normal:
                        Normal2Metal(reactionsBuff);
                        break;
                }
                break;
            case ReactionsBuff.DamageEnum.highTemperature:
                switch (reactionsBuff.damageEnum)
                {
                    case ReactionsBuff.DamageEnum.metal:
                        Metal2HighTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.lowTemperature:
                        LowTemperature2HighTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.electric:
                        Electric2HighTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.normal:
                        Normal2Other(reactionsBuff);
                        break;
                }
                break;
            case ReactionsBuff.DamageEnum.lowTemperature:
                switch (reactionsBuff.damageEnum)
                {
                    case ReactionsBuff.DamageEnum.metal:
                        Metal2LowTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.highTemperature:
                        HighTemperature2LowTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.electric:
                        Electric2LowTemperature(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.normal:
                        Normal2LowTemperature(reactionsBuff);
                        break;
                }
                break;
            case ReactionsBuff.DamageEnum.electric:
                switch (reactionsBuff.damageEnum)
                {
                    case ReactionsBuff.DamageEnum.metal:
                        Metal2Electric(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.highTemperature:
                        HighTemperature2Electric(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.lowTemperature:
                        LowTemperature2Electric(reactionsBuff);
                        break;
                    case ReactionsBuff.DamageEnum.normal:
                        Normal2Other(reactionsBuff);
                        break;
                }
                break;
        }
        if (this.count == 0)
        {
            this.damageEnum = ReactionsBuff.DamageEnum.normal;
        }
    }

    private ReactionsBuff Metal2HighTemperature(ReactionsBuff metal)
    {
        this.damage = ((int)(metal.damage - this.count / highTemperatureShieldFactor));

        int tempCount = this.count - metal.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
        }
        else
        {
            this.count = -tempCount;
            this.damage = metal.damage;
            this.damageEnum = metal.damageEnum;
        }
        //Debug.Log("Metal2HighTemperature" + metal);

        return this;
    }

    private ReactionsBuff Metal2Electric(ReactionsBuff metal)
    {
        this.damage = metal.damage;
        int tempCount = this.count / 2;
        if (tempCount > 0)
        {
            this.count = tempCount;
            if (this.damage < 2)
            {
                return this;
            }
            ReactionsBuff electric = new ReactionsBuff(ReactionsBuff.DamageEnum.electric, this.damage / 2, -1, -1, false);
            UniversalEffectsManager.Instance.GenerateElectric(thisTransform, electric, 2f);

        }
        //else
        //{
        //    if (this.damage < 2)
        //    {
        //        return this;
        //    }
        //    ReactionsBuff electric = new ReactionsBuff(ReactionsBuff.GetDamageEnum.electric, this.damage / 2);
        //    UniversalEffectsManager.Instance.GenerateElectric(thisTransform, electric, 2f);
        //    this.damageCount = 0;
        //    this.damageEnum = ReactionsBuff.GetDamageEnum.normal;
        //}
        //Debug.Log("Metal2Electric" + metal);
        return this;
    }





    private ReactionsBuff Metal2LowTemperature(ReactionsBuff metal)
    {
        int tempCount = this.count - metal.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
            this.damage = metal.damage + metal.count;
        }
        else
        {
            this.damage = metal.damage + this.count;
            this.count = -tempCount;
            this.damageEnum = metal.damageEnum;
        }
        //Debug.Log("Metal2LowTemperature" + metal);
        return this;
    }

    private ReactionsBuff HighTemperature2Metal(ReactionsBuff highTemperature)
    {

        int tempCount = this.count - (int)(highTemperature.count * Temperature2MetalCountMultiplier);
        if (tempCount > 0)
        {
            this.count = tempCount;
            this.damage = highTemperature.damage;
        }
        else
        {
            this.count = -tempCount;
            this.damage = highTemperature.damage;
            this.damageEnum = highTemperature.damageEnum;
        }
        //Debug.Log("HighTemperature2Metal" + highTemperature);
        return this;
    }

    private ReactionsBuff HighTemperature2Electric(ReactionsBuff highTemperature)
    {
        this.damage = highTemperature.damage;
        int tempCount = this.count - highTemperature.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
            ReactionsBuff boom = new ReactionsBuff(ReactionsBuff.DamageEnum.electric, highTemperature.count, -1, 1, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, boom, 0.3f);
        }
        else
        {
            ReactionsBuff boom = new ReactionsBuff(ReactionsBuff.DamageEnum.electric, this.count, -1, 1, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, boom, 0.3f);
            this.count = -tempCount;
            this.damageEnum = ReactionsBuff.DamageEnum.normal;
        }
        //Debug.Log("HighTemperature2Electric" + highTemperature);
        return this;
    }


    private ReactionsBuff HighTemperature2LowTemperature(ReactionsBuff highTemperature)
    {
        int tempCount = this.count - highTemperature.count;
        this.damage = highTemperature.damage;
        if (tempCount > 0)
        {
            this.count = tempCount;
            ReactionsBuff broken = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, (int)Mathf.Pow((highTemperature.count / 10f), 2), -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, broken, 0.1f);
        }
        else
        {
            this.count = -tempCount;
            this.damageEnum = highTemperature.damageEnum;
            ReactionsBuff broken = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, (int)Mathf.Pow((highTemperature.count / 10f), 2), -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, broken, 0.1f);
        }
        //Debug.Log("HighTemperature2LowTemperature" + highTemperature);
        return this;
    }

    private ReactionsBuff Electric2Metal(ReactionsBuff electric)
    {
        this.damage = electric.damage;
        if (this.damage < 2)
        {
            return this;
        }
        ReactionsBuff electricChains = new ReactionsBuff(ReactionsBuff.DamageEnum.electric, electric.damage / 2, -1, 0, false);
        UniversalEffectsManager.Instance.GenerateElectricChains(thisTransform, electricChains, 0.5f);
        //Debug.Log("Electric2Metal" + electric);
        return this;
    }

    private ReactionsBuff Electric2HighTemperature(ReactionsBuff electric)
    {
        this.damage = electric.damage;
        int tempCount = this.count - electric.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
            ReactionsBuff boom = new ReactionsBuff(ReactionsBuff.DamageEnum.highTemperature, electric.count, -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, boom, 0.3f);
        }
        else
        {
            ReactionsBuff boom = new ReactionsBuff(ReactionsBuff.DamageEnum.highTemperature, this.count, -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, boom, 0.3f);
            this.count = -tempCount;
            this.damageEnum = ReactionsBuff.DamageEnum.normal;
        }
        //Debug.Log("Electric2HighTemperature" + electric);
        return this;
    }


    private ReactionsBuff Electric2LowTemperature(ReactionsBuff electric)
    {

        this.damage = ((int)(electric.damage - this.count / lowTemperatureShieldFactor));
        int tempCount = this.count - electric.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
        }
        else
        {
            this.count = -tempCount;
            this.damage = electric.damage;
            this.damageEnum = electric.damageEnum;
        }
        //Debug.Log("Electric2LowTemperature" + electric);
        return this;
    }


    private ReactionsBuff LowTemperature2Metal(ReactionsBuff lowTemperature)
    {
        int tempCount = this.count - (int)(lowTemperature.count * Temperature2MetalCountMultiplier);
        if (tempCount > 0)
        {
            this.count = tempCount;
            this.damage = lowTemperature.damage;
        }
        else
        {
            this.count = -tempCount;
            this.damage = lowTemperature.damage;
            this.damageEnum = lowTemperature.damageEnum;
        }
        //Debug.Log("LowTemperature2Metal" + lowTemperature);
        return this;
    }

    private ReactionsBuff LowTemperature2HighTemperature(ReactionsBuff lowTemperature)
    {
        int tempCount = this.count - lowTemperature.count;
        this.damage = lowTemperature.damage;
        if (tempCount > 0)
        {
            this.count = tempCount;
            ReactionsBuff broken = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, (int)Mathf.Pow((lowTemperature.count / 10f), 2), -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, broken, 0.1f);
        }
        else
        {
            this.count = -tempCount;
            this.damageEnum = lowTemperature.damageEnum;
            ReactionsBuff broken = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, (int)Mathf.Pow((lowTemperature.count / 10f), 2), -1, 0, false);
            UniversalEffectsManager.Instance.GenerateBoom(thisTransform, broken, 0.1f);
        }
        //Debug.Log("LowTemperature2HighTemperature" + lowTemperature);
        return this;
    }



    private ReactionsBuff LowTemperature2Electric(ReactionsBuff lowTemperature)
    {
        int tempCount = this.count + lowTemperature.count;
        this.damage = lowTemperature.damage - tempCount;
        this.count = tempCount;
        //Debug.Log("LowTemperature2Electric" + lowTemperature);
        return this;
    }

    private ReactionsBuff Normal2Metal(ReactionsBuff normal)
    {
        if (normal.damage > this.count)
        {
            this.damage = normal.damage + this.count;
        }
        else
        {

            this.damage = 0;

        }
        //Debug.Log("Normal2Metal" + normal);
        return this;
    }

    private ReactionsBuff Normal2LowTemperature(ReactionsBuff normal)
    {
        int tempCount = this.count - normal.count;
        if (tempCount > 0)
        {
            this.count = tempCount;
            this.damage = normal.damage + normal.count;
        }
        else
        {
            this.damage = normal.damage + this.count;
            this.count = -tempCount;
            this.damageEnum = normal.damageEnum;
        }
        //Debug.Log("Normal2LowTemperature" + normal);
        return this;
    }

    private ReactionsBuff Normal2Other(ReactionsBuff normal)
    {
        this.damage = normal.damage;
        //Debug.Log("Normal2Other" + normal);
        return this;
    }
}
