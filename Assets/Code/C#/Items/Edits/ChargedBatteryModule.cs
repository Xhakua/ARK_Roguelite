using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedBatteryModule : MonoBehaviour, IAble2Edit
{
    [SerializeField] private float energy;
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        EditableBullet editableBullet = self.GetComponent<EditableBullet>();
        editableBullet.printTimer += energy;
        return index;
    }

}
