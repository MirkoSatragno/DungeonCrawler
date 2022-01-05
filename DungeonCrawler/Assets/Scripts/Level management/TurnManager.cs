using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public delegate void EndTurnDelegate(int characterId);
    public static EndTurnDelegate EndTurn;  
    
    private LevelManager levelManager;

    //Queue of charactersId
    private Queue<int> turnQueue;

    private void Awake()
    {
        turnQueue = new Queue<int>();
        levelManager = GetComponent<LevelManager>();
        Debug.Assert(levelManager, "TurnManager: levelManager component not found");

        EndTurn = onEndTurn;
    }








    public void AddToQueue(int characterId)
    {
        Debug.Assert(!turnQueue.Contains(characterId), "TurnManager: characterId is already present in TurnQueue");
        turnQueue.Enqueue(characterId);
    }

    public Character ExctractFromQueue()
    {
        Debug.Assert(turnQueue.Count != 0, "TurnManager: accessing empty turnQueue");
        
        int characterId = turnQueue.Dequeue();
        Character extractedChar = levelManager.GetActiveCharacter(characterId);

        while(extractedChar == null)
        {
            Debug.Assert(turnQueue.Count != 0, "TurnManager: accessing empty turnQueue");
            characterId = turnQueue.Dequeue();
            extractedChar = levelManager.GetActiveCharacter(characterId);
        }

        return extractedChar;
    }

    public int ActiveCharacterIdCurrentTurn()
    {
        Debug.Assert(turnQueue.Count != 0, "TurnManager: accessing empty turnQueue");

        int characterId = turnQueue.Peek();
        Character activeChar = levelManager.GetActiveCharacter(characterId);

        while (activeChar == null)
        {
            turnQueue.Dequeue();
            Debug.Assert(turnQueue.Count != 0, "TurnManager: accessing empty turnQueue");
            characterId = turnQueue.Peek();
            activeChar = levelManager.GetActiveCharacter(characterId);
        }

        return characterId;
    }
    
    public void StartNewTurn()
    {
        int characterId = ActiveCharacterIdCurrentTurn();
        Character currentCharacter = levelManager.GetActiveCharacter(characterId);

        currentCharacter.PlayTurn();
        IEnumerator coroutine = currentCharacter.PlayTurn();
        StartCoroutine(coroutine);
    }

    public void onEndTurn(int characterId)
    {
        if (turnQueue.Peek() != characterId)
            return;

        turnQueue.Dequeue();
        turnQueue.Enqueue(characterId);

        StartNewTurn();
    }
}
