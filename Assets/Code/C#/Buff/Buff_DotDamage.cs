using UnityEngine;

public class Buff_DotDamage : BaseBuff
{
    [SerializeField] private ReactionsBuff.DamageEnum damageType;
    [SerializeField] private int damage;
    private ReactionsBuff reactionsBuff;
    private IHurt icanHurt;
    private ISetDeltaTimeScale ihasDeltaTime;
    private GameObject buffVFX;
    private float healthMax;
    protected override void StartEffect()
    {
        this.reactionsBuff = new ReactionsBuff(damageType, damage, damage, 0, false);
        this.icanHurt = target.GetComponent<IHurt>();
        this.ihasDeltaTime = target.GetComponent<ISetDeltaTimeScale>();
        this.healthMax = icanHurt.GetMaxHealth();
        switch (damageType)
        {
            //case ReactionsBuff.GetDamageEnum.metal:
            //    if (buffVFX != null)
            //    {
            //        buffVFX.SetActive(false);
            //        buffVFX.SetActive(true);
            //    }
            //    buffVFX = null;
            //    buffVFX = UniversalEffectsManager.Instance.GenerateFireParticle(transform, 100f);
            //    buffVFX.transform.SetParent(transform);
            //    hasDebuff = true;
            //    break;
            case ReactionsBuff.DamageEnum.highTemperature:
                buffVFX = UniversalEffectsManager.Instance.GenerateFireParticle(transform, duration);
                buffVFX.transform.SetParent(transform);
                break;
            case ReactionsBuff.DamageEnum.electric:
                buffVFX = UniversalEffectsManager.Instance.GenerateEffect_ElectricBuff(transform, duration);
                ParticleSystem particleElectric = buffVFX.GetComponentInChildren<ParticleSystem>();
                ParticleSystem.ShapeModule electricShapeModule = particleElectric.shape;
                electricShapeModule.spriteRenderer = target.GetComponent<BaseEnemy>().GetMainVisual().GetComponent<SpriteRenderer>();
                buffVFX.GetComponent<Effect_BuffVisual>().SetTarget(gameObject);

                break;
            case ReactionsBuff.DamageEnum.lowTemperature:
                buffVFX = UniversalEffectsManager.Instance.GenerateEffect_Ice(transform, duration);
                ParticleSystem particleLowTemperature = buffVFX.GetComponentInChildren<ParticleSystem>();
                ParticleSystem.ShapeModule lowTemperatureShapeModule = particleLowTemperature.shape;
                lowTemperatureShapeModule.spriteRenderer = target.GetComponent<BaseEnemy>().GetMainVisual().GetComponent<SpriteRenderer>();
                buffVFX.GetComponent<Effect_BuffVisual>().SetTarget(gameObject);
                break;
            default:
                break;
        }
    }

    protected override void Effect()
    {
        switch (damageType)
        {

            case ReactionsBuff.DamageEnum.highTemperature:
                icanHurt.Hurt(reactionsBuff);
                break;
            case ReactionsBuff.DamageEnum.electric:
                ihasDeltaTime.SetDeltaTime(1);
                if (UnityEngine.Random.value < ((float)reactionsBuff.GetCount() / (0.7f * healthMax)))
                {
                    ihasDeltaTime.SetDeltaTime(0);
                }
                break;
            case ReactionsBuff.DamageEnum.lowTemperature:
                ihasDeltaTime.SetDeltaTime(1f - ((float)reactionsBuff.GetCount() / (0.7f * healthMax)));
                break;
            default:
                break;
        }
    }


    private void OnDestroy()
    {
        if (buffVFX != null)
            buffVFX.SetActive(false);
    }
}
