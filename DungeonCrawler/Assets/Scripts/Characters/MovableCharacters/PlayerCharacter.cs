using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MovableCharacter
{
    [SerializeField, Tooltip("Starting main character")]
    protected TurnCanvas turnCanvas;

    protected TurnCanvas playerTurnCanvas;
    public delegate void UseItemDelegate(int characterId);
    public static UseItemDelegate UseItem;


    protected DirectionalActions directionalUI;

    override protected void Awake()
    {
        base.Awake();

        Debug.Assert(turnCanvas, "PlayerCharacter: CanvasRenderer reference not found");
        playerTurnCanvas =  Instantiate<TurnCanvas>(turnCanvas);

        if (playerTurnCanvas.gameObject.activeSelf)
            playerTurnCanvas.gameObject.SetActive(false);
        playerTurnCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        directionalUI = GetComponentInChildren<DirectionalActions>();
        Debug.Assert(directionalUI, "PlayerCharacter: directionalActions child not found");
        directionalUI.gameObject.SetActive(false);
    }

    override protected void OnEnable()
    {
        base.OnEnable();
        UseItem += onUseItem;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMyTurn() && CurrentState == CharacterState.Moving)
        {
            
            transform.position = Vector3.MoveTowards(transform.position, movementDestination, LevelManager.Instance.CellSize * Time.deltaTime / movementDuration);
            if (transform.position == movementDestination)
                EndMovement();

        }

        if (CurrentState == CharacterState.Dieing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a =  Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
                Die();
                
        }
    }

    override protected void OnDisable()
    {
        base.OnDisable();
        UseItem -= onUseItem;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Potion>())
        {
            Destroy(collision.gameObject);
            if(PotionManager.PotionFound != null)
                PotionManager.PotionFound();
        }
                    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameManager.TAG_ENEMY_SPAWNER))
            collision.gameObject.GetComponent<EnemySpawn>().enteringPlayer();
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameManager.TAG_ENEMY_SPAWNER))
            collision.gameObject.GetComponent<EnemySpawn>().exitingPlayer();
            
    }







    public override IEnumerator PlayTurn()
    {
        SetupStartingTurnUI();

        //The player doesn't need to wait
        yield return new WaitForSeconds(0);
        CurrentState = CharacterState.Idle;
    }
    
    override protected void SetupStartingTurnUI()
    {
        turnCircle.SetActive(true);
        
        directionalUI.gameObject.SetActive(true);
        SetupDirectionalUI();
        SetPlayerTurnCanvasActive(true);

    }

    public void SetPlayerTurnCanvasActive(bool active)
    {
        if (playerTurnCanvas.gameObject.activeSelf != active)
            playerTurnCanvas.gameObject.SetActive(active);

        if(active)
            playerTurnCanvas.SetInputsActive(true);
    }

    protected void SetupDirectionalUI()
    {
        float cellSize = LevelManager.Instance.CellSize;
        Vector3 position = transform.position;

        SetMoveForSingleDirectionalUI(DirectionalActions.Direction.up, transform.position + Vector3.up);
        SetMoveForSingleDirectionalUI(DirectionalActions.Direction.down, transform.position + Vector3.down);
        SetMoveForSingleDirectionalUI(DirectionalActions.Direction.left, transform.position + Vector3.left);
        SetMoveForSingleDirectionalUI(DirectionalActions.Direction.right, transform.position + Vector3.right);
    }

    protected void SetMoveForSingleDirectionalUI(DirectionalActions.Direction dir, Vector3 position)
    {
        if (LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_ENEMY, position, boxCollider.size))
            directionalUI.SetAction(dir, DirectionalActions.Action.Attack);
        else if (LevelManager.GetTaggedObjectAtNearPosition<Item>(GameManager.TAG_ITEM, position, boxCollider.size))
            directionalUI.SetAction(dir, DirectionalActions.Action.Move);
        else if (!LevelManager.GetGameObjectAtLocation(position))
            directionalUI.SetAction(dir, DirectionalActions.Action.Move);
        else //C'è qualcosa, ma non è nulla di interagibile. Lo si tratta da ostacolo
            directionalUI.SetAction(dir, DirectionalActions.Action.None);

    }

    
    public void MoveButtonPressed(DirectionalActions.Direction dir)
    {
        DisableInputTurnUI();
        Vector3 movementDirection = Vector3.zero;

        switch (dir)
        {
            case DirectionalActions.Direction.up:
                movementDirection = Vector3.up;
                break;
            case DirectionalActions.Direction.down:
                movementDirection = Vector3.down;
                break;
            case DirectionalActions.Direction.right:
                movementDirection = Vector3.right;
                break;
            case DirectionalActions.Direction.left:
                movementDirection = Vector3.left;
                break;
        }

        MoveTo(movementDirection);
    }

    public void AttackButtonPressed(DirectionalActions.Direction dir)
    {
        DisableInputTurnUI();
        Vector3 enemyLocation = Vector3.zero;

        switch (dir)
        {
            case DirectionalActions.Direction.up:
                enemyLocation = transform.position + Vector3.up;
                break;
            case DirectionalActions.Direction.down:
                enemyLocation = transform.position + Vector3.down;
                break;
            case DirectionalActions.Direction.right:
                enemyLocation = transform.position + Vector3.right;
                break;
            case DirectionalActions.Direction.left:
                enemyLocation = transform.position + Vector3.left;
                break;
        }

        attackedCharacter = LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_ENEMY, enemyLocation, boxCollider.size);
        Attack(attackedCharacter);
    }

    public void SleepButtonPressed()
    {
        CurrentState = CharacterState.Busy;
        DisableInputTurnUI();

        StartCoroutine("EndMyTurn");
    }

    public void ReceivedPotion()
    {
        Debug.Log(Potion.HealingAmount);
        Debug.Log(Stamina);
        SetStamina(Mathf.Min(MaxStaminaStat, Stamina + Potion.HealingAmount));
        Debug.Log(Stamina);
    }

    public void onUseItem(int characterId)
    {
        if (characterId != CharacterId)
            return;

        DisableInputTurnUI();
        CurrentState = CharacterState.Busy;
    }

    protected void DisableInputTurnUI()
    {
        directionalUI.gameObject.SetActive(false);
        playerTurnCanvas.SetInputsActive(false);
    }

    override protected void DisableTurnUI()
    {
        SetPlayerTurnCanvasActive(false);
        turnCircle.SetActive(false);
        CurrentState = CharacterState.Spectating;
    }

    
}
