using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_OathOfJudah : BaseBuff
{
    private ISetDeltaTimeScale ihasDeltaTime;

    override protected void StartEffect()
    {
        ihasDeltaTime = target.GetComponent<ISetDeltaTimeScale>();
        ihasDeltaTime.SetDeltaTime(0);
    }


    override protected void StopEffect()
    {
        ihasDeltaTime.SetDeltaTime(1);
    }
}
