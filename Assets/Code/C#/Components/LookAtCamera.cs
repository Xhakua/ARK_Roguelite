using UnityEngine;
/// <summary>
/// 面向摄像机组件
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        //看向摄像机
        LookAt,
        //看向摄像机反方向
        LookAtInverted,
        //摄像机前方
        CameraForward,
        //摄像机后方
        CameraForwardInverted
    }
    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
        Debug.Log(transform.rotation.eulerAngles);

    }
}