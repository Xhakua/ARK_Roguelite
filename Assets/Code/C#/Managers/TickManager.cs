using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections;


public class TickManager : MonoBehaviour
{
    public static TickManager Instance { get; private set; }
    private readonly WaitForSeconds wait_60 = new WaitForSeconds(1f / 60f);
    private readonly WaitForSeconds wait_1 = new WaitForSeconds(1f);
    private readonly WaitForSeconds wait_30 = new WaitForSeconds(1f / 30f);
    public event EventHandler OnTick_60;
    public event EventHandler OnTick_1;
    public event EventHandler OnTick_30;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(Tick_60());
        StartCoroutine(Tick_1());
        StartCoroutine(Tick_30());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    private IEnumerator Tick_60()
    {
        while (true)
        {
            OnTick_60?.Invoke(this, EventArgs.Empty);
            yield return wait_60;
        }
    }

    private IEnumerator Tick_1()
    {
        while (true)
        {
            OnTick_1?.Invoke(this, EventArgs.Empty);
            yield return wait_1;
        }
    }

    private IEnumerator Tick_30()
    {
        while (true)
        {
            OnTick_30?.Invoke(this, EventArgs.Empty);
            yield return wait_30;
        }
    }
}
