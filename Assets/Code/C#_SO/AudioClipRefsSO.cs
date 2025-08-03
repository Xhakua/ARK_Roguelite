using UnityEngine;
[CreateAssetMenu(fileName = "New AudioClipRefs", menuName = "Audio Clip Refs")]

public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip[] player_movement;
    public AudioClip[] player_meleeAttack;
    public AudioClip[] player_rangedAttack;

    public AudioClip[] hit_metal;
    public AudioClip[] hit_meat;
    public AudioClip[] drill;
    public AudioClip[] block_destroy;


}
