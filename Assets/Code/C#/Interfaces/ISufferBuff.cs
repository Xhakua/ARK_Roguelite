


using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 能承受Buff的接口
/// </summary>
public interface ISufferBuff 
{
    public List<int> GetBuffStructs();

    public Transform GetBuffParent();


}
