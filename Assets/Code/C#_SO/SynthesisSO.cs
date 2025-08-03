using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Synthesis", menuName = "SynthesisSO")]
public class SynthesisSO : ScriptableObject
{
    [Serializable]
    public struct synthesisInput
    {
        public ItemSO item;
        public int count;
    }
    public synthesisInput[] inputDic;
    [Serializable]
    public struct synthesisOutput
    {
        public ItemSO item;
        public int count;
    }
    public synthesisOutput[] outputDic;

}

