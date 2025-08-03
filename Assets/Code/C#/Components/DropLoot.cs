using UnityEngine;
/// <summary>
/// 掉落物体类，负责掉落物体的下落逻辑
/// 有这个player才能拾取掉落物体
/// </summary>
public class DropLoot : MonoBehaviour
{
    private LayerMask layerMaskGround;
    private LayerMask oriLayerMask;
    private ItemSO ItemSO;
    private void OnEnable()
    {
        oriLayerMask = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Loot");
    }

    public ItemSO GetItemSO()
    {
        return ItemSO;
    }

    public void SetItemSO(ItemSO itemSO)
    {
        ItemSO = itemSO;
    }

    public void SetLayerMask(LayerMask layerMask)
    {
        this.layerMaskGround = layerMask;
    }

    private void OnDisable()
    {
        gameObject.layer = oriLayerMask;
        Destroy(this.gameObject.GetComponent<DropLoot>());
    }
}
