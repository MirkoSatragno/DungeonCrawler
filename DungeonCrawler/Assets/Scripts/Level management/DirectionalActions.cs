using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalActions : MonoBehaviour
{
    [SerializeReference]
    private GameObject leftArrow;
    [SerializeReference]
    private GameObject rightArrow;
    [SerializeReference]
    private GameObject topArrow;
    [SerializeReference]
    private GameObject downArrow;
    [SerializeReference]
    private GameObject leftAttack;
    [SerializeReference]
    private GameObject rightAttack;
    [SerializeReference]
    private GameObject topAttack;
    [SerializeReference]
    private GameObject downAttack;
    [SerializeReference]
    private GameObject sleep;

    public enum Direction
    {
        left,
        right,
        up,
        down
    }

    public enum Action
    {
        Move,
        Attack,
        None
    }

    GameObject[,] actionLists;
    PlayerCharacter playerParent;

    private void Awake()
    {
        Debug.Assert(leftArrow, "DirectionalActions: arrow gameobject reference not found");
        Debug.Assert(rightArrow, "DirectionalActions: arrow gameobject reference not found");
        Debug.Assert(topArrow, "DirectionalActions: arrow gameobject reference not found");
        Debug.Assert(downArrow, "DirectionalActions: arrow gameobject reference not found");
        Debug.Assert(leftAttack, "DirectionalActions: attack gameobject reference not found");
        Debug.Assert(rightAttack, "DirectionalActions: attack gameobject reference not found");
        Debug.Assert(topAttack, "DirectionalActions: attack gameobject reference not found");
        Debug.Assert(downAttack, "DirectionalActions: attack gameobject reference not found");
        Debug.Assert(sleep, "DirectionalActions: sleep gameObject reference not found");

        actionLists = new GameObject[4, 2];
        actionLists[(int)Direction.left, (int)Action.Attack] = leftAttack;
        actionLists[(int)Direction.right, (int)Action.Attack] = rightAttack;
        actionLists[(int)Direction.up, (int)Action.Attack] = topAttack;
        actionLists[(int)Direction.down, (int)Action.Attack] = downAttack;
        actionLists[(int)Direction.left, (int)Action.Move] = leftArrow;
        actionLists[(int)Direction.right, (int)Action.Move] = rightArrow;
        actionLists[(int)Direction.up, (int)Action.Move] = topArrow;
        actionLists[(int)Direction.down, (int)Action.Move] = downArrow;

    }

    private void Start()
    {
        playerParent = GetComponentInParent<PlayerCharacter>();
        Debug.Assert(playerParent, "DirectionalActions: playerCharacter parent not found");
    }


    private void Update()
    {
        if(playerParent.CurrentState == Character.CharacterState.Idle)
        {
            CheckKeyboardInput();
        }
    }



    public void SetAction(Direction dir, Action action)
    {
    
        foreach (Action act in System.Enum.GetValues(typeof(Action))){
            //"None" action is a valid player request, but it's not physically stored in the matrix
            if (act == Action.None)
                continue;

            if (act == action)
                actionLists[(int)dir, (int)act].SetActive(true);
            else
                actionLists[(int)dir, (int)act].SetActive(false);
        }
        
    }

    public void onMoveButtonPressed()
    {
        Vector3 relativePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if(Mathf.Abs(relativePosition.y) < Mathf.Abs(relativePosition.x))
        {
            if (relativePosition.x < 0)
                playerParent.MoveButtonPressed(Direction.left);
            else
                playerParent.MoveButtonPressed(Direction.right);
        }
        else
        {
            if (relativePosition.y < 0)
                playerParent.MoveButtonPressed(Direction.down);
            else
                playerParent.MoveButtonPressed(Direction.up);
        }
    }

    public void onAttackButtonPressed()
    {
        Vector3 relativePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Mathf.Abs(relativePosition.y) < Mathf.Abs(relativePosition.x))
        {
            if (relativePosition.x < 0)
                playerParent.AttackButtonPressed(Direction.left);
            else
                playerParent.AttackButtonPressed(Direction.right);
        }
        else
        {
            if (relativePosition.y < 0)
                playerParent.AttackButtonPressed(Direction.down);
            else
                playerParent.AttackButtonPressed(Direction.up);
        }
    }

    public void onSleepButtonPressed()
    {
        playerParent.SleepButtonPressed();
    }


    private void CheckKeyboardInput()
    {
        if(0 < Input.GetAxis(GameManager.AXIS_NAME_VERTICAL))
        {
            if (actionLists[(int)Direction.up, (int)Action.Move].gameObject.activeSelf)
                playerParent.MoveButtonPressed(Direction.up);
            else
                playerParent.AttackButtonPressed(Direction.up);
        }
        if (Input.GetAxis(GameManager.AXIS_NAME_VERTICAL) < 0)
        {
            if (actionLists[(int)Direction.down, (int)Action.Move].gameObject.activeSelf)
                playerParent.MoveButtonPressed(Direction.down);
            else
                playerParent.AttackButtonPressed(Direction.down);
        }
        if (Input.GetAxis(GameManager.AXIS_NAME_HORIZONTAL) < 0)
        {
            if (actionLists[(int)Direction.left, (int)Action.Move].gameObject.activeSelf)
                playerParent.MoveButtonPressed(Direction.left);
            else
                playerParent.AttackButtonPressed(Direction.left);
        }
        if (0 < Input.GetAxis(GameManager.AXIS_NAME_HORIZONTAL))
        {
            if (actionLists[(int)Direction.right, (int)Action.Move].gameObject.activeSelf)
                playerParent.MoveButtonPressed(Direction.right);
            else
                playerParent.AttackButtonPressed(Direction.right);
        }
    }
}
