using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringModule : MonoBehaviour, IAble2Edit, IAble2BagInteraction_Scrollbar, IAble2BagInteraction_DropDown, IApplyPlayerPrefsData
{
    public enum TargetType
    {
        Enemy,
        Player,
        Mouse
    }
    private GameObject target;
    [SerializeField] private TargetType targetType;
    [SerializeField] private float range = 100.0f;
    [SerializeField, Range(0f, 360f)]
    private float steeringSpeed = 0.0f;
    [SerializeField] private LayerMask targetLayer;

    public Dictionary<string, float> GetScrollbarMultipliers()
    {
        Dictionary<string, float> dict = new Dictionary<string, float>
        {
            { "steeringSpeed", 360 }
        };
        return dict;
    }

    public void SetScrollbarValues(float[] values)
    {
        steeringSpeed = values[0];
    }

    public void ApplyData()
    {
        steeringSpeed = PlayerPrefs.GetFloat("steeringSpeed") * 360;
        targetType = (TargetType)PlayerPrefs.GetInt("SteeringModule");

    }

    public Enum BagInteraction()
    {
        return targetType;
    }

    public void SetEnum(int i)
    {
        this.targetType = (TargetType)i;
    }



    public int Edit(GameObject self, InventorySO inventory, int index)
    {

        if (self.activeSelf == false)
        {
            target = null;
        }
        if (target == null)
        {
            switch (targetType)
            {
                case TargetType.Enemy:

                    if (Physics2D.OverlapCircle(self.transform.position, range, targetLayer) != null)
                        target = Physics2D.OverlapCircle(self.transform.position, range, targetLayer).gameObject;
                    break;
                case TargetType.Player:

                    target = PlayerManager.Instance.GetPlayer().gameObject;
                    break;
                case TargetType.Mouse:

                    target = new GameObject();
                    break;
            }
        }
        if (target != null)
        {
            switch (targetType)
            {
                case TargetType.Enemy:
                    CommonTracking(self);
                    break;
                case TargetType.Player:
                    CommonTracking(self);
                    break;
                case TargetType.Mouse:
                    MouseMovent(self);
                    break;
            }

        }
        return index;

    }


    private void MouseMovent(GameObject self)
    {
        target.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = target.transform.position - self.transform.position;
        Vector3 aimV3 = direction.normalized;
        float angle = Vector2.Angle(self.transform.right, aimV3);
        if (steeringSpeed >= 0.1f)
        {
            float needtime = angle / steeringSpeed;
            self.transform.right = Vector2.Lerp(self.transform.right, aimV3, Time.deltaTime / needtime).normalized;
        }
    }

    public void CommonTracking(GameObject self)
    {
        Vector2 direction = target.transform.position - self.transform.position;
        Vector3 aimV3 = direction.normalized;
        float angle = Vector2.Angle(self.transform.right, aimV3);
        if (steeringSpeed >= 0.1f)
        {
            float needtime = angle / steeringSpeed;
            self.transform.right = Vector2.Lerp(self.transform.right, aimV3, Time.deltaTime / needtime).normalized;
        }
    }

}
