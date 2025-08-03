using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterEffectModule : MonoBehaviour, IAble2Edit
{
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        return index;
    }
}
