using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 能够与背包交互的下拉菜单接口
/// </summary>
public interface IAble2BagInteraction_DropDown 
{
    /// <summary>
    /// 获取背包交互的枚举值
    /// </summary>
    /// <returns></returns>
    public Enum BagInteraction();

    /// <summary>
    /// 设置背包交互的枚举值
    /// </summary>
    /// <param name="i"></param>
    public void SetEnum(int i);
}
