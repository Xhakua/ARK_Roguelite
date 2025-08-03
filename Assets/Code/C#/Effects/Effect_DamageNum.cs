using TMPro;
using UnityEngine;
/// <summary>
/// 伤害飘字效果组件
/// </summary>
public class Effect_DamageNum : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    private ReactionsBuff reactionsBuff;

    private void OnEnable()
    {
        Init();
    }

    public void SetReactionsBuff(ReactionsBuff reactionsBuff)
    {
        this.reactionsBuff = reactionsBuff;
    }

    private void Init()
    {
        damageText.text = reactionsBuff.GetDamage().ToString();
        switch (reactionsBuff.GetDamageEnum())
        {
            case ReactionsBuff.DamageEnum.electric:
                damageText.color = Color.yellow;
                break;
            case ReactionsBuff.DamageEnum.metal:
                damageText.color = Color.gray;
                break;
            case ReactionsBuff.DamageEnum.normal:
                damageText.color = Color.white;
                break;
            case ReactionsBuff.DamageEnum.highTemperature:
                damageText.color = Color.red;
                break;
            case ReactionsBuff.DamageEnum.lowTemperature:
                damageText.color = Color.blue;
                break;
            case ReactionsBuff.DamageEnum.honkai:
                damageText.color = Color.black;
                break;
        }
    }

}
