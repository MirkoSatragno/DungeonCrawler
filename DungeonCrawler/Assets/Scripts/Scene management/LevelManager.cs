using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Grid mainGrid;
    [SerializeReference]
    private UnmovableCharacter unmovableCharacter;
    [SerializeField]
    private Vector2 startingPosition;
    [SerializeReference]
    private EnemyCharacter enemyCharacter;
    [SerializeField]
    private Vector2 startingPosition2;
    [SerializeReference]
    private PlayerCharacter playerCharacter;
    [SerializeField]
    private Vector2 startingPosition3;

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InstantiatePlayer();
        InstantiatePlayer2();
        InstantiatePlayer3();

        turnManager.StartNewTurn();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public Character GetActiveCharacter(int characterId)
    {
        if (activeCharactersMap.ContainsKey(characterId))
            return (Character) activeCharactersMap[characterId];

        return null;
    }

    private void InstantiatePlayer()
    {
        int characterId = getNewCharacterId();
        UnmovableCharacter spawnedPlayer = Instantiate(unmovableCharacter);
        spawnedPlayer.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedPlayer);
        turnManager.AddToQueue(characterId);

        spawnedPlayer.transform.position = startingPosition;
    }

    //Debug purpose only
    private void InstantiatePlayer2()
    {
        int characterId = getNewCharacterId();
        EnemyCharacter spawnedPlayer = Instantiate(enemyCharacter);
        spawnedPlayer.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedPlayer);
        turnManager.AddToQueue(characterId);

        spawnedPlayer.transform.position = startingPosition2;
    }

    //Debug purpose only
    private void InstantiatePlayer3()
    {
        int characterId = getNewCharacterId();
        PlayerCharacter spawnedPlayer = Instantiate(playerCharacter);
        spawnedPlayer.CharacterId = characterId;
        activeCharactersMap.Add(characterId, spawnedPlayer);
        turnManager.AddToQueue(characterId);

        spawnedPlayer.transform.position = startingPosition3;
    }

    public void RemoveCharacter(int characterId)
    {
        Debug.Assert(activeCharactersMap.ContainsKey(characterId), "LevelManager: character to be removed not found");

        activeCharactersMap.Remove(characterId);
        //can't remove from turnQueue because it's not random access
    }
}
