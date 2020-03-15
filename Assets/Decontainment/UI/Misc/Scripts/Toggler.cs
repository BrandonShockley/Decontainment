using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    [SerializeField]
    private GameObject toggleGO = null;

    public void Toggle()
    {
        toggleGO.SetActive(!toggleGO.activeSelf);
    }
}
