using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletListSO", menuName = "BulletListSO")]
public class BulletListSO : ScriptableObject
{
    public List<GameObject> bullets;
}
