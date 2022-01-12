using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialPowerManager : MonoBehaviour
{
    [SerializeReference]
    private Button enabledPowerButton;
    [SerializeReference]
    private Button disabledPowerButton;

    private PlayerCharacter playerParent;

    private void Awake()
    {
        Debug.Assert(enabledPowerButton && disabledPowerButton, "SpecialPower: button reference not found");

        playerParent = GetComponentInParent<PlayerCharacter>();
        Debug.Assert(playerParent, "TurnCanvas: playerCharacter parent not found");

        enabledPowerButton.gameObject.SetActive(false);
        disabledPowerButton.gameObject.SetActive(true);
    }

    public void showCorrectPowerButton()
    {
        Obstacle obstacle = LevelManager.FindTaggedObjectAround<Obstacle>(GameManager.TAG_OBSTACLE, playerParent.transform.position, playerParent.boxCollider.size);
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
