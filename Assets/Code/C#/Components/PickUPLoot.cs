using UnityEngine;

public class PickUPLoot : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private InventorySO inventorySO;
    [SerializeField] private float pickUpRadius;
    [SerializeField] private float pickUpSpeed;

    private void FixedUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpRadius, layerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].transform.parent && colliders[i].TryGetComponent(out DropLoot dorpLoot))
            {
                colliders[i].transform.position = Vector3.MoveTowards(colliders[i].transform.position, transform.position, pickUpSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(transform.position, colliders[i].transform.position) < 0.1f)
                {
                    if (inventorySO.AddItem(colliders[i].GetComponent<DropLoot>().GetItemSO()) >= 0)
                    {
                        InventoryManager.Instance.RefreshHandItem();
                        colliders[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
