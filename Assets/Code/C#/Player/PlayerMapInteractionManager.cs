using System;
using UnityEngine;

public class PlayerMapInteractionManager : MonoBehaviour
{
    public static PlayerMapInteractionManager Instance { get; private set; }
    public float InteractionDistance = 10;
    public Vector3 offset;
    [SerializeField] private LayerMask interactionLayerMask;
    private GameObject lastInteraction;
    private GameObject currentInteraction;
    protected void Awake()
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

    protected void Start()
    {
        GameInputManager.Instance.OnInteraction += GameInputManager_OnInteraction;
    }

    protected void FixedUpdate()
    {
        RaycastHit raycastHit;
        Vector3 forward = GameInputManager.Instance.GetMovement();
        //Debug.DrawRay(transform.position+ offset, forward * InteractionDistance, Color.red);
        if (Physics.Raycast(transform.position + offset, forward, out raycastHit, InteractionDistance, interactionLayerMask))
        {
            //Debug.Log(raycastHit.collider.gameObject.name);
            lock (raycastHit.collider)
            {
                if (raycastHit.collider != null)
                {
                    currentInteraction = raycastHit.collider.gameObject;
                    if (lastInteraction != currentInteraction)
                    {
                        if (lastInteraction != null)
                        {
                            try
                            {
                                lastInteraction.GetComponentInParent<ModelOutline>().enabled = false;
                            }
                            catch (Exception)
                            {

                            }

                        }
                        try
                        {
                            currentInteraction.GetComponentInParent<ModelOutline>().enabled = true;
                        }
                        catch (Exception)
                        {

                        }


                        lastInteraction = currentInteraction;
                    }
                }
            }
        }
        else
        {
            if (lastInteraction != null)
            {
                try
                {
                    lastInteraction.GetComponentInParent<ModelOutline>().enabled = false;
                }
                catch (Exception)
                {

                }

                lastInteraction = null;
                currentInteraction = null;
            }
        }
    }
    private void GameInputManager_OnInteraction(object sender, EventArgs e)
    {
        currentInteraction?.GetComponentInParent<ICanMapInteraction>()?.OnMapInteraction();
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, GameInputManager.Instance.GetMovement() * InteractionDistance);
    }

    private void OnDestroy()
    {
        GameInputManager.Instance.OnInteraction -= GameInputManager_OnInteraction;
    }
}
