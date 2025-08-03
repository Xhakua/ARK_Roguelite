using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BaseBuff : MonoBehaviour, IGetDescription
{

    protected int id;
    //当前持续时间
    protected float duration;
    //最大持续时间
    protected float durationMax;
    //持续时间内触发的频率
    protected float frequency;
    protected Image buffIcon;
    //显示的CD遮罩
    protected Image buffProgress;
    //buff的目标
    protected GameObject target;
    protected bool display = false;



    protected IEnumerator BuffEffect()
    {
        while (duration > 0)
        {
            Effect();
            frequency = Mathf.Clamp(frequency, 0.1f, duration);

            yield return new WaitForSeconds(frequency);
            duration -= frequency;
            if (display)
                buffProgress.fillAmount = 1f - duration / durationMax;
        }
        if (duration <= 0)
        {
            StopBuff();
        }
    }


    public void StartBuff(GameObject target, BuffSO buffSO, bool display, int id)
    {
        this.id = id;
        this.target = target;
        this.duration = buffSO.durationMax;
        this.durationMax = buffSO.durationMax;
        this.frequency = buffSO.frequency;
        this.display = display;
        if (this.display)
        {
            this.buffIcon = GetComponentInParent<Image>();
            this.buffIcon.sprite = buffSO.sprite;
            buffProgress = transform.parent.GetChild(0).GetComponent<Image>();
        }

        StartEffect();
        StartCoroutine(BuffEffect());
    }

    public void StopBuff()
    {

        StopEffect();

        StopAllCoroutines();

        target.GetComponent<ISufferBuff>().GetBuffStructs().Remove(id);
        Destroy(transform.parent.gameObject);
        //Destroy(this);
        //gameObject.SetActive(false);
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public void Refresh()
    {
        StopAllCoroutines();
        StartCoroutine(BuffEffect());
    }

    public void ExtendBuff(float extendTime)
    {
        duration += extendTime;
        durationMax += extendTime;

    }

    protected virtual void StartEffect()
    {

    }

    protected virtual void Effect()
    {

    }
    protected virtual void StopEffect()
    {

    }

    public string GetDescription()
    {
        string description
              = BuffManager.Instance.buffListSO.buffList[id].description;
        return description;
    }
}


//备忘录
//public void StartBuff(GameObject target, float duration, float frequency, Sprite sprite, bool display, int id)
//{
//    this.id = id;

//    this.target = target;

//    this.duration = duration;
//    this.durationMax = duration;
//    this.frequency = frequency;

//    this.display = display;
//    if (this.display)
//    {
//        this.buffIcon = GetComponentInParent<Image>();
//        this.buffIcon.sprite = sprite;
//        buffProgress = transform.parent.GetChild(0).GetComponent<Image>();
//    }

//    StartEffect();
//    StartCoroutine(BuffEffect());
//}

//public void StartBuff(GameObject target, Sprite sprite, bool display, int id)
//{
//    this.id = id;
//    this.target = target;
//    duration = durationMax;
//    this.display = display;
//    if (this.display)
//    {
//        this.buffIcon = GetComponentInParent<Image>();
//        this.buffIcon.sprite = sprite;
//        buffProgress = transform.parent.GetChild(0).GetComponent<Image>();
//    }

//    StartEffect();
//    StartCoroutine(BuffEffect());
//}