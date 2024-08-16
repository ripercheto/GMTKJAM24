using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WalkablePolygon))]
public class WalkablePolygonEditor : OdinEditor
{
    private void OnSceneGUI()
    {
        WalkablePolygon walkablePolygon = (WalkablePolygon)target;
        Handles.matrix = Matrix4x4.TRS(walkablePolygon.transform.position, walkablePolygon.transform.rotation, Vector3.one);
        // Iterate through each point and allow moving it
        for (int i = 0; i < walkablePolygon.polygonPoints.Count; i++)
        {
            Vector2 point = walkablePolygon.polygonPoints[i];
            Handles.color = Color.red;

            // Display a handle for each point
            Vector3 handlePosition = new Vector3(point.x, 0f, point.y);
            EditorGUI.BeginChangeCheck();
            handlePosition = Handles.PositionHandle(handlePosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move Point");
                walkablePolygon.polygonPoints[i] = new Vector2(handlePosition.x, handlePosition.z);
                EditorUtility.SetDirty(target);
            }

            // Draw the point as a label in the Scene view
            Handles.Label(handlePosition, $"Point {i}");
        }
    }
}