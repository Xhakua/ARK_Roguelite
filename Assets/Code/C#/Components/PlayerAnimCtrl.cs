using UnityEngine;

public class PlayerAnimCtrl : MonoBehaviour
{
    private Animator animator;

    private int SpeedHash = Animator.StringToHash("Speed");
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetFloat(SpeedHash, Vector3.Magnitude(PlayerManager.Instance.GetPlayer().rb.velocity));


    }


}