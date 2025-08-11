using Spine.Unity;
using System;
using UnityEngine;

/// <summary>
/// Spine�����������л�����
/// </summary>
public class PlayerAnimCtrl : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Walk,
        Attack_Loop,
        Hit,
        Die
    }
    public AnimationState currentState = AnimationState.Idle;

    // ֧�ֶ�SkeletonAnimation�л�
    public SkeletonAnimation[] skeletonAnimations = new SkeletonAnimation[2];
    public int currentAnimIndex = 0;

    void Start()
    {
        for (int i = 0; i < skeletonAnimations.Length; i++)
        {
            if (skeletonAnimations[i] == null)
            {
                skeletonAnimations[i] = transform.GetChild(0).GetChild(i).GetComponent<SkeletonAnimation>();
                if (skeletonAnimations[i] == null)
                    Debug.LogError("SkeletonAnimation component not found in children.");
            }
        }
        // Ĭ��ʹ�õ�һ��SkeletonAnimation
        for (int i = 0; i < skeletonAnimations.Length; i++)
        {
            if (skeletonAnimations[i] != null)
                skeletonAnimations[i].gameObject.SetActive(i == currentAnimIndex);
        }


        GameInputManager.Instance.OnLeftDown += Instance_OnLeftDown;
        GameInputManager.Instance.OnMouseUp += Instance_OnMouseUp;
    }

    private void Instance_OnMouseUp(object sender, EventArgs e)
    {
        if(currentAnimIndex == 1)
        {
            SwitchAnim(0, "Relax", true);
        }
    }

    private void Instance_OnLeftDown(object sender, System.EventArgs e)
    {
        SwitchAnim(1, "Attack_Loop", true);
    }

    /// <summary>
    /// �л�����������������ָ������
    /// </summary>
    public void SwitchAnim(int index, string animName, bool loop = true)
    {
        if (index < 0 || index >= skeletonAnimations.Length)
        {
            Debug.LogError("Index out of bounds for skeletonAnimations array.");
            return;
        }
        if (skeletonAnimations[currentAnimIndex] != null)
            skeletonAnimations[currentAnimIndex].gameObject.SetActive(false);

        currentAnimIndex = index;
        if (skeletonAnimations[currentAnimIndex] != null)
        {
            skeletonAnimations[currentAnimIndex].gameObject.SetActive(true);
            skeletonAnimations[currentAnimIndex].state.SetAnimation(0, animName, loop);
        }
    }

    private void LateUpdate()
    {
        if (currentAnimIndex == 1)
        {
            return;
        }
        if (PlayerManager.Instance.GetPlayer().rb.velocity == Vector3.zero)
        {
            if (currentState != AnimationState.Idle)
            {
                currentState = AnimationState.Idle;
                skeletonAnimations[currentAnimIndex].AnimationName = "Relax";
            }
        }
        else
        {
            if (currentState != AnimationState.Walk)
            {
                currentState = AnimationState.Walk;
                skeletonAnimations[currentAnimIndex].AnimationName = "Move";
            }
        }
    }
}