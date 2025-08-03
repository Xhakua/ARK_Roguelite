using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 能够手持生效的接口
/// </summary>
public interface IHandheldInEffect 
{
    /// <summary>
    /// 手持生效的方法
    /// </summary>
    /// <remarks>
    /// 这个方法可以在手持物品时触发特定的效果或行为
    /// </remarks>
    public void HandheldInEffect();
}
