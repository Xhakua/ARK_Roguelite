

using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 能够与背包交互的网格接口
/// </summary>

public interface IAble2BagInteraction_Grid
{
    /// <summary>
    /// 获取背包交互的物品
    /// </summary>
    /// <returns></returns>
    public InventorySO BagInteraction();

}
