using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int _attackStat=5;
    public int AttackStat
    {
        get { return _attackStat; }
    }
    [SerializeField]
    private int _defenseStat;
    public int DefenseStat
    {
        get { return _defenseStat; }
    }
    [SerializeField]
    private int _maxStaminaStat;
    public int MaxStaminaStat
    {
        get { return _maxStaminaStat; }
    }

    public GameObject turnCircle;



    private int _characterId;
    public int CharacterId
    {
        get { return _characterId; }
        set { _characterId = value; }
    }
    private int _stamina;
    public int Stamina
    {
        get { return _stamina; }
    }
    protected bool _isMovable;
    public bool IsMovable
    {
        get { return _isMovable; }
    }

    public enum CharacterState
    {
        Spectating,
        Idle,
        Moving,
        Attacking
    }

    protected CharacterState _currentState;
    public CharacterState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    protected BoxCollider2D collider;
    protected bool turnUiLoaded;

    private void Awake()
    {
        Debug.Assert(turnCircle, "Character: turnCircle not found");
        Debug.Assert(turnCircle.CompareTag(GameManager.Instance.TAG_VISUAL_EFFECT), "Character: turnCircle not found");
        turnCircle.SetActive(false);

        collider = GetComponent<BoxCollider2D>();
        Debug.Assert(collider, "Character: boxCollider component not found");

        turnUiLoaded = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStamina(int stamina)
    {
        _stamina = stamina;
        //TODO some checks
    }

    protected bool IsMyTurn()
    {
        if (GameManager.Instance.IsGamePaused)
            return false;
        
        return LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn() == CharacterId;
    }

    virtual public IEnumerator PlayTurn() { yield return new WaitForSeconds(0); }

    virtual protected void SetupStartingTurnUI() 
    {
        turnCircle.SetActive(true);
    }

    virtual protected Character FindPlayerAround()
    {
        Vector3 characterLocation = transform.position;
        Character result;

        result = FindPlayerAtPosition(transform.position + Vector3.down);
        if (result && result.CompareTag(GameManager.Instance.TAG_PLAYER))
            return result;

        result = FindPlayerAtPosition(transform.position + Vector3.left);
        if (result && result.CompareTag(GameManager.Instance.TAG_PLAYER))
            return result;

        result = FindPlayerAtPosition(transform.position + Vector3.up);
        if (result && result.CompareTag(GameManager.Instance.TAG_PLAYER))
            return result;

        result = FindPlayerAtPosition(transform.position + Vector3.right);
        if (result && result.CompareTag(GameManager.Instance.TAG_PLAYER))
            return result;

        return null;
    }

    protected Character FindPlayerAtPosition(Vector3 location)
    {
        Vector2 pointA = (Vector2)location + collider.size / 2;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, (Vector2)location - collider.size / 2);
        
        foreach(Collider2D coll in colliders)
            if (coll.CompareTag(GameManager.Instance.TAG_PLAYER))
                return coll.gameObject.GetComponent<Character>();


        return null;
    }

    virtual protected void Attack(Character target) 
    {
        Debug.Log(this.name + " is attacking " + target.name);
    }


    virtual public void EndMyTurn()
    {
        DisableTurnUI();
        TurnManager.EndTurn(CharacterId);
    }

    virtual protected void DisableTurnUI()
    {
        turnCircle.SetActive(false);
        turnUiLoaded = false;
    }
}
