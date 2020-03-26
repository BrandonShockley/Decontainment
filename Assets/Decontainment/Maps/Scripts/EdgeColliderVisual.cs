using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

[RequireComponent(typeof(EdgeCollider2D), typeof(LineRenderer))]
public class EdgeColliderVisual : MonoBehaviour
{
    [SerializeField]
    private bool validate = false;
    [SerializeField]
    private GameObject cornerPrefab = null;

    private EdgeCollider2D ec;
    private LineRenderer lr;

    #if UNITY_EDITOR
    void OnValidate()
    {
        // A bunch of weird code just to create gameobjects during OnValidate in a prefab
        validate = false;
        if (Application.isPlaying || EditorApplication.timeSinceStartup == 0) {
            return;
        }

        bool thing = validate;
        ec = GetComponent<EdgeCollider2D>();
        lr = GetComponent<LineRenderer>();

        foreach (Transform child in transform) {
            EditorApplication.CallbackFunction callback = () =>
            {
                if (child != null && !EditorUtility.IsPersistent(child) && PrefabStageUtility.GetCurrentPrefabStage() != null) {
                    DestroyImmediate(child.gameObject, true);
                }
            };
            EditorApplication.delayCall += () =>
            {
                callback();
                EditorApplication.delayCall -= callback;
            };
        }

        Vector3[] points3D = new Vector3[ec.pointCount - 1];
        for (int i = 0; i < ec.pointCount - 1; ++i) {
            int iCopy = i;
            points3D[i] = ec.points[i];
            EditorApplication.CallbackFunction callback = () =>
            {
                if (this != null && !EditorUtility.IsPersistent(transform) && PrefabStageUtility.GetCurrentPrefabStage() != null) {
                    GameObject go = Instantiate(cornerPrefab, transform.TransformPoint(ec.points[iCopy]), Quaternion.identity, transform);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(go);
                    EditorUtility.SetDirty(go);
                }
            };
            EditorApplication.delayCall += () =>
            {
                callback();
                EditorApplication.delayCall -= callback;
            };
        }
        lr.positionCount = points3D.Length;
        lr.SetPositions(points3D);
    }
    #endif
}
