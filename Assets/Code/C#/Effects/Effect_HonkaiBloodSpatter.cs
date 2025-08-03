using System.Collections;
using UnityEngine;

public class Effect_HonkaiBloodSpatter : MonoBehaviour, ISetLifeTime
{
    [SerializeField] private float lifeTimerMax;

    public void LifeTime()
    {
        gameObject.SetActive(false);

    }
    public void SetLifeTime(float lifeTime)
    {
        lifeTimerMax = lifeTime;
    }


    public void SetState(float angle, float strength)
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.startSpeed = strength * 20;
        ParticleSystem.EmissionModule emission = particleSystem.emission;

        int count = Mathf.Clamp((int)(strength * 15), 1, 15);

        emission.SetBurst(0, new ParticleSystem.Burst(0, count));
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle - 45);

    }

    public void SetState(float strength)
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.startSpeed = strength * 20;
        ParticleSystem.EmissionModule emission = particleSystem.emission;

        int count = Mathf.Clamp((int)(strength * 15), 1, 15);

        emission.SetBurst(0, new ParticleSystem.Burst(0, count));


    }
    private void OnEnable()
    {
        StartCoroutine(Life());
    }
    private IEnumerator Life()
    {
        yield return new WaitForSeconds(lifeTimerMax);
        LifeTime();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}



