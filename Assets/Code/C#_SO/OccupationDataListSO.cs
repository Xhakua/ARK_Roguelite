using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OccupationDataListSO", menuName = "ScriptableObject/OccupationDataListSO", order = 0)]
public class OccupationDataListSO : ScriptableObject
{
   public List<CharacterDataSO> occupationDataList = new List<CharacterDataSO>();
}
