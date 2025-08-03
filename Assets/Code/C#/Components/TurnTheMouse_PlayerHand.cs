using System;
using UnityEngine;

/// <summary>
/// 鼠标控制玩家手部方向组件
/// </summary>
public class TurnTheMouse_PlayerHand : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 mousePos;
    private Vector3 aimDir;
    private float angle;
    private float offset_X;
    private float offset_Y;
    private bool recoil = false;
    private float force = 0;
    private Rigidbody rb;

    public Vector2 weaponDirection;

    protected void Start()
    {
        mainCamera = Camera.main;
       
        rb = transform.parent.GetComponent<Rigidbody>();
        GameInputManager.Instance.OnAim += GameInputManager_OnAim;
    }

    private void GameInputManager_OnAim(object sender, Vector2 e)
    {
        if(mainCamera == null)
        {
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(e);
        Plane groundPlane = new Plane(Vector3.forward, PlayerManager.Instance.GetPlayer().transform.position);
        float enter = 0f;

        if (groundPlane.Raycast(ray, out enter))
        {
            mousePos = ray.GetPoint(enter);
            mousePos += (new Vector3(offset_X, offset_Y, 0) * UnityEngine.Random.Range(-5, 5) * 0.2f);
            weaponDirection = mousePos - PlayerManager.Instance.GetPlayer().transform.position;
            transform.right = weaponDirection;
            PlayerManager.Instance.GetPlayer().SetFaceDir(weaponDirection.x);
            transform.localScale = new Vector3(1, Mathf.Sign(weaponDirection.x), 1);
            offset_X = 0;
            offset_Y = 0;
            force = 0;
        }

    }




    public void SetRecoil(float recoilMin, float recoilMax, float force, float recoilResistance)
    {
        this.offset_Y = recoilMin * (1 - recoilResistance);
        this.offset_X = recoilMax * (1 - recoilResistance);
        this.force = force * (1 - recoilResistance);
        GameInputManager.Instance.Aim_performed(new UnityEngine.InputSystem.InputAction.CallbackContext());
    }
    private void OnDestroy()
    {
        GameInputManager.Instance.OnAim -= GameInputManager_OnAim;
    }

}
//private void TurnTheMouseManager()
//{

//    if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || recoil)
//    {
//        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
//        aimDir = (mousePos - transform.position).normalized;
//        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

//        PlayerManager.Instance.GetPlayer().SetFaceDir(MathF.Sign(Input.mousePosition.x - Screen.currentResolution.width * 0.5f));
//        //后座力
//        if (Mathf.Abs(angle) < 90)
//        {


//            angle += UnityEngine.Random.Range(offset_Y, offset_X);
//            transform.eulerAngles = new Vector3(0, 0, angle);
//            rb.AddForce(-aimDir * force, ForceMode.Impulse);
//            transform.localScale = new Vector3(1, 1, 1);
//        }
//        else
//        {


//            angle += UnityEngine.Random.Range(offset_Y, offset_X);
//            transform.eulerAngles = new Vector3(0, 0, angle);
//            rb.AddForce(-aimDir * force, ForceMode.Impulse);
//            transform.localScale = new Vector3(1, -1, 1);
//        }

//        recoil = false;
//        offset_X = 0;
//        offset_Y = 0;
//        force = 0;
//    }
//}