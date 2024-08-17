using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WalkablePolygon : MonoBehaviour
{
    public List<Vector2> polygonPoints = new();

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (polygonPoints.Count <= 2)
        {
            return;
        }
        for (int i = 0; i < polygonPoints.Count; i++)
        {
            var start = new Vector3(polygonPoints[i].x, 0, polygonPoints[i].y);
            var end = new Vector3(polygonPoints[(i + 1) % polygonPoints.Count].x, 0, polygonPoints[(i + 1) % polygonPoints.Count].y);
            Gizmos.DrawLine(start, end);
        }
    }

    public void ClampPosition(ref Vector3 velocity, ref Vector3 newPosition)
    {
        if (polygonPoints.Count < 3) return;

        var position2D = new Vector2(newPosition.x, newPosition.z);

        if (IsPointInPolygon(position2D))
        {
            return;
        }
        var closestPoint = ClosestPointOnPolygon(position2D);
        newPosition = new Vector3(closestPoint.x, newPosition.y, closestPoint.y);

        // Calculate slide velocity
        var normal = (position2D - closestPoint).normalized;
        var velocity2D = new Vector2(velocity.x, velocity.z);
        var slideVelocity = Vector2.Dot(velocity2D, normal) * normal;
        velocity = new Vector3(velocity.x - slideVelocity.x, velocity.y, velocity.z - slideVelocity.y);
    }

    private bool IsPointInPolygon(Vector2 point)
    {
        var inside = false;
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
        var minDistance = float.MaxValue;
        var closestPoint = Vector2.zero;

        for (int i = 0; i < polygonPoints.Count; i++)
        {
            var start = polygonPoints[i];
            var end = polygonPoints[(i + 1) % polygonPoints.Count];
            var closest = ClosestPointOnLineSegment(point, start, end);
            var distance = Vector2.Distance(point, closest);

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
        var line = end - start;
        var lineLength = line.magnitude;
        line /= lineLength;

        var projectLength = Mathf.Clamp(Vector2.Dot(point - start, line), 0f, lineLength);
        return start + line * projectLength;
    }

    [Button]
    private void CenterPolygon()
    {
        var centroid = CalculateCentroid(polygonPoints);
        CenterPointsAroundOrigin(centroid);
    }

    Vector2 CalculateCentroid(List<Vector2> points)
    {
        var centroid = Vector2.zero;
        foreach (var point in points)
        {
            centroid += point;
        }

        centroid /= points.Count;
        return centroid;
    }

    void CenterPointsAroundOrigin(Vector2 centroid)
    {
        for (var i = 0; i < polygonPoints.Count; i++)
        {
            polygonPoints[i] -= centroid;
        }
    }
}