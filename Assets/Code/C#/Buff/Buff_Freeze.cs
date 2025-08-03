using UnityEngine;

public class Buff_Freeze : BaseBuff
{
    private GameObject buffVFX;
    private float healthMax;
    private IHurt icanHurt;
    private ISetDeltaTimeScale ihasDeltaTime;

    private int count = 0;
    override protected void StartEffect()
    {
        this.icanHurt = target.GetComponent<IHurt>();
        this.ihasDeltaTime = target.GetComponent<ISetDeltaTimeScale>();
        this.healthMax = icanHurt.GetMaxHealth();
        target.GetComponent<IReactionsUI>().OnBuffChanged += Buff_Paralysis_OnBuffChanged;

        buffVFX = UniversalEffectsManager.Instance.GenerateEffect_Ice(transform, duration);
        ParticleSystem particleLowTemperature = buffVFX.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.ShapeModule lowTemperatureShapeModule = particleLowTemperature.shape;
        lowTemperatureShapeModule.spriteRenderer = target.GetComponent<BaseEnemy>().GetMainVisual().GetComponent<SpriteRenderer>();
        buffVFX.GetComponent<Effect_BuffVisual>().SetTarget(target);

    }

    private void Buff_Paralysis_OnBuffChanged(object sender, IReactionsUI.OnBuffChangedEventArgs e)
    {
        if (e.buff.GetDamageEnum() != ReactionsBuff.DamageEnum.lowTemperature)
        {
            duration = -1;
        }
        count = e.buff.GetCount();
    }

    override protected void Effect()
    {
        ihasDeltaTime.SetDeltaTime(1f - ((float)count / (0.7f * healthMax)));
    }

    override protected void StopEffect()
    {
        ihasDeltaTime.SetDeltaTime(1);
        buffVFX.SetActive(false);
    }

    private void OnDestroy()
    {
        if (buffVFX != null)
            buffVFX.SetActive(false);
    }
}
