using System.Collections.Generic;
using UnityEngine;

public class WalkablePolygon : MonoBehaviour
{
    public List<Vector2> polygonPoints = new List<Vector2>();
    
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (polygonPoints.Count > 2)
        {
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Vector3 start = new Vector3(polygonPoints[i].x, 0, polygonPoints[i].y);
                Vector3 end = new Vector3(polygonPoints[(i + 1) % polygonPoints.Count].x, 0, polygonPoints[(i + 1) % polygonPoints.Count].y);
                Gizmos.DrawLine(start, end);
            }
        }
    }

    public void ClampPosition(ref Vector3 velocity, ref Vector3 newPosition)
    {
        if (polygonPoints.Count < 3) return;

        Vector2 position2D = new Vector2(newPosition.x, newPosition.z);

        if (!IsPointInPolygon(position2D))
        {
            Vector2 closestPoint = ClosestPointOnPolygon(position2D);
            newPosition = new Vector3(closestPoint.x, newPosition.y, closestPoint.y);
            
            // Calculate slide velocity
            Vector2 normal = (position2D - closestPoint).normalized;
            Vector2 velocity2D = new Vector2(velocity.x, velocity.z);
            Vector2 slideVelocity = Vector2.Dot(velocity2D, normal) * normal;
            velocity = new Vector3(velocity.x - slideVelocity.x, velocity.y, velocity.z - slideVelocity.y);
        }
    }

    private bool IsPointInPolygon(Vector2 point)
    {
        bool inside = false;
        for (int i = 0, j = polygonPoints.Count - 1; i < polygonPoints.Count; j = i++)
        {
            if (((polygonPoints[i].y > point.y) != (polygonPoints[j].y > point.y)) &&
                (point.x < (polygonPoints[j].x - polygonPoints[i].x) * (point.y - polygonPoints[i].y) / (polygonPoints[j].y - polygonPoints[i].y) + polygonPoints[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private Vector2 ClosestPointOnPolygon(Vector2 point)
    {
        float minDistance = float.MaxValue;
        Vector2 closestPoint = Vector2.zero;

        for (int i = 0; i < polygonPoints.Count; i++)
        {
            Vector2 start = polygonPoints[i];
            Vector2 end = polygonPoints[(i + 1) % polygonPoints.Count];
            Vector2 closest = ClosestPointOnLineSegment(point, start, end);
            float distance = Vector2.Distance(point, closest);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = closest;
            }
        }

        return closestPoint;
    }

    private Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 line = end - start;
        float lineLength = line.magnitude;
        line /= lineLength;

        float projectLength = Mathf.Clamp(Vector2.Dot(point - start, line), 0f, lineLength);
        return start + line * projectLength;
    }
}