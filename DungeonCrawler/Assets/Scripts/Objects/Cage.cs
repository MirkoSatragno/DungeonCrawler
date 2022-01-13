using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, Range(0.5f, 10f)]
    protected float disappearingDuration = 2;

    protected SpriteRenderer sprite;
    private BoxCollider2D boxCollider;
    private bool vanishing;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        Debug.Assert(boxCollider, "Cage: boxCollider component not found");

        sprite = GetComponent<SpriteRenderer>();
        Debug.Assert(sprite, "Obstacle: spriteRenderer component not found");

        vanishing = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            if (CanBeFreed())
                FreePlayer();

        if (vanishing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a = Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
            {
                Character.EndAction(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());
                LevelManager.Instance.InstantiateCagedPlayer(transform.position + Vector3.down * 0.5f);
                Destroy(gameObject);
            }

        }
    }

    private void OnDisable()
    {
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
    }



    private bool CanBeFreed()
    {
        if (GameManager.Instance.IsGamePaused)
            return false;
        
        Character currentChar = LevelManager.Instance.GetCurrentCharacter();
        if (!currentChar.CompareTag(GameManager.TAG_PLAYER) || currentChar.CurrentState != Character.CharacterState.Idle)
            return false;

        Character character = LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_PLAYER, transform.position + Vector3.up, boxCollider.size);
        if (character != null && character.CharacterId == currentChar.CharacterId)
            return true;

        character = LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_PLAYER, transform.position + Vector3.down, boxCollider.size);
        if (character != null && character.CharacterId == currentChar.CharacterId)
            return true;

        character = LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_PLAYER, transform.position + Vector3.left, boxCollider.size);
        if (character != null && character.CharacterId == currentChar.CharacterId)
            return true;

        character = LevelManager.GetTaggedObjectAtNearPosition<Character>(GameManager.TAG_PLAYER, transform.position + Vector3.right, boxCollider.size);
        if (character != null && character.CharacterId == currentChar.CharacterId)
            return true;

        return false;
    }

    private void FreePlayer()
    {
        //Actually he's not using an item, but it's similar enough
        PlayerCharacter.UseItem(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());
        vanishing = true;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
    }
}
