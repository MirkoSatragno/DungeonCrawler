using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeReference]
    private EnemyCharacter weakEnemy;
    [SerializeReference]
    private EnemyCharacter strongEnemy;
    [SerializeField, Range(0.0f, 1.0f)]
    private float weakEnemySpawnRate;
    [SerializeField, Range(0.0f, 1.0f)]
    private float strongEnemySpawnRate;

    [SerializeReference]
    private SpecialEffect spawnEffect;
    [SerializeField]
    private int maxCharacterPerDungeon = 20;

    private const int SPAWN_CYCLE_DURATION = 20;
    private int roomWidth, roomHeight;
    //serialization is for debug only
    [SerializeField]
    private int playersInRoomCount;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        Debug.Assert(weakEnemy && strongEnemy, "EnemySpawn: enemyCharacter reference not found");

        Debug.Assert(spawnEffect, "EnemySpawn: spawnEffect reference not found");

        boxCollider = GetComponent<BoxCollider2D>();
        Debug.Assert(boxCollider, "EnemySpawn: boxCollider2D component not found");

        playersInRoomCount = 0;
        RoomDetect();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SpawnCycle");
    }

    

    public void enteringPlayer()
    {
        playersInRoomCount++;
    }

    public void exitingPlayer()
    {
        playersInRoomCount--;
    }




    void RoomDetect()
    {
        RaycastHit2D hitLeft, hitRight, hitUp, hitDown;
        LayerMask wallMask = LayerMask.GetMask(GameManager.LAYER_NAME_SPAWN_COLLIDER_BOUNDARY);

        hitLeft = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity, wallMask);
        hitRight = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, wallMask);
        hitUp = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity, wallMask);
        hitDown = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, wallMask);

        if (hitLeft.collider == null || hitRight.collider == null || hitUp.collider == null  || hitDown.collider == null)
            return;

        roomWidth = Mathf.RoundToInt(hitLeft.distance + hitRight.distance);
        roomHeight = Mathf.RoundToInt(hitUp.distance + hitDown.distance);
        Vector3 position = transform.position;
        position.x += (hitRight.distance - hitLeft.distance)/2;
        position.y += (hitUp.distance - hitDown.distance)/2;
        transform.position = position;

        boxCollider.size = new Vector2(roomWidth, roomHeight);
    }


    private IEnumerator SpawnCycle()
    {

        while(true)
        {
            Character currentCharacter = LevelManager.Instance.GetCharacter(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());

            // if a character is moving, or if there are already enough characters around,
            // or if there are no players in the room, let's wait
            yield return new WaitWhile(() => {
                if (currentCharacter.CurrentState == Character.CharacterState.Moving)
                    return true;
                if (maxCharacterPerDungeon < LevelManager.Instance.charactersNumberInDungeon())
                    return true;
                if (playersInRoomCount == 0)
                    return true;
                return false; });

            if (!LevelManager.Instance.friendSaved || Random.Range(0, 1) < 0.5)
            {
                if (Random.Range(0f, 1f) < weakEnemySpawnRate)
                    InstantiateEnemy(weakEnemy);
            }
            else
                if (Random.Range(0f, 1f) < strongEnemySpawnRate)
                    InstantiateEnemy(strongEnemy);

            yield return new WaitForSeconds(SPAWN_CYCLE_DURATION);
        }
        
    }

    private void InstantiateEnemy(EnemyCharacter enemy)
    {
        Vector2 location;

        do
            location = GetRandomLocationInsideRoom();
        while (LevelManager.GetGameObjectAtLocation(location));

        Instantiate(spawnEffect, (Vector3)location, Quaternion.identity);
        LevelManager.Instance.InstantiateEnemy(enemy, location);
    }

    private Vector2 GetRandomLocationInsideRoom()
    {
        float x = transform.position.x + (Random.Range(0, roomWidth) - roomWidth/2);
        if (roomWidth % 2 == 0)
            x += 0.5f;
        float y = transform.position.y + (Random.Range(0, roomHeight) - roomHeight/2);
        if (roomHeight % 2 == 0)
            y += 0.5f;

        return new Vector2(x, y);
    }
}
