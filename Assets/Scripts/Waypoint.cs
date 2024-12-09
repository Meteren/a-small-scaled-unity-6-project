using UnityEditor;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private float radius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
