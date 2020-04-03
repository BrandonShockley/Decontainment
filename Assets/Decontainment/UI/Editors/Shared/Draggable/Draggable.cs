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
        public struct State
        {
            public Transform parent;
            public int siblingIndex;
            public Vector2 anchor;
            public Vector2 anchorPos;

            public void Save(RectTransform rt)
            {
                parent = rt.parent;
                siblingIndex = rt.GetSiblingIndex();
                anchor = rt.anchorMin;
                anchorPos = rt.anchoredPosition;
            }
            public void RestorePosition(RectTransform rt)
            {
                rt.anchorMin = anchor;
                rt.anchorMax = anchor;
                rt.anchoredPosition = anchorPos;
            }
            public void RestoreParent(RectTransform rt)
            {
                rt.SetParent(parent, false);
                rt.SetSiblingIndex(siblingIndex);
            }
        }

        /// Extend to make a slot ui element
        public abstract class Slot : MonoBehaviour
        {
            private RectTransform _rt;

            public RectTransform RT
            {
                get {
                    if (_rt == null) {
                        _rt = GetComponent<RectTransform>();
                        Debug.Assert(_rt != null);
                    }
                    return _rt;
                }
            }

            public abstract void HandleDragEnter();
            public abstract void HandleDragExit();
        }

        /// Called upon drag start. Returns whether to continue with normal drag.
        public Func<PointerEventData, bool> onDragStart;
        /// Called when drag ended, whether successful or not
        public Action onDragEnd;
        /// Called if drag manually cancelled or not successful
        public Action onDragCancel;
        /// Called upon draggable overlapping a valid slot
        public Action<Slot> onDragEnter;
        /// Called upon draggable unoverlapping a valid slot
        public Action<Slot> onDragExit;
        /// Called upon drag release with a valid slot highlighted
        public Action<Slot> onDragSuccess;
        /// Called upon drag release with a trash slot highlighted
        public Action<Slot> onDragTrash;
        /// Returns whether arg1 is included in valid slot search
        public Func<Slot, bool> filterFunc;

        private bool isDragging;
        private List<Slot> validSlots;
        private List<Slot> trashSlots;
        private Slot bestSlot;
        private bool isTrashSlot;
        private State oldState;

        private Canvas canvas;
        private RectTransform rt;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rt = GetComponent<RectTransform>();
        }

        void Update()
        {
            Slot oldBestSlot = bestSlot;
            bestSlot = null;
            isTrashSlot = false;
            if (isDragging) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    CancelDrag();
                } else {
                    float minSqrDistance = float.PositiveInfinity;
                    foreach (Slot validSlot in validSlots) {
                        // Filtering results
                        if (!validSlot.gameObject.activeInHierarchy || (filterFunc != null && filterFunc(validSlot))) {
                            continue;
                        }

                        // Choose the closest overlapping slot
                        if (rt.RectOverlaps(validSlot.RT)) {
                            float sqDistance = (validSlot.RT.position - rt.position).sqrMagnitude;
                            if (sqDistance < minSqrDistance) {
                                minSqrDistance = sqDistance;
                                bestSlot = validSlot;
                                isTrashSlot = false;
                            }
                        }
                    }
                    foreach (Slot trashSlot in trashSlots) {
                        // Filtering results
                        if (!trashSlot.gameObject.activeInHierarchy || (filterFunc != null && filterFunc(trashSlot))) {
                            continue;
                        }

                        // Choose the closest overlapping slot
                        if (rt.RectOverlaps(trashSlot.RT)) {
                            float sqDistance = (trashSlot.RT.position - rt.position).sqrMagnitude;
                            if (sqDistance < minSqrDistance) {
                                minSqrDistance = sqDistance;
                                bestSlot = trashSlot;
                                isTrashSlot = true;
                            }
                        }
                    }
                }
            }

            if (oldBestSlot != bestSlot) {
                if (oldBestSlot != null) {
                    oldBestSlot.HandleDragExit();
                    onDragExit?.Invoke(oldBestSlot);
                }
                if (bestSlot != null) {
                    bestSlot.HandleDragEnter();
                    onDragEnter?.Invoke(bestSlot);
                }
            }
        }

        void OnDestroy()
        {
            if (bestSlot != null) {
                bestSlot.HandleDragExit();
                onDragExit?.Invoke(bestSlot);
            }
        }

        public void Init(List<Slot> validSlots, List<Slot> trashSlots)
        {
            this.validSlots = validSlots;
            this.trashSlots = trashSlots;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || (onDragStart != null && !onDragStart(eventData))) {
                return;
            }

            oldState.Save(rt);
            isDragging = true;

            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();
            rt.anchoredPosition = Vector2.zero;
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
                onDragEnd?.Invoke();
                oldState.RestorePosition(rt);
                if (isTrashSlot) {
                    onDragTrash?.Invoke(bestSlot);
                } else {
                    onDragSuccess?.Invoke(bestSlot);
                }
                isDragging = false;
            } else {
                CancelDrag();
            }
        }

        private void CancelDrag()
        {
            onDragEnd?.Invoke();
            oldState.RestorePosition(rt);
            oldState.RestoreParent(rt);
            isDragging = false;
            onDragCancel?.Invoke();
        }
    }
}
