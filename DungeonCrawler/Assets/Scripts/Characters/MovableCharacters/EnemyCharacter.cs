using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class EnemyCharacter : MovableCharacter
{
    [SerializeField, Range(1, 100), Tooltip("Radius within wihch players are detected")]
    private float maxStartHuntingRadius = 30;
    [SerializeField, Range(1, 100), Tooltip("Maximum steps for trying to reach a player")]
    private int maxPredictedWalkCost = 10;
    


    // Update is called once per frame
    void Update()
    {
        if(IsMyTurn() && CurrentState == CharacterState.Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, movementDestination, LevelManager.Instance.CellSize * Time.deltaTime / movementDuration);
            if (transform.position == movementDestination)
                EndMovement();

        }

        if (CurrentState == CharacterState.Dieing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a = Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
                Die();

        }
    }





    public override IEnumerator PlayTurn()
    {
        StartCoroutine("SetupStartingTurnUI");

        yield return new WaitForSeconds(preTurnWait);
        CurrentState = CharacterState.Idle;

        attackedCharacter = LevelManager.FindTaggedObjectAround<PlayerCharacter>(GameManager.TAG_PLAYER, transform.position, boxCollider.size);
        if (attackedCharacter)
        {
            Attack(attackedCharacter);
        }  
        else
        {
            Vector2 movementDirection;
            Character player = FindReachablePlayer(out movementDirection);

            if (player)
                MoveTo(movementDirection);
            else
                if(!MoveRandom())
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

        SortedSet<DjikNode> nearbyNodes = new SortedSet<DjikNode>();
        Hashtable visitedNodes = new Hashtable();
        
        DjikNode startingNode = new DjikNode(null, (Vector2)transform.position, player.transform.position);
        visitedNodes.Add(startingNode.position, startingNode);

        DjikNode topNode = new DjikNode(startingNode, startingNode.position + new Vector2(0, cellSize), targetPos);
        nearbyNodes.Add(topNode);
        DjikNode downNode = new DjikNode(startingNode, startingNode.position + new Vector2(0, -1 * cellSize), targetPos);
        nearbyNodes.Add(downNode);
        DjikNode leftNode = new DjikNode(startingNode, startingNode.position + new Vector2(-1 * cellSize, 0), targetPos);
        nearbyNodes.Add(leftNode);
        DjikNode rightNode = new DjikNode(startingNode, startingNode.position + new Vector2(cellSize, 0), targetPos);
        nearbyNodes.Add(rightNode);

        

        while (nearbyNodes.Count != 0)
        {
            DjikNode currentNode = nearbyNodes.Min;
            nearbyNodes.Remove(currentNode);
            

            GameObject obj = LevelManager.GetGameObjectAtLocation(currentNode.position);
            if(obj == null && currentNode.cost < maxPredictedWalkCost)
            {
                //The cell is empty and the enemy could potentially move here. Let's look around and find new nearby cells.

                visitedNodes.Add(currentNode.position, currentNode);

                topNode = new DjikNode(currentNode, currentNode.position + new Vector2(0, cellSize), targetPos);
                if (!visitedNodes.Contains(topNode.position))
                    nearbyNodes.Add(topNode);
                downNode = new DjikNode(currentNode, currentNode.position + new Vector2(0, -1 * cellSize), targetPos);
                if (!visitedNodes.Contains(downNode.position))
                    nearbyNodes.Add(downNode);
                leftNode = new DjikNode(currentNode, currentNode.position + new Vector2(-1 * cellSize, 0), targetPos);
                if (!visitedNodes.Contains(leftNode.position))
                    nearbyNodes.Add(leftNode);
                rightNode = new DjikNode(currentNode, currentNode.position + new Vector2(cellSize, 0), targetPos);
                if (!visitedNodes.Contains(rightNode.position))
                    nearbyNodes.Add(rightNode);


            } else if (obj != null && obj.CompareTag(GameManager.TAG_PLAYER))
            {
                while (currentNode.previousNode.previousNode != null)
                    currentNode = currentNode.previousNode;
                
                //"currentNode", now, is actually the first step we enemy character took in his successful path
                startDirection = currentNode.position - startingNode.position;
                return true;
            }                

        }


        startDirection = Vector2.zero;
        return false;
    }


    protected bool MoveRandom()
    {
        Vector2[] directions = { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
        int index = Random.Range(0, directions.Length);

        for(int i = 0; i < directions.Length; i++)
        {
            
            if (!LevelManager.GetGameObjectAtLocation(transform.position + (Vector3)directions[index] * LevelManager.Instance.CellSize))
            {
                MoveTo(directions[index]);
                return true;
            }

            index = (index + 1) % directions.Length;
        }
        
        return false;
    }

}








//"Djik" stands for Djikstra
public class DjikNode : System.IComparable<DjikNode>
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

    public int CompareTo(DjikNode otherNode)
    {
        if (this.targetDist != otherNode.targetDist)
            return this.targetDist.CompareTo(otherNode.targetDist);

        if(this.position.x != otherNode.position.x)
        return this.position.x.CompareTo(otherNode.position.x);

        return this.position.y.CompareTo(otherNode.position.y);
    }
}


