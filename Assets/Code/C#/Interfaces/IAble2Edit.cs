using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 能够子弹编辑的接口
/// </summary>
public interface IAble2Edit
{
    public int Edit(GameObject self,InventorySO inventory,int index);
}
