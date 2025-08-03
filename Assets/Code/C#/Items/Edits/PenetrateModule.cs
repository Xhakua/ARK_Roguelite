using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrateModule : MonoBehaviour, IAble2Edit
{
    public int penetrateNum;
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        self.GetComponent<EditableBullet>().penetrateNum = this.penetrateNum;
        return index;
    }

}
