using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionModule : MonoBehaviour, IAble2Edit, IAble2BagInteraction_DropDown, IAble2BagInteraction_Scrollbar, IApplyPlayerPrefsData
{
    private bool isfirst = true;
    private EditableBullet editableBullet;
    public enum ConditionType
    {
        hit,
        onDisable,
        onEnable,

    }
    public ConditionType conditionType;
    public int ctrlRange;



    public void ApplyData()
    {
        isfirst = true;
        ctrlRange = (int)(PlayerPrefs.GetFloat("ctrlRange") * 4);
        conditionType = (ConditionType)PlayerPrefs.GetInt(name);
    }

    public Enum BagInteraction()
    {
        return conditionType;
    }

    public void SetEnum(int @enum)
    {
        this.conditionType = (ConditionType)@enum;
    }



    public Dictionary<string, float> GetScrollbarMultipliers()
    {
        Dictionary<string, float> dict = new Dictionary<string, float>
        {
            { "ctrlRange", 4 },
        };
        return dict;
    }

    public void SetScrollbarValues(float[] values)
    {
        ctrlRange = (int)values[0];
    }



    //注意return index++;和return ++index;的区别
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        if (editableBullet == null)
        {
            editableBullet = self.GetComponent<EditableBullet>();
        }
        switch (conditionType)
        {
            case ConditionType.hit:
                if (ConditionHit(editableBullet))
                {
                    return index;
                }
                break;
            case ConditionType.onDisable:
                if (ConditionOnDisable(editableBullet))
                {
                    return index;
                }
                break;
            case ConditionType.onEnable:
                if (ConditionOnEnable(editableBullet))
                {
                    return index;
                }
                break;
            default:
                break;
        }
        index += ctrlRange;
        return index;
    }

    private bool ConditionHit(EditableBullet editableBullet)
    {
        if (editableBullet.colliders.Length == 0)
        {
            return false;
        }
        return true;
    }
    private bool ConditionOnDisable(EditableBullet editableBullet)
    {
        if (editableBullet.gameObject.activeSelf == false)
        {
            return true;
        }
        return false;
    }
    private bool ConditionOnEnable(EditableBullet editableBullet)
    {
        if (isfirst)
        {
            isfirst = false;
            return true;
        }
        return false;
    }


}
