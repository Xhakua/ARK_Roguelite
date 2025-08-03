using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private Image iconImage;
    //[SerializeField] private Image lostHealthBarImage;
    //[SerializeField] private Image honkaiImpactBarImage2;
    //[SerializeField] private Image honkaiImpactBarImage1;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject hasHealthBarGameObject;
    [SerializeField] private DamageEnum2IMGsSO[] damageEnum2IMGsSOs;
    private ISetHealthUI hasProgress;
    private IReactionsUI hasBuff;
    private float hpMax;
    //private float MagicResistance;

    private void Start()
    {
        hasProgress = hasHealthBarGameObject.GetComponent<ISetHealthUI>();
        hasBuff = hasHealthBarGameObject.GetComponent<IReactionsUI>();

        hasProgress.OnHealthUIChanged += HasProgress_OnProgressChanged;
        hasBuff.OnBuffChanged += HasBuff_OnBuffChanged;
        barImage.fillAmount = 0f;
        hpMax = hasHealthBarGameObject.GetComponent<BaseEnemy>().GetHPMax();
        //MagicResistance = hasHealthBarGameObject.GetComponent<BaseEnemy>().GetHonkaiImpactPatience();
        Hide();
    }

    private void HasBuff_OnBuffChanged(object sender, IReactionsUI.OnBuffChangedEventArgs e)
    {
        foreach (var item in damageEnum2IMGsSOs)
        {
            if (item.damageEnum == e.buff.GetDamageEnum())
            {
                iconImage.sprite = item.sprite;
                countText.text = e.buff.GetCount().ToString();
                break;
            }
        }
    }

    private void HasProgress_OnProgressChanged(object sender, ISetHealthUI.OnProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.hp / hpMax;

        if ((barImage.fillAmount <= 0 || barImage.fillAmount >= 1f) && (countText.text == "" || countText.text == "0"))
        {

            Hide();
        }
        else
        {
            Show();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        transform.up = Camera.main.transform.up;
    }
}
