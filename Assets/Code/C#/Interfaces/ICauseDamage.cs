/// <summary>
/// 能够造成伤害的接口
/// </summary>
public interface ICauseDamage
{
    /// <summary>
    /// 设置伤害值
    /// </summary>
    /// <param name="reactionsBuff"></param>
    public void SetReactionsBuff(ReactionsBuff reactionsBuff);
}
