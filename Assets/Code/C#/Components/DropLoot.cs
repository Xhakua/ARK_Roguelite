using UnityEngine;
/// <summary>
/// ���������࣬�����������������߼�
/// �����player����ʰȡ��������
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
