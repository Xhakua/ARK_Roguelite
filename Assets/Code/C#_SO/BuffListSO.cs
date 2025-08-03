using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BuffListSO", menuName = "BuffListSO")]
public class BuffListSO : ScriptableObject
{
   public List<BuffSO> buffList;
}
