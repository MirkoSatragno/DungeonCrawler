using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Canvas popUpCanvas;

    private void Awake()
    {
        popUpCanvas = GetComponentInChildren<Canvas>();
        Debug.Assert(popUpCanvas, "PopUp: popUpCanvas child component not found");

        popUpCanvas.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        onPointerExit();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.getMouseIcon() != GameManager.MouseIcon.Default)
            return;

        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Info);
        popUpCanvas.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit();
    }

    public void onPointerExit()
    {
        if (GameManager.Instance.getMouseIcon() != GameManager.MouseIcon.Info)
            return;

        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
        popUpCanvas.gameObject.SetActive(false);
    }
}
