using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ܹ��뱳�������������˵��ӿ�
/// </summary>
public interface IAble2BagInteraction_DropDown 
{
    /// <summary>
    /// ��ȡ����������ö��ֵ
    /// </summary>
    /// <returns></returns>
    public Enum BagInteraction();

    /// <summary>
    /// ���ñ���������ö��ֵ
    /// </summary>
    /// <param name="i"></param>
    public void SetEnum(int i);
}
