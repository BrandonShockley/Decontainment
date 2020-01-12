using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action onDragStart;
        public Action onDragCancel;
        // Called upon draggable overlapping a valid slot
        public Action<RectTransform> onDragEnter;
        // Called upon draggable unoverlapping a valid slot
        public Action<RectTransform> onDragExit;
        // Called upon drag release with a slot highlighted
        public Action<RectTransform> onDragSuccess;
        // Returns whether arg1 is included in valid slot search
        public Func<RectTransform, bool> filterFunc;

        private bool isDragging;
        private List<RectTransform> validSlots;
        private RectTransform bestSlot;
        private Transform origParent;
        private int origIndex;
        private Vector2 origAnchor;
        private Vector2 origAnchorPos;

        private Canvas canvas;
        private RectTransform rt;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rt = GetComponent<RectTransform>();
            SaveState();
        }

        void Update()
        {
            RectTransform oldBestSlot = bestSlot;
            bestSlot = null;
            if (isDragging) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    CancelDrag();
                } else {
                    float minSqDistance = float.PositiveInfinity;
                    foreach (RectTransform validSlot in validSlots) {
                        // Filtering results
                        if (filterFunc != null && filterFunc(validSlot)) {
                            continue;
                        }

                        // Choose the closest overlapping slot
                        if (rt.RectOverlaps(validSlot)) {
                            float sqDistance = (validSlot.position - rt.position).sqrMagnitude;
                            if (sqDistance < minSqDistance) {
                                minSqDistance = sqDistance;
                                bestSlot = validSlot;
                            }
                        }
                    }
                }
            }

            if (oldBestSlot != null && oldBestSlot != bestSlot) {
                onDragExit?.Invoke(oldBestSlot);
            }
            if (bestSlot != null) {
                onDragEnter?.Invoke(bestSlot);
            }
        }

        public void Init(List<RectTransform> validSlots)
        {
            this.validSlots = validSlots;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            SaveState();
            isDragging = true;

            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();
            rt.anchoredPosition = Vector2.zero;
            onDragStart?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging) {
                // Follow mouse
                Rect camRect = Camera.main.rect;
                Vector2 normalizedViewPoint = Camera.main.ScreenToViewportPoint(eventData.position);
                Vector2 viewPoint = new Vector2(normalizedViewPoint.x * camRect.width, normalizedViewPoint.y * camRect.height)
                    + new Vector2(camRect.xMin, camRect.yMin);
                rt.anchorMin = viewPoint;
                rt.anchorMax = viewPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDragging) {
                EndDrag();
            }
        }

        private void EndDrag()
        {
            if (bestSlot != null) {
                RestorePosition();
                onDragSuccess?.Invoke(bestSlot);
                isDragging = false;
            } else {
                CancelDrag();
            }
        }

        private void CancelDrag()
        {
            RestorePosition();
            RestoreParent();
            isDragging = false;
            onDragCancel?.Invoke();
        }

        private void SaveState()
        {
            origParent = transform.parent;
            origIndex = transform.GetSiblingIndex();
            origAnchor = rt.anchorMin;
            origAnchorPos = rt.anchoredPosition;
        }

        private void RestorePosition()
        {
            rt.anchorMin = origAnchor;
            rt.anchorMax = origAnchor;
            rt.anchoredPosition = origAnchorPos;
        }

        private void RestoreParent()
        {
            transform.SetParent(origParent, false);
            transform.SetSiblingIndex(origIndex);
        }
    }
}
