using System.Collections.Generic;
using UnityEngine;
public class EditableBullet : BaseBullet, IAble2BagInteraction_Grid
{
    [SerializeField] private InventorySO inventory;
    private Transform moduleContainer;
    private List<GameObject> instances = new List<GameObject>();
    public int penetrateNum;
    public int index;
    public float printTimer;
    public Collider2D[] colliders;
    public List<int> penetratedInstanceIDs = new List<int>();
    public InventorySO BagInteraction()
    {
        return inventory;
    }

    private new void OnEnable()
    {
        if (transform.parent != null)
        {
            return;
        }
        base.OnEnable();
        moduleContainer = transform.GetChild(0);
        penetratedInstanceIDs.Clear();
        colliders = new Collider2D[0];

        printTimer = 0;

        Init();
        ApplyEditData();

    }

    private void ApplyEditData()
    {
        for (int i = 0; instances.Count > i; i++)
        {
            if (instances[i].TryGetComponent(out IApplyPlayerPrefsData apply))
            {
                apply.ApplyData();
            }
        }
    }



    private void FixedUpdate()
    {
        LifeTime();
        ImplementEditing();
        Hit();
    }



    public void Init()
    {
        instances.Clear();
        //onDisable.Clear();
        for (int i = 0; inventory.items.Count > i; i++)
        {
            if (inventory.IsNull(i) == true)
                continue;
            Debug.Log("inventory.items[i]:" + inventory.items[i]);
            GameObject go = ModuleManager.Instance.GenerateModule(inventory.items[i], moduleContainer, true);
            instances.Add(go);
        }
    }

    public void ImplementEditing()
    {
        for (index = 0; instances.Count > index; index++)
        {
            if (instances[index].TryGetComponent(out IAble2Edit edit))
            {
                index = edit.Edit(gameObject, inventory, index);//怎么new一个新的,多个打印机都是调用这一个,但是他只能回应一个,可能是inventory的原因,试试在update[i]建新的IcanEdit;
            }
        }
    }

    private void OnDisable()
    {
        foreach (GameObject go in instances)
        {
            go.transform.SetParent(null);
            go.SetActive(false);
        }
    }
    public void Hit()
    {

        colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (penetratedInstanceIDs.Count < penetrateNum)
                {
                    if (penetratedInstanceIDs.Contains(collider.gameObject.GetInstanceID()))
                    {
                        continue;
                    }
                    penetratedInstanceIDs.Add(collider.gameObject.GetInstanceID());
                }
                else
                {
                    lifeTimer = -10000;
                }
            }
            else
            {
                lifeTimer = -10000;
            }

        }
    }
}
