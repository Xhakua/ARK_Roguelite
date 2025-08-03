using UnityEngine;

public class Effect_BuffVisual : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    private GameObject target;
    private int lastCount = 0;

    private void OnBuffChanged(object sender, IReactionsUI.OnBuffChangedEventArgs e)
    {
        if (e.buff.GetDamageEnum() == ReactionsBuff.DamageEnum.electric)
        {
            ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
            emissionModule.rateOverTime = (int)(e.buff.GetCount() * 0.1f);
            return;
        }
        ParticleSystem.MainModule main = particleSystem.main;
        if (e.buff.GetCount() > lastCount)
            main.maxParticles = (int)(e.buff.GetCount() * 0.1f);
        else
        {
            particleSystem.Clear();
            main.maxParticles = (int)(e.buff.GetCount() * 0.1f);
        }
        lastCount = e.buff.GetCount();
    }
    private void OnDisable()
    {
        if (target != null)
            target.GetComponent<IReactionsUI>().OnBuffChanged -= OnBuffChanged;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
        //transform.SetParent(target.transform);
        this.target.GetComponent<IReactionsUI>().OnBuffChanged += OnBuffChanged;
    }
}
