/*
    Creates a Gizmo of a specified shape, size, and color on the object this script is attached to.
    I gave up trying to get the "frustum" shape working correctly.
*/

using UnityEngine;

public class Marker : MonoBehaviour
{
    enum MarkerShape
    {
        CUBE,
        SPHERE,
        WIRE_CUBE,
        WIRE_SPHERE,
        MESH,
        WIRE_MESH
    }

    [SerializeField] MarkerShape shape;
    [SerializeField] Color markerColor;
    [SerializeField][Range(0.0f, 1f)] float markerOpacity = 1f; // Unity defaults to 0 alpha for colors set in the color picker, this just forces it to 1 by default so you can see the marker initially.
    [SerializeField] float sphereSize = 1f;
    [SerializeField] Vector3 cubeSize = Vector3.one;
    [SerializeField] Mesh mesh;
    void OnDrawGizmos(){
        Gizmos.color = markerColor;
        markerColor.a = markerOpacity;
        switch (shape)
        {
            case MarkerShape.CUBE:
                Gizmos.DrawCube(transform.position, cubeSize);
                break;
            case MarkerShape.SPHERE:
                Gizmos.DrawSphere(transform.position, sphereSize);
                break;
            case MarkerShape.WIRE_CUBE:
                Gizmos.DrawWireCube(transform.position, cubeSize);
                break;
            case MarkerShape.WIRE_SPHERE:
                Gizmos.DrawWireSphere(transform.position, sphereSize);
                break;
            case MarkerShape.MESH:
                Gizmos.DrawMesh(mesh, transform.position);
                break;
            case MarkerShape.WIRE_MESH:
                Gizmos.DrawWireMesh(mesh, transform.position);
                break;
        }
    }
}
