using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(GravityField))]
public class GravityFieldGizmo : MonoBehaviour
{
    [SerializeField] Color gizmoColor = new(0.3f, 0.6f, 1f, 0.3f);
    GravityField field;

    void OnDrawGizmos()
    {
        field ??= GetComponent<GravityField>();
        if (field == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(field.transform.position, field.Radius);
    }
}
