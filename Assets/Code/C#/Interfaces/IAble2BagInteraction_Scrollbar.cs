using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 能够与背包交互的滚动条接口
/// </summary>
public interface IAble2BagInteraction_Scrollbar
{
    /// <summary>
    /// 获取背包交互的滚动条名称和对应的缩放值
    /// </summary>
    /// <returns></returns>
    public Dictionary<string,float> GetScrollbarMultipliers();
    /// <summary>
    /// 设置背包交互的滚动条值
    /// </summary>
    /// <param name="values"></param>
    public void SetScrollbarValues(float[] values);
}
