using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MovableCharacter
{
    [SerializeField]
    private float maxStartHuntingRadius = 3;
    [SerializeField]
    private int maxPredictedWalkCost = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (IsMyTurn() && CurrentState == CharacterState.Idle && Input.GetMouseButtonDown(0))
        //{
        //    StartCoroutine("EndMyTurn");
        //}
    }





    public override IEnumerator PlayTurn()
    {
        StartCoroutine("SetupStartingTurnUI");

        yield return new WaitForSeconds(preTurnWait);
        CurrentState = CharacterState.Idle;

        attackedCharacter = FindPlayerAround();
        if (attackedCharacter)
        {
            Attack(attackedCharacter);
            
            yield return new WaitForSeconds(1);
        }
        else
        {
            Vector2 movementDirection;
            Character player = FindReachablePlayer(out movementDirection);

            //if (player)
            //    Debug.Log("Found " + player.name);

            StartCoroutine("EndMyTurn");
        }

        

    }


    public Character FindReachablePlayer(out Vector2 movementDirection)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, maxStartHuntingRadius);


        string TAG = GameManager.TAG_PLAYER;
        foreach (Collider2D coll in colliders)
            if (coll.CompareTag(TAG))
            {
                Character player = coll.gameObject.GetComponent<Character>();

                if(CanPlayerBeReached(player, out movementDirection))
                    return player;
            }

        movementDirection = Vector2.zero;
        return null;
    }

    public bool CanPlayerBeReached(Character player, out Vector2 startDirection)
    {
        float cellSize = LevelManager.Instance.CellSize;
        Vector3 targetPos = player.transform.position;

        SortedList<float, DjikNode> nearbyNodes = new SortedList<float, DjikNode>();
        Hashtable visitedNodes = new Hashtable();
        
        DjikNode startingNode = new DjikNode(null, (Vector2)transform.position, player.transform.position);
        visitedNodes.Add(startingNode.position, startingNode);

        DjikNode topNode = new DjikNode(startingNode, startingNode.position + new Vector2(0, cellSize), targetPos);
        nearbyNodes.Add(topNode.targetDist, topNode);
        DjikNode downNode = new DjikNode(startingNode, startingNode.position + new Vector2(0, -1 * cellSize), targetPos);
        nearbyNodes.Add(downNode.targetDist, downNode);
        DjikNode leftNode = new DjikNode(startingNode, startingNode.position + new Vector2(-1 * cellSize, 0), targetPos);
        nearbyNodes.Add(leftNode.targetDist, leftNode);
        DjikNode rightNode = new DjikNode(startingNode, startingNode.position + new Vector2(cellSize, 0), targetPos);
        nearbyNodes.Add(rightNode.targetDist, rightNode);

        

        while (nearbyNodes.Count != 0)
        {
            float currentKey = nearbyNodes.Keys[0];
            DjikNode currentNode = nearbyNodes[currentKey];
            nearbyNodes.Remove(currentKey);
            

            Debug.Log("Current node: " + currentNode.targetDist);

            GameObject obj = GameobjectAtLocation(currentNode.position);
            if(obj == null && currentNode.cost < maxPredictedWalkCost)
            {
                //The cell is empty and the enemy could potentially move here. Let's look around and find new nearby cells.

                visitedNodes.Add(currentNode.position, currentNode);

                topNode = new DjikNode(currentNode, currentNode.position + new Vector2(0, cellSize), targetPos);
                if (!visitedNodes.Contains(topNode.position))
                    nearbyNodes.Add(topNode.targetDist, topNode);
                downNode = new DjikNode(currentNode, currentNode.position + new Vector2(0, -1 * cellSize), targetPos);
                if (!visitedNodes.Contains(downNode.position))
                    nearbyNodes.Add(downNode.targetDist, downNode);
                leftNode = new DjikNode(currentNode, currentNode.position + new Vector2(-1 * cellSize, 0), targetPos);
                if (!visitedNodes.Contains(leftNode.position))
                    nearbyNodes.Add(leftNode.targetDist, leftNode);
                rightNode = new DjikNode(currentNode, currentNode.position + new Vector2(cellSize, 0), targetPos);
                if (!visitedNodes.Contains(rightNode.position))
                    nearbyNodes.Add(rightNode.targetDist, rightNode);


            } else if (obj != null && obj.CompareTag(GameManager.TAG_PLAYER))
            {
                Debug.Log("Yes, player can be reached");
                startDirection = Vector2.zero;
                return true;
            }                

        }

        startDirection = Vector2.zero;
        return false;
    }

    private GameObject GameobjectAtLocation(Vector2 position)
    {
        //I don't want to detect a collision on the extreme limit of a collider
        float boundaryCorrection = 0.9f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, LevelManager.Instance.CellSize/2 * boundaryCorrection);
        if (1 < colliders.Length)
            Debug.Log("EnemyCharacter: unexpected coliders quantity");

        if(colliders.Length == 1)
            return colliders[0].gameObject;
            
        return null;
    }


}

public class DjikNode
{
    public Vector2 position;
    public DjikNode previousNode;
    public float targetDist;
    public int cost;

    public DjikNode(DjikNode previousNode, Vector2 position, Vector3 targetPos)
    {
        this.previousNode = previousNode;
        this.position = position;
        cost = previousNode != null ? previousNode.cost + 1 : 0;

        targetDist = Vector2.Distance(position, (Vector2)targetPos);
    }

    public static bool operator <(DjikNode nodeA, DjikNode nodeB)
    {
        return nodeA.targetDist < nodeB.targetDist;
    }

    public static bool operator >(DjikNode nodeA, DjikNode nodeB)
    {
        return nodeA.targetDist > nodeB.targetDist;
    }

}


