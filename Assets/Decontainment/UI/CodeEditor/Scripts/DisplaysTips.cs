using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Editor
{
    public class DisplaysTips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public GameObject toolTipContainer;
        public GameObject toolTipInstance;

        private GameObject canvas;
        private int t;

        private bool hovered;

        private const int tipDelay = 40;

        void Awake()
        {
            canvas = GameObject.Find("Canvas");
        }

        void Update()
        {
            if (hovered)
            {
                if ((toolTipInstance == null) && (t > tipDelay))
                {
                    toolTipInstance = Instantiate(toolTipContainer, Input.mousePosition + Vector3.right * 10, Quaternion.identity, canvas.transform);
                } else
                {
                    t++;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
            t = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
            t = 0;

            if (toolTipInstance != null)
            {
                Debug.Log("Destroyed");
                Destroy(toolTipInstance);
            }
        }

    }
}
