using UnityEngine;
/// <summary>
/// “Ù∆µπ‹¿Ì∆˜
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefs;
    public static AudioManager Instance { get; private set; }
    [Range(0, 1)] public float bgmVolume = 0.5f;
    [Range(0, 1)] public float sfxVolume = 0.5f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMeleeAttackSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.player_meleeAttack, position);
    }
    public void PlayRangedAttackSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.player_rangedAttack, position);
    }
    public void PlayHitMetalSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.hit_metal, position);
    }
    public void PlayHitMeatSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.hit_meat, position);
    }
    public void PlayFootstepsSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.player_movement, position);
    }
    //public void PlayDrillSound(Vector3 position)
    //{
    //    PlaySFXSound(audioClipRefs.drill[0], position);
    //}

    public void PlayDrillingSound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.drill[1], position);
    }


    public void PlayBlockDestorySound(Vector3 position)
    {
        PlaySFXSound(audioClipRefs.block_destroy, position);
    }


    private void PlaySFXSound(AudioClip audioClip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, sfxVolume);
    }

    private void PlayBGMSound(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, bgmVolume);
    }



    private void PlaySFXSound(AudioClip[] audioClipArray, Vector3 position)
    {
        PlaySFXSound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position);
    }

}
