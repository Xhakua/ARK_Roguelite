using UnityEngine;

public class DestroyAroundCubeGene : MonoBehaviour, IGeneEffect
{
    [SerializeField] protected ItemSO itemSO;
    RaycastHit hit;
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask destroyableLayer;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 offset;
    private LineRenderer lineRenderer;

    private void OnEnable()
    {
        ApplyItemModifiers();
    }

    private void ApplyItemModifiers()
    {
        transform.position += offset;
    }
    public void Effect()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * radius);
        if (Physics.Raycast(transform.position, transform.forward, out hit, radius, destroyableLayer))
        {
            lineRenderer.SetPosition(1, hit.point);
            MapDynamicLoadingManager.Instance.DestroyCube(new Vector2Int((int)hit.collider.transform.position.x, (int)hit.collider.transform.position.z));
            MapManager.Instance.SetWorldData(new Vector3Int((int)hit.collider.transform.position.x, (int)hit.collider.transform.position.z, 0), MapManager.TileLayer.Wall, 0);
        }
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    public void StartEffect()
    {

    }

    public void StopEffect()
    {

    }

    void IGeneEffect.EffectOnTakeDamager(Transform TakeDamagerTarget, Transform TakeDamagerSource)
    {
        throw new System.NotImplementedException();
    }

    void IGeneEffect.EffectOnHurt()
    {
        throw new System.NotImplementedException();
    }

    void IGeneEffect.EffectOnDeath()
    {
        throw new System.NotImplementedException();
    }
}
