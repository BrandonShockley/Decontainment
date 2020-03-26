using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSelector : MonoBehaviour
{
    private GameObject oldPanel;

    public void SelectPanel(GameObject panel)
    {
        oldPanel?.SetActive(false);

        if (panel != oldPanel) {
            panel?.SetActive(true);
            oldPanel = panel;
        } else {
            oldPanel = null;
        }

    }
}
