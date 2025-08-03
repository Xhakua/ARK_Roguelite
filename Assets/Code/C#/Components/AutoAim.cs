using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    public enum TargetType
    {
        Enemy,
    }

    private GameObject target;
    [SerializeField] private TargetType targetType;
    [SerializeField] private float range = 100.0f;
    //×îÐ¡Éä»÷½Ç¶È
    [SerializeField, UnityEngine.Range(0f, 360f)]
    private float targetRotation;
    [SerializeField, UnityEngine.Range(0f, 360f)]
    private float rotationSpeed = 0.0f;
    [SerializeField] private LayerMask targetLayer;
    private Vector3 aimDir;
    private float angle;
    private float offset_X;
    private float offset_Y;
    private bool recoil = false;
    private List<IGeneEffect> geneEffects = new List<IGeneEffect>();

    private void FixedUpdate()
    {
        switch (targetType)
        {
            case TargetType.Enemy:
                target = SeekTarget();
                break;
            default:
                target = null;
                break;
        }
        if (target != null)
        {
            TrunToTarget(target);

            aimDir = target.transform.position - transform.position;
            aimDir += new Vector3(offset_X, 0, offset_Y);
            angle = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg -90;
            if (angle < targetRotation)
            {
                if (geneEffects.Count == 0)
                {
                    geneEffects.Clear();
                    foreach (Transform child in transform)
                    {
                        child.TryGetComponent(out IGeneEffect iGeneEffect);
                        if (iGeneEffect != null)
                        {
                            geneEffects.Add(iGeneEffect);
                            iGeneEffect.StartEffect();

                        }
                    }
                }
            }

            transform.rotation = Quaternion.Euler(0, angle, 0);

        }
        foreach (IGeneEffect geneEffect in geneEffects)
        {
            geneEffect.Effect();
        }
    }
    private GameObject SeekTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayer);
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == gameObject) continue; 
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = collider.gameObject;
            }
        }
        return closestTarget;
    }
    public void SetRecoil(float recoil_Y, float recoil_X)
    {
        this.offset_Y = recoil_Y;
        this.offset_X = recoil_X;
    }
    private void TrunToTarget(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
