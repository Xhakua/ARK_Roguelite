using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterModule : MonoBehaviour, IAble2Edit
{
    public int Edit(GameObject self, InventorySO inventory, int index)
    {
        //BulletManager.Instance.GenerateBullet(inventory.GetGameObject(index + 1), self.transform);
        //return index;
        return 0;
    }


}
