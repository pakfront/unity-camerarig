using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraRig))]
public class CameraRigEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        CameraRig example = (CameraRig)target;

        EditorGUI.BeginChangeCheck();
        Handles.matrix = example.transform.localToWorldMatrix;
        Vector3 dollyPosition = Handles.PositionHandle(new Vector3(example.truck, 0, example.dolly), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(example, "Change Dolly Position");
            example.truck = dollyPosition.x;
            example.dolly = dollyPosition.z;
            example.Apply();
        }
        Handles.matrix = Matrix4x4.identity;


        // EditorGUI.BeginChangeCheck();
        // Vector3 newTargetPosition = Handles.PositionHandle(example.boomHead, Quaternion.identity);
        // if (EditorGUI.EndChangeCheck())
        // {
        //     dollyPosition.y = example.dolly.y;
        //     Undo.RecordObject(example, "Change Look At Target Position");
        //     example.dolly = dollyPosition;
        //     example.Apply();
        // }


    }
}