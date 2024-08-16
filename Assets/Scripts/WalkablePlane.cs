using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkablePlane : MonoBehaviour
{
    [SerializeField]
    private Rect area = new Rect(-5f, -5f, 10f, 10f);

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(area.size.x, 0, area.size.y));
    }

    public void ClampPosition(ref Vector3 velocity, ref Vector3 newPosition)
    {
        if (newPosition.x < area.xMin)
        {
            newPosition.x = area.xMin;
            velocity.x = 0;
        }
        else if (newPosition.x > area.xMax)
        {
            newPosition.x = area.xMax;
            velocity.x = 0;
        }
        if (newPosition.z < area.yMin)
        {
            newPosition.z = area.yMin;
            velocity.z = 0;
        }
        else if (newPosition.z > area.yMax)
        {
            newPosition.z = area.yMax;
            velocity.z = 0;
        }
    }
}