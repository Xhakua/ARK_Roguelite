using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ܹ��뱳�������Ĺ������ӿ�
/// </summary>
public interface IAble2BagInteraction_Scrollbar
{
    /// <summary>
    /// ��ȡ���������Ĺ��������ƺͶ�Ӧ������ֵ
    /// </summary>
    /// <returns></returns>
    public Dictionary<string,float> GetScrollbarMultipliers();
    /// <summary>
    /// ���ñ��������Ĺ�����ֵ
    /// </summary>
    /// <param name="values"></param>
    public void SetScrollbarValues(float[] values);
}
