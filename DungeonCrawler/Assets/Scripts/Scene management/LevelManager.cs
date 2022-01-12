using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField]
    private Grid mainGrid;
    
    [SerializeReference]
    private UnmovableCharacter bossCharacter;
    [SerializeField]
    private Vector2 startingPositionBoss;
    [SerializeReference]
    private PlayerCharacter playerCharacter;
    [SerializeField]
    private Vector2 startingPositionPlayer;

    static LevelManager _instance;
    static public LevelManager Instance
    {
        get { return _instance; }
    }
    
    private float _cellSize;
    public float CellSize
    {
        get { return _cellSize; }
    }

    private int nextCharacterID;
    private Hashtable activeCharactersMap;
    public bool friendSaved;

    [HideInInspector]
    public TurnManager turnManager;

    private void Awake()
    {
        _instance = this;
        
        setGridCellSize();

        nextCharacterID = 0;
        activeCharactersMap = new Hashtable();

        turnManager = GetComponent<TurnManager>();
        Debug.Assert(turnManager, "LevelManager: turnManaget component not found");

        friendSaved = false;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InstantiatePlayer();
        //InstantiateBoss();

        turnManager.StartNewTurn();
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }






    private void setGridCellSize()
    {
        Debug.Assert(mainGrid, "LevelManager: mainGrid reference is missing");
        Vector3 cell = mainGrid.cellSize;
        Debug.Assert(cell.x == cell.y, "LevelManager: mainGrid cell shape is not a square");
        _cellSize = cell.x;
    }


    private int getNewCharacterId()
    {
        return nextCharacterID++;
    }

    /*"Active" means that it's alive and present; not "current"*/
    public Character GetCharacter(int characterId)
    {
        if (activeCharactersMap.ContainsKey(characterId))
            return (Character) activeCharactersMap[characterId];

        return null;
    }

    public void InstantiateBoss()
    {
        int characterId = getNewCharacterId();
        UnmovableCharacter spawnedCharacter = Instantiate(bossCharacter, (Vector3)startingPositionBoss, Quaternion.identity);
        spawnedCharacter.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedCharacter);
        turnManager.AddToQueue(characterId);
    }

    private void InstantiatePlayer()
    {
        int characterId = getNewCharacterId();
        PlayerCharacter spawnedPlayer = Instantiate(playerCharacter, (Vector3)startingPositionPlayer, Quaternion.identity);
        spawnedPlayer.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedPlayer);
        turnManager.AddToQueue(characterId);
    }


    public void InstantiateEnemy(EnemyCharacter enemyType, Vector2 position)
    {
        int characterId = getNewCharacterId();
        EnemyCharacter spawnedEnemy = Instantiate(enemyType, (Vector3)position, Quaternion.identity);
        spawnedEnemy.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedEnemy);
        turnManager.AddToQueue(characterId);
    }

    public void RemoveCharacter(int characterId)
    {
        Debug.Assert(activeCharactersMap.ContainsKey(characterId), "LevelManager: character to be removed not found");

        activeCharactersMap.Remove(characterId);
        //can't remove from turnQueue because it's not random access
    }



    static public GameObject GetGameObjectAtLocation(Vector2 position)
    {
        //I don't want to detect a collision on the extreme limit of a collider
        float smallRadiusFactor = 0.01f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, LevelManager.Instance.CellSize / 2 * smallRadiusFactor);

        List<Collider2D> nonTriggers = new List<Collider2D>();
        foreach (Collider2D coll in colliders)
            if (!coll.isTrigger)
                nonTriggers.Add(coll);

        if (1 < nonTriggers.Count)
            Debug.Log("LevelManager: unexpected coliders quantity");

        if (nonTriggers.Count == 1)
            return nonTriggers[0].gameObject;

        return null;
    }

    static public Type FindTaggedObjectAround<Type>(string tag, Vector3 location, Vector2 colliderSize)
    {
        Type result;

        result = GetTaggedObjectAtNearPosition<Type>(tag, location + Vector3.down, colliderSize);
        if (result != null && !result.Equals(default(Type)))
            return result;

        result = GetTaggedObjectAtNearPosition<Type>(tag, location + Vector3.left, colliderSize);
        if (result != null && !result.Equals(default(Type)))
            return result;

        result = GetTaggedObjectAtNearPosition<Type>(tag, location + Vector3.up, colliderSize);
        if (result != null && !result.Equals(default(Type)))
            return result;

        result = GetTaggedObjectAtNearPosition<Type>(tag, location + Vector3.right, colliderSize);
        if (result != null && !result.Equals(default(Type)))
            return result;

        return default(Type);
    }

    static public Type GetTaggedObjectAtNearPosition<Type>(string TAG, Vector3 location, Vector2 colliderSize)
    {
        //I don't want to detect a collision on the extreme limit of a collider
        float boundaryCorrection = 0.9f;
        Vector2 pointA = (Vector2)location + colliderSize / 2 * boundaryCorrection;
        Vector2 pointB = (Vector2)location - colliderSize / 2 * boundaryCorrection;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB);

        

        foreach (Collider2D coll in colliders)
            if (coll.CompareTag(TAG))
                return coll.gameObject.GetComponent<Type>();

        return default(Type);
    }

    public int charactersNumberInDungeon()
    {
        return activeCharactersMap.Count;
    }

}
