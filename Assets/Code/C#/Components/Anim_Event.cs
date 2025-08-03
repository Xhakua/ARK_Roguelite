using UnityEngine;


/// <summary>
/// 动画事件类，给动画摧毁物体的功能
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
