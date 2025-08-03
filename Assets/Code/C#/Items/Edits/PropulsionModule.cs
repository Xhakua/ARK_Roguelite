using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropulsionModule : MonoBehaviour,IAble2Edit
{
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        self.transform.position +=15 * Time.deltaTime * self.transform.right;
        return index;
        
    }


}
