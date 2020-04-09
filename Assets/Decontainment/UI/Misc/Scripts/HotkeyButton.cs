using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotkeyButton : MonoBehaviour
{
    [SerializeField]
    private KeyCode keyCode = default;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(keyCode) && EventSystem.current.currentSelectedGameObject == null) {
            button.onClick.Invoke();
        }
    }
}
