using UnityEngine;
/// <summary>
/// �ܹ��ܵ��˺��Ľӿ�
/// </summary>
public interface IHurt
{
    public ReactionsBuff Hurt(ReactionsBuff reactionsBuffTakeDamage, GameObject source = null);
    public float GetHealth();
    public float GetMaxHealth();
}
