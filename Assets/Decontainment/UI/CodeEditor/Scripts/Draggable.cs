using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool isDragging;
    private List<RectTransform> validParents;
    private Transform origParent;
    private Vector2 origAnchor;
    private Vector2 origAnchorPos;

    private Canvas canvas;
    private RectTransform rt;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rt = GetComponent<RectTransform>();

        SavePosition();
    }

    void Update()
    {
        if (isDragging) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                EndDrag();
            }

            // Check overlap with slots
            foreach (RectTransform validParent in validParents) {
                if (rt.RectOverlaps(validParent)) {
                    // TODO: Change this
                    validParent.gameObject.GetComponent<Outline>().enabled = true;
                } else {
                    validParent.gameObject.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }

    public void Init(List<RectTransform> validParents)
    {
        this.validParents = validParents;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SavePosition();
        isDragging = true;

        Debug.Log("Begin drag"  + eventData.position);
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
        rt.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow mouse
        Rect camRect = Camera.main.rect;
        Vector2 normalizedViewPoint = Camera.main.ScreenToViewportPoint(eventData.position);
        Vector2 viewPoint = new Vector2(normalizedViewPoint.x * camRect.width, normalizedViewPoint.y * camRect.height)
            + new Vector2(camRect.xMin, camRect.yMin);
        Debug.Log("Drag " + viewPoint);
        Debug.Log(rt.rect);
        rt.anchorMin = viewPoint;
        rt.anchorMax = viewPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag"  + eventData.position);
        EndDrag();
    }

    private void EndDrag()
    {
        RestorePosition();
        isDragging = false;
    }

    private void SavePosition()
    {
        origParent = transform.parent;
        origAnchor = rt.anchorMin;
        origAnchorPos = rt.anchoredPosition;
    }

    private void RestorePosition()
    {
        transform.SetParent(origParent, true);
        rt.anchorMin = origAnchor;
        rt.anchorMax = origAnchor;
        rt.anchoredPosition = origAnchorPos;
    }
}
