using UnityEngine;
[CreateAssetMenu(fileName = "BuffSO", menuName = "BuffSO")]
public class BuffSO : ScriptableObject
{

    public BuffManager.BuffEnum buffEnum;
    public Sprite sprite;
    [TextArea] public string description;
    public GameObject buff;
    public float durationMax;
    public float frequency;
}
