using UnityEngine;

public class Buff_Paralysis : BaseBuff
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

        buffVFX = UniversalEffectsManager.Instance.GenerateEffect_ElectricBuff(transform, duration);
        ParticleSystem particleElectric = buffVFX.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.ShapeModule electricShapeModule = particleElectric.shape;
        electricShapeModule.spriteRenderer = target.GetComponent<BaseEnemy>().GetMainVisual().GetComponent<SpriteRenderer>();
        buffVFX.GetComponent<Effect_BuffVisual>().SetTarget(target);
    }

    private void Buff_Paralysis_OnBuffChanged(object sender, IReactionsUI.OnBuffChangedEventArgs e)
    {
        if (e.buff.GetDamageEnum() != ReactionsBuff.DamageEnum.electric)
        {
            duration = -1;
        }
        count = e.buff.GetCount();
    }

    override protected void Effect()
    {
        ihasDeltaTime.SetDeltaTime(1);
        if (UnityEngine.Random.value < ((float)count / (0.7f * healthMax)))
        {
            ihasDeltaTime.SetDeltaTime(0);
        }
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
