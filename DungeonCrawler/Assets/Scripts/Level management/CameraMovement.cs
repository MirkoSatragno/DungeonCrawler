using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        int newCurrentCharacter = LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn();
        Character currentChar = LevelManager.Instance.GetActiveCharacter(newCurrentCharacter);
        Debug.Assert(currentChar, "CameraMovement: character not found");

        if (this.transform.position != currentChar.transform.position)
        {
            this.transform.position = new Vector3(currentChar.transform.position.x, currentChar.transform.position.y, this.transform.position.z);
        }
            
    }
}
