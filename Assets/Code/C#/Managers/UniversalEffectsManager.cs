using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 通用特效管理器
/// </summary>
public class UniversalEffectsManager : MonoBehaviour
{
    public static UniversalEffectsManager Instance { get; private set; }
    public List<GameObject> effectlist = new List<GameObject>();
    [SerializeField] private GameObject boom;
    [SerializeField] private GameObject electricChains;
    [SerializeField] private GameObject broken;
    [SerializeField] private GameObject electric;
    [SerializeField] private GameObject damageNum;
    [SerializeField] private GameObject honkaiBloodSpatter;
    [SerializeField] private GameObject fireParticle;
    [SerializeField] private GameObject effect_Ice;
    [SerializeField] private GameObject electricBuff;
    [SerializeField] private GameObject bW;
    [SerializeField] private GameObject effect_destroy;
    [SerializeField] private GameObject effect_Afterimg;
    [SerializeField] private GameObject effect_Slash;
    [SerializeField] private GameObject effect_Fumes;
    [SerializeField] private GameObject effect_BulletHit;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject GenerateBulletHit(Transform transform, float lifetime, float strength)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_BulletHit.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = transform.rotation;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<Effect_HonkaiBloodSpatter>().SetState(strength);
                effectlist[i].SetActive(true);
                take = true;
                ret = effectlist[i];


