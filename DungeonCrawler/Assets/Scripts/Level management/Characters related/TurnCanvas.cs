using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCanvas : MonoBehaviour
{
    [SerializeReference]
    private Canvas buttonsCanvas;

    private void Awake()
    {
        Debug.Assert(buttonsCanvas, "TurnCanvas: child Canvas component not found");
    }


    public void SetInputsActive(bool active)
    {
        if (active)
            buttonsCanvas.gameObject.SetActive(true);
        else
            buttonsCanvas.gameObject.SetActive(false);
    }
}
