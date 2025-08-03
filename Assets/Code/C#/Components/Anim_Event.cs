using UnityEngine;


/// <summary>
/// �����¼��࣬�������ݻ�����Ĺ���
/// </summary>
public class Anim_Event : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
    
    public void SetParentActiveFalse()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
