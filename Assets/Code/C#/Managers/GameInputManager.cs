using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }
    public enum InputType
    {
        None = 0,
        Move = 1,
        Attack = 2,
        Skill = 3
    }

    public struct InputCommand
    {
        public InputType Type;
        public Vector3 Direction;
        public int Priority;
    }

    //private InputType inputType;
    private PlayerControls playerControls;

    public event EventHandler OnLeftDown;

    //public event EventHandler OnLeftUp;
    public event EventHandler OnRightDown;
    public event EventHandler OnMouseUp;
    //public event EventHandler OnRightUp;
    public event EventHandler OnInteraction;
    public event EventHandler OnLight;
    public event EventHandler OnMenu;
    public event EventHandler OnTurnLeft;
    public event EventHandler OnTurnRight;
    public event EventHandler<Vector2> OnAim;
    public event EventHandler<Vector2> OnSwitchItem;

    protected void Awake()
    {
        Debug.Log("Awake");
        if (Instance == null)
        {
            Instance = this;
            playerControls = new PlayerControls();
            //inputType = InputType.Common;
            playerControls.Enable();
            playerControls.Commonactionmap.Mouse.performed += Mouse_performed;
            playerControls.Commonactionmap.Mouse.canceled += Mouse_canceled;
            playerControls.Commonactionmap.Interaction.performed += Interaction_performed;
            playerControls.Commonactionmap.Light.performed += Light_performed;
            playerControls.Commonactionmap.Menu.performed += Menu_performed;
            playerControls.Commonactionmap.TurnVision.performed += TurnVision_performed;
            playerControls.Commonactionmap.Aim.performed += Aim_performed;
            playerControls.Commonactionmap.SwitchItem.performed += SwitchItem_performed;


        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SwitchItem_performed(InputAction.CallbackContext context)
    {
        OnSwitchItem?.Invoke(this, playerControls.Commonactionmap.SwitchItem.ReadValue<Vector2>());
    }

    public void Aim_performed(InputAction.CallbackContext context)
    {
        OnAim?.Invoke(this, playerControls.Commonactionmap.Aim.ReadValue<Vector2>());
    }

    private void TurnVision_performed(InputAction.CallbackContext context)
    {
        if (playerControls.Commonactionmap.TurnVision.ReadValue<float>() > 0)
        {
            OnTurnRight?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnTurnLeft?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Menu_performed(InputAction.CallbackContext context)
    {
        OnMenu?.Invoke(this, EventArgs.Empty);
    }

    private void Light_performed(InputAction.CallbackContext context)
    {
        OnLight?.Invoke(this, EventArgs.Empty);
    }

    private void Interaction_performed(InputAction.CallbackContext context)
    {
        OnInteraction?.Invoke(this, EventArgs.Empty);
    }

    private void Mouse_canceled(InputAction.CallbackContext context)
    {

        OnMouseUp?.Invoke(this, EventArgs.Empty);
    }

    private void Mouse_performed(InputAction.CallbackContext context)
    {
        if (playerControls.Commonactionmap.Mouse.ReadValue<float>() < 0)
        {
            OnLeftDown?.Invoke(this, EventArgs.Empty);
            Debug.Log("LeftDown" + playerControls.Commonactionmap.Mouse.ReadValue<float>());
        }
        else
        {
            OnRightDown?.Invoke(this, EventArgs.Empty);
            Debug.Log("RightDown" + playerControls.Commonactionmap.Mouse.ReadValue<float>());
        }
    }

    public Vector3 GetMovement()
    {
        Vector3 movement = playerControls.Commonactionmap.Movement.ReadValue<Vector2>();
        movement = new Vector3(movement.x, 0, movement.y);
        if (movement != Vector3.zero)
        {
            OnAim?.Invoke(this, playerControls.Commonactionmap.Aim.ReadValue<Vector2>());
        }
        return movement;
    }

}
