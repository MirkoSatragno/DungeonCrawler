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

    [SerializeReference]
    protected GameObject turnCircle;
    [SerializeReference]
    protected SpecialEffect AttackEffect;

    [SerializeField]
    protected float preTurnWait = 1f;
    [SerializeField]
    protected float postTurnWait = 1f;

    protected const string ANIMATOR_PARAMETER_SPEED_NAME = "Speed";
    protected const float ANIMATOR_PARAMETER_SPEED_SLOW = 0.1f;
    protected const float ANIMATOR_PARAMETER_SPEED_INTERMEDIATE = 0.3f;
    protected const float ANIMATOR_PARAMETER_SPEED_FAST = 0.5f;
    protected const string ANIMATOR_PARAMETER_DIRECTION_NAME = "Direction";
    protected const int ANIMATOR_PARAMETER_DIRECTION_FORWARD = 0;
    protected const int ANIMATOR_PARAMETER_DIRECTION_LEFT = 1;
    protected const int ANIMATOR_PARAMETER_DIRECTION_BACK = 2;
    protected const int ANIMATOR_PARAMETER_DIRECTION_RIGHT = 3;

    public delegate void EndAttackDelegate(int attackerId);
    public static EndAttackDelegate EndAttack;





    protected Animator animator;

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
        Attacking,
        Dieing
    }

    protected CharacterState _currentState;
    public CharacterState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    protected BoxCollider2D boxCollider;






    private void Awake()
    {
        Debug.Assert(turnCircle, "Character: turnCircle not found");
        Debug.Assert(turnCircle.CompareTag(GameManager.TAG_VISUAL_EFFECT), "Character: turnCircle not found");
        turnCircle.SetActive(false);

        Debug.Assert(AttackEffect, "Character: attackEffect not found");

        boxCollider = GetComponent<BoxCollider2D>();
        Debug.Assert(boxCollider, "Character: boxCollider component not found");

        animator = GetComponent<Animator>();
        Debug.Assert(animator, "Character: animator component not found");
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);

        

        CurrentState = CharacterState.Spectating;
    }

    private void OnEnable()
    {
        EndAttack += onEndAttack;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        EndAttack -= onEndAttack;
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

        string playerTag = GameManager.TAG_PLAYER;

        result = FindTaggedCharacterAtPosition(playerTag, transform.position + Vector3.down);
        if (result && result.CompareTag(GameManager.TAG_PLAYER))
            return result;

        result = FindTaggedCharacterAtPosition(playerTag, transform.position + Vector3.left);
        if (result && result.CompareTag(GameManager.TAG_PLAYER))
            return result;

        result = FindTaggedCharacterAtPosition(playerTag, transform.position + Vector3.up);
        if (result && result.CompareTag(GameManager.TAG_PLAYER))
            return result;

        result = FindTaggedCharacterAtPosition(playerTag, transform.position + Vector3.right);
        if (result && result.CompareTag(GameManager.TAG_PLAYER))
            return result;

        return null;
    }

    protected Character FindTaggedCharacterAtPosition(string TAG, Vector3 location)
    {
        //I don't want to detect a collision on the extreme limit of a collider
        float boundaryCorrection = 0.9f;
        Vector2 pointA = (Vector2)location + boxCollider.size / 2 * boundaryCorrection;
        Vector2 pointB = (Vector2)location - boxCollider.size / 2 * boundaryCorrection;
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB);

        

        foreach(Collider2D coll in colliders)
            if (coll.CompareTag(TAG))
            {
                return coll.gameObject.GetComponent<Character>();
            }
                


        return null;
    }

    virtual protected void Attack(Character target) 
    {
        CurrentState = CharacterState.Attacking;
        TurnToCharacter(target);
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_FAST);

        SpecialEffect spawnedEffect = Instantiate(AttackEffect);
        spawnedEffect.transform.position = target.transform.position;

        float attackDuration = spawnedEffect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        IEnumerator coroutine = target.ReceiveAttack(this, attackDuration);
        StartCoroutine(coroutine);
    }

    protected void TurnToCharacter(Character target)
    {
        Vector3 direction = target.transform.position - transform.position;

        if(Mathf.Abs(direction.y) < Mathf.Abs(direction.x))
        {
            if (direction.x < 0)
                animator.SetInteger(Character.ANIMATOR_PARAMETER_DIRECTION_NAME, Character.ANIMATOR_PARAMETER_DIRECTION_LEFT);
            else
                animator.SetInteger(Character.ANIMATOR_PARAMETER_DIRECTION_NAME, Character.ANIMATOR_PARAMETER_DIRECTION_RIGHT);
        }
        else
        {
            if(direction.y < 0)
                animator.SetInteger(Character.ANIMATOR_PARAMETER_DIRECTION_NAME, Character.ANIMATOR_PARAMETER_DIRECTION_FORWARD);
            else
                animator.SetInteger(Character.ANIMATOR_PARAMETER_DIRECTION_NAME, Character.ANIMATOR_PARAMETER_DIRECTION_BACK);
        }
    }

    virtual protected IEnumerator ReceiveAttack(Character attacker, float AttackDuration)
    {
        TurnToCharacter(attacker);
        
        yield return new WaitForSeconds(AttackDuration);

        int damage = Mathf.Min(1, attacker.AttackStat / DefenseStat);
        SetStamina(Stamina - damage);
    }

    virtual protected void onEndAttack(int attackerId)
    {
        if (attackerId != CharacterId)
            return;

        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);
        StartCoroutine("EndMyTurn");
    }


    virtual public IEnumerator EndMyTurn()
    {
        yield return new WaitForSeconds(postTurnWait);
        
        DisableTurnUI();
        TurnManager.EndTurn(CharacterId);
    }

    virtual protected void DisableTurnUI()
    {
        turnCircle.SetActive(false);
        CurrentState = CharacterState.Spectating;
    }
}
