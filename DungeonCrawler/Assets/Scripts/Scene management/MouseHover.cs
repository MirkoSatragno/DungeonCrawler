using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    [SerializeField]
    private GameManager.MouseIcon mouseIcon;

    private void OnMouseEnter()
    {
        Debug.Log("Hello mouse");
        GameManager.Instance.setMouseIcon(mouseIcon);
    }

    private void OnMouseExit()
    {
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
    }

    private void OnDisable()
    {
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
    }
}
