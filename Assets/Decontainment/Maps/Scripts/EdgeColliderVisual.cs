using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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
        bool thing = validate;
        validate = false;
        ec = GetComponent<EdgeCollider2D>();
        lr = GetComponent<LineRenderer>();

        foreach (Transform child in transform) {
            EditorApplication.CallbackFunction callback = () =>
            {
                if (child != null) {
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
                if (this != null && !EditorUtility.IsPersistent(transform)) {
                    Instantiate(cornerPrefab, transform.TransformPoint(ec.points[iCopy]), Quaternion.identity, transform);
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
