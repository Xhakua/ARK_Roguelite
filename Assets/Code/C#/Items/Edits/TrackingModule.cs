using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackingModule : MonoBehaviour, IAble2Edit, IAble2BagInteraction_Scrollbar, IAble2BagInteraction_DropDown, IApplyPlayerPrefsData
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
    [SerializeField, Range(0f, 60f)]
    private float trackingSpeed = 0.0f;
    [SerializeField, Range(0f, 360f)]
    private float rotationSpeed = 0.0f;
    [SerializeField] private LayerMask targetLayer;
    //private Vector2 ctrlpos;
    //private float distance;

    public Dictionary<string, float> GetScrollbarMultipliers()
    {
        Dictionary<string, float> dict = new Dictionary<string, float>
        {
            { "trackingSpeed", 60 },
            { "rotationSpeed", 360 }
        };
        return dict;
    }

    public void SetScrollbarValues(float[] values)
    {
        trackingSpeed = values[0];
        rotationSpeed = values[1];
    }

    public void ApplyData()
    {
        trackingSpeed = PlayerPrefs.GetFloat("trackingSpeed") * 60;
        rotationSpeed = PlayerPrefs.GetFloat("rotationSpeed") * 360;
        targetType = (TargetType)PlayerPrefs.GetInt("TrackingModule");

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
            self.transform.position += Time.deltaTime * trackingSpeed * self.transform.right;
        }
        if (target != null)
        {
            switch (targetType)
            {
                case TargetType.Enemy:
                    CommonTracking(self);
                    break;
                case TargetType.Player:
                    PlayerTrackig(self);
                    break;
                case TargetType.Mouse:
                    MouseMovent(self);
                    break;
            }

        }
        else
        {
            CommonMovent(self);
        }
        return index;

    }

    private void CommonMovent(GameObject self)
    {
        self.transform.position += Time.deltaTime * trackingSpeed * self.transform.right;
    }

    private void MouseMovent(GameObject self)
    {
        target.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = target.transform.position - self.transform.position;
        Vector3 aimV3 = direction.normalized;
        float angle = Vector2.Angle(self.transform.right, aimV3);
        if (rotationSpeed >= 0.1f)
        {
            float needtime = angle / rotationSpeed;
            self.transform.right = Vector2.Lerp(self.transform.right, aimV3, Time.deltaTime / needtime).normalized;
        }
        self.transform.position += Time.deltaTime * trackingSpeed * self.transform.right;
    }

    public void CommonTracking(GameObject self)
    {
        Vector2 direction = target.transform.position - self.transform.position;
        Vector3 aimV3 = direction.normalized;
        float angle = Vector2.Angle(self.transform.right, aimV3);
        if (rotationSpeed >= 0.1f)
        {
            float needtime = angle / rotationSpeed;
            self.transform.right = Vector2.Lerp(self.transform.right, aimV3, Time.deltaTime / needtime).normalized;
        }
        self.transform.position += Time.deltaTime * trackingSpeed * self.transform.right;
    }

    private Vector3 add;
    public void PlayerTrackig(GameObject self)
    {
        if (add == Vector3.zero)
            InvokeRepeating(nameof(PlayerPosAdd), 0, 1);
        Vector2 direction = target.transform.position + add - self.transform.position;
        float distance = Vector2.Distance(target.transform.position + add, self.transform.position);
        if (distance > 1)
        {
            Vector3 aimV3 = direction.normalized;
            self.transform.right = aimV3;
            self.transform.position += Time.deltaTime * trackingSpeed * self.transform.right;
        }


    }

    private void PlayerPosAdd()
    {
        add = UnityEngine.Random.insideUnitCircle * 8;
    }

}