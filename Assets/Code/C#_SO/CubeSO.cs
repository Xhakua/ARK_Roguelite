using UnityEngine;

[CreateAssetMenu(fileName = "CubeSO", menuName = "CubeSO")]
public class CubeSO : ScriptableObject
{
    public int id;
    public GameObject cube;
    public int destroyCount = 1;
    public ItemSO loot;
    public Color mapColor;
}
