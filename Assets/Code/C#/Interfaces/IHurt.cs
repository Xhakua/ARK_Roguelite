using UnityEngine;
/// <summary>
/// 能够受到伤害的接口
/// </summary>
public interface IHurt
{
    public ReactionsBuff Hurt(ReactionsBuff reactionsBuffTakeDamage, GameObject source = null);
    public float GetHealth();
    public float GetMaxHealth();
}
