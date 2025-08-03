using Spine.Unity;
using UnityEngine;

public class PlayerAnimCtrl : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Walk,
        Attack,
        Hit,
        Die
    }
    public AnimationState currentState = AnimationState.Idle;
    public SkeletonAnimation skeletonAnimation;
    public SkeletonRootMotion skeletonRootMotion;

    void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        skeletonRootMotion = GetComponentInChildren<SkeletonRootMotion>();
        skeletonRootMotion.rigidBody=PlayerManager.Instance.GetPlayer().rb;
    }


    private void LateUpdate()
    {
        if (PlayerManager.Instance.GetPlayer().rb.velocity == Vector3.zero)
        {
            if (currentState != AnimationState.Idle)
            {
                currentState = AnimationState.Idle;
                skeletonAnimation.AnimationName = "Relax";
            }
        }
        else
        {
            if (currentState != AnimationState.Walk)
            {
                currentState = AnimationState.Walk;
                skeletonAnimation.AnimationName = "Move";
            }
        }
    }

}

