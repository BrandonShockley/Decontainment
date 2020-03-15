using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Assumes this GameObject doesn't use a canvas group
public class DebugToggleUI : MonoBehaviour
{
    [SerializeField]
    private KeyCode toggleKey = default;

    private bool disabled;

    private CanvasGroup cg;

    void Awake()
    {
        cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) {
            disabled = !disabled;

            cg.alpha = disabled ? 0 : 1;
            cg.interactable = !disabled;
            cg.blocksRaycasts = !disabled;
        }
    }
}
