using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialPowerManager : MonoBehaviour
{
    [SerializeReference, Tooltip("Special power button in enabled state")]
    private Button enabledPowerButton;
    [SerializeReference, Tooltip("Special power button in disabled state")]
    private Button disabledPowerButton;

    private void Awake()
    {
        Debug.Assert(enabledPowerButton && disabledPowerButton, "SpecialPowerManager: button reference not found");

        enabledPowerButton.gameObject.SetActive(false);
        disabledPowerButton.gameObject.SetActive(true);
    }

    public void showCorrectPowerButton()
    {
        Character currentCharacter = LevelManager.Instance.GetCurrentCharacter();
        Obstacle obstacle = LevelManager.FindTaggedObjectAround<Obstacle>(GameManager.TAG_OBSTACLE, currentCharacter.transform.position, currentCharacter.boxCollider.size);
        if (obstacle)
        {
            enabledPowerButton.gameObject.SetActive(true);
            disabledPowerButton.gameObject.SetActive(false);
        }
        else
        {
            enabledPowerButton.gameObject.SetActive(false);
            disabledPowerButton.gameObject.SetActive(true);
        }

    }
}
