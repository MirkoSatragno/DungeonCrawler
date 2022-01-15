using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField, Range(0, 100)]
    private int _attackStat=5;
    public int AttackStat
    {
        get { return _attackStat; }
    }
    [SerializeField, Range(0, 100)]
    private int _defenseStat;
    public int DefenseStat
    {
        get { return _defenseStat; }
    }
    [SerializeField, Range(0, 1000)]
    private int _maxStaminaStat;
    public int MaxStaminaStat
    {
        get { return _maxStaminaStat; }
    }

    [Space(20)]


    [SerializeReference, Tooltip("Animation under the player during his turn")]
    protected GameObject turnCircle;
    [SerializeReference, Tooltip("Special effect showed while attacking")]
    protected SpecialEffect AttackEffect;

    [SerializeField, Range(0.2f, 10f), Tooltip("Seconds waited before starting the turn")]
    protected float preTurnWait = 1f;
    [SerializeField, Range(0.2f, 10f), Tooltip("Seconds waited before ending the turn")]
    protected float postTurnWait = 1f;
    [SerializeField, Range(0.5f, 10f), Tooltip("Seconds duration of vanisging animation when killed")]
    public float disappearingDuration = 2;

    protected const string ANIMATOR_PARAMETER_SPEED_NAME = "Speed";
    protected const float ANIMATOR_PARAMETER_SPEED_SLOW = 0.1f;
    protected const float ANIMATOR_PARAMETER_SPEED_INTERMEDIATE = 0.3f;
    protected const float ANIMATOR_PARAMETER_SPEED_FAST = 0.5f;
    protected const string ANIMATOR_PARAMETER_DIRECTION_NAME = "Direction";
    protected const int ANIMATOR_PARAMETER_DIRECTION_FORWARD = 0;
    protected const int ANIMATOR_PARAMETER_DIRECTION_LEFT = 1;
    protected const int ANIMATOR_PARAMETER_DIRECTION_BACK = 2;
    protected const int ANIMATOR_PARAMETER_DIRECTION_RIGHT = 3;

    public delegate void EndActionDelegate(int attackerId);
    public static EndActionDelegate EndAction;





    protected Animator animator;
    [HideInInspector]
    public BoxCollider2D boxCollider;
    protected HealthBar healthBar;
    protected SpriteRenderer sprite;



    private int _characterId;
    public int CharacterId
    {
        get { return _characterId; }
        set { _characterId = value; }
    }
    private int _stamina;
    protected int Stamina
    {
        get { return _stamina; }
        set { _stamina = value; }
    }
    

    public enum CharacterState
    {
        Spectating,
        Idle,
        Moving,
        Attacking,
        Busy, //special ability, if any is available
        Dieing
    }

    protected CharacterState _currentState;
    public CharacterState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    protected Character attackedCharacter;

    






    virtual protected void Awake()
    {
        Debug.Assert(turnCircle, "Character: turnCircle not found");
        turnCircle.SetActive(false);

        Debug.Assert(AttackEffect, "Character: attackEffect not found");

        boxCollider = GetComponent<BoxCollider2D>();
        Debug.Assert(boxCollider, "Character: boxCollider component not found");

        healthBar = GetComponentInChildren<HealthBar>();
        Debug.Assert(healthBar, "Character: HealthBar child not found");

        animator = GetComponent<Animator>();
        Debug.Assert(animator, "Character: animator component not found");
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);

        sprite = GetComponent<SpriteRenderer>();
        Debug.Assert(sprite, "Character: spriteRenderer component not found");



        Stamina = MaxStaminaStat;
        CurrentState = CharacterState.Spectating;
        attackedCharacter = null;
    }

    virtual protected void OnEnable()
    {
        EndAction += onEndAction;
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual protected void OnDisable()
    {
        EndAction -= onEndAction;
    }





    public void SetStamina(int stamina)
    {
        Stamina = Mathf.Max(0, stamina);
        healthBar.SetHealthBar((float)stamina / (float)MaxStaminaStat);

        if (Stamina == 0)
            StartDieing();
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

    

    virtual protected void Attack(Character target) 
    {
        CurrentState = CharacterState.Attacking;
        TurnToTarget(target.transform.position);
        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_FAST);

        SpecialEffect spawnedEffect = Instantiate(AttackEffect);
        spawnedEffect.transform.position = target.transform.position;

        float attackDuration = spawnedEffect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        IEnumerator coroutine = target.ReceiveAttack(this, attackDuration);
        StartCoroutine(coroutine);
    }

    protected void TurnToTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;

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
        TurnToTarget(attacker.transform.position);
        
        yield return new WaitForSeconds(AttackDuration);

        int damage = Mathf.Max(1, attacker.AttackStat / DefenseStat);
        SetStamina(Stamina - damage);
    }

    virtual protected void onEndAction(int characterId)
    {
        if (characterId != CharacterId)
            return;

        animator.SetFloat(ANIMATOR_PARAMETER_SPEED_NAME, ANIMATOR_PARAMETER_SPEED_SLOW);
        StartCoroutine("EndMyTurn");
    }


    virtual public IEnumerator EndMyTurn()
    {
        //better wait before the possible death wait, to be sure the player has already entered dieing state
        yield return new WaitForSeconds(postTurnWait);

        if (attackedCharacter && attackedCharacter.CurrentState == CharacterState.Dieing)
            yield return new WaitForSeconds(attackedCharacter.disappearingDuration);

        
        DisableTurnUI();
        TurnManager.EndTurn(CharacterId);
    }

    virtual protected void DisableTurnUI()
    {
        turnCircle.SetActive(false);
        CurrentState = CharacterState.Spectating;
    }

    protected void StartDieing()
    {
        CurrentState = CharacterState.Dieing;
    }

    virtual protected void Die()
    {
        Destroy(gameObject);
        LevelManager.Instance.RemoveCharacter(CharacterId);
    }
}