                break;

            }
        }
        if (!take)
        {


            ret = Instantiate(effect_BulletHit, transform.position, transform.rotation);


            ret.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);

            ret.GetComponent<Effect_HonkaiBloodSpatter>().SetState(strength);
            ret.SetActive(true);
            effectlist.Add(ret);


        }
        return ret;
    }

    public GameObject GenerateSlash(Transform transform)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_Slash.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position + 0.5f * Random.insideUnitSphere;
                effectlist[i].transform.rotation = transform.rotation * Quaternion.Euler(0, 0, Random.Range(-30, 30));
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            effect_Slash.SetActive(false);
            ret = Instantiate(effect_Slash, transform.position, transform.rotation);
            effectlist.Add(ret);
            GenerateSlash(transform);
        }
        return ret;
    }

    public GameObject GenerateDamageNum(Transform transform, ReactionsBuff reactionsBuff)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == damageNum.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position + 0.5f * Random.insideUnitSphere;
                effectlist[i].transform.rotation = Quaternion.identity * Quaternion.Euler(0, 0, Random.Range(-30, 30));
                effectlist[i].GetComponent<Effect_DamageNum>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            damageNum.SetActive(false);
            damageNum.GetComponent<Effect_DamageNum>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(damageNum, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateDamageNum(transform, reactionsBuff);
        }
        return ret;

    }






    public GameObject GenerateElectricChains(Transform transform, ReactionsBuff reactionsBuff, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == electricChains.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<Effect_ElectricChains>().selflinkedInstanceIDs[0] = transform.gameObject.GetInstanceID();
                effectlist[i].GetComponent<Effect_ElectricChains>().selflinked[0] = transform.gameObject;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            electricChains.SetActive(false);
            electricChains.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            electricChains.GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(electricChains, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateElectricChains(transform, reactionsBuff, lifetime);
        }
        return ret;
    }


    public GameObject GenerateBroken(Transform transform, ReactionsBuff reactionsBuff, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == broken.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            broken.SetActive(false);
            broken.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            broken.GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(broken, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateBroken(transform, reactionsBuff, lifetime);
        }
        return ret;
    }



    public GameObject GenerateElectric(Transform transform, ReactionsBuff reactionsBuff, float lifetime)
    {

        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == electric.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {
                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            electric.SetActive(false);
            electric.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            electric.GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(electric, transform.position, Quaternion.identity);
            effectlist.Add(ret);

            GenerateElectric(transform, reactionsBuff, lifetime);
        }
        return ret;
    }

    public GameObject GenerateBoom(Transform transform, ReactionsBuff reactionsBuff, float lifetime)
    {
        bool take = false;
        GameObject ret = null;


        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (!effectlist[i].gameObject.activeSelf && effectlist[i].name == boom.name + "(Clone)")
            {
                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }


        if (!take)
        {
            boom.SetActive(false);
            boom.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            boom.GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(boom, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateBoom(transform, reactionsBuff, lifetime);
        }
        return ret;
    }

    public GameObject GenerateHonkaiBloodSpatter(Transform transform, float lifetime, float strength, float angle)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == honkaiBloodSpatter.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<Effect_HonkaiBloodSpatter>().SetState(angle, strength);
                effectlist[i].SetActive(true);
                take = true;
                ret = effectlist[i];


                break;

            }
        }
        if (!take)
        {


            ret = Instantiate(honkaiBloodSpatter, transform.position, Quaternion.identity);


            ret.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);

            ret.GetComponent<Effect_HonkaiBloodSpatter>().SetState(angle, strength);
            ret.SetActive(true);
            effectlist.Add(ret);


        }
        return ret;
    }

    public GameObject GenerateFireParticle(Transform transform, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == fireParticle.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                //effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            fireParticle.SetActive(false);
            //fireParticle.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            ret = Instantiate(fireParticle, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateFireParticle(transform, lifetime);
        }
        return ret;
    }

    public GameObject GenerateEffect_Ice(Transform transform, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_Ice.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                //effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            effect_Ice.SetActive(false);
            ret = Instantiate(effect_Ice, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_Ice(transform, lifetime);
        }
        return ret;
    }

    public GameObject GenerateEffect_ElectricBuff(Transform transform, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == electricBuff.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                //effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            electricBuff.SetActive(false);
            //fireParticle.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            ret = Instantiate(electricBuff, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_ElectricBuff(transform, lifetime);
        }
        return ret;
    }
    public GameObject GenerateEffect_BW(Transform transform, ReactionsBuff reactionsBuff, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == bW.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = transform.position;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            bW.SetActive(false);
            bW.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            bW.GetComponent<ICauseDamage>().SetReactionsBuff(reactionsBuff);
            ret = Instantiate(bW, transform.position, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_BW(transform, reactionsBuff, lifetime);
        }
        return ret;
    }

    public GameObject GenerateEffect_Destroy(Vector3 pos, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_destroy.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = pos;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            effect_destroy.SetActive(false);
            effect_destroy.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            ret = Instantiate(effect_destroy, pos, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_Destroy(pos, lifetime);
        }
        return ret;
    }
    public GameObject GenerateEffect_AfterIMG(Vector3 pos, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_Afterimg.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = pos;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            effect_Afterimg.SetActive(false);
            effect_Afterimg.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            ret = Instantiate(effect_Afterimg, pos, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_AfterIMG(pos, lifetime);
        }
        return ret;
    }

    public GameObject GenerateEffect_Fumes(Vector3 pos, float lifetime)
    {
        bool take = false;
        GameObject ret = null;
        for (int i = 0; i < effectlist.Count; i++)
        {
            if (effectlist[i] == null)
            {
                effectlist.RemoveAt(i);
                continue;
            }
            if (effectlist[i].name == effect_Fumes.name + "(Clone)" && !effectlist[i].gameObject.activeSelf)
            {

                effectlist[i].transform.position = pos;
                effectlist[i].transform.rotation = Quaternion.identity;
                effectlist[i].GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
                effectlist[i].gameObject.SetActive(true);
                take = true;
                ret = effectlist[i];
                break;

            }
        }
        if (!take)
        {
            effect_Fumes.SetActive(false);
            effect_Fumes.GetComponent<ISetLifeTime>().SetLifeTime(lifetime);
            ret = Instantiate(effect_Fumes, pos, Quaternion.identity);
            effectlist.Add(ret);
            GenerateEffect_Fumes(pos, lifetime);
        }
        return ret;
    }


}
