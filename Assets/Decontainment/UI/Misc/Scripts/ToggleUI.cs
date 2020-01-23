using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Assumes this GameObject doesn't use a canvas group
public class ToggleUI : MonoBehaviour
{
    [SerializeField]
    private KeyCode toggleKey = default;

    private CanvasGroup cg;

    void Awake()
    {
        cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) {
            cg.enabled = !cg.enabled;
        }
    }
}
