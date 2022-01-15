using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PotionManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeReference, Tooltip("Image for UI potion icon")]
    private Image potionImage;
    [SerializeReference, Tooltip("Visual effect applied behind potion image")]
    private Image loopingRedSparkling;
    [SerializeReference, Tooltip("Visual effect applied to characters")]
    private SpecialEffect redSparkling;

    public delegate void PotionFoundDelegate();
    public static PotionFoundDelegate PotionFound;

    private int _potionCounter;
    public int PotionCounter
    {
        get { return _potionCounter; }
        set { _potionCounter = value;
            potionCounterText.text = value.ToString();
        }
    }

    private Text potionCounterText;
    private Image flyingPotion;
    private Image flyingSparkling;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        Debug.Assert(parentCanvas, "PotionManager: Canvas parent not found");

        potionCounterText = GetComponentInChildren<Text>();
        Debug.Assert(potionCounterText, "PotionManager: Text child component not found");

        Debug.Assert(potionImage && loopingRedSparkling, "PotionManager: potionImage reference not found");

        loopingRedSparkling.gameObject.SetActive(false);

        flyingPotion = null;
        PotionCounter = 3;
    }

    private void OnEnable()
    {
        PotionFound += onPotionFound;
    }

    private void Update()
    {
        //enable/disable flying sparkling when is flying over a player
        if(flyingPotion != null)
        {
            GameObject obj = LevelManager.GetGameObjectAtLocation(flyingPotion.transform.position);
            if (obj != null && obj.GetComponent<PlayerCharacter>() && !flyingSparkling.gameObject.activeSelf)
                flyingSparkling.gameObject.SetActive(true);
            else if((obj == null || !obj.GetComponent<PlayerCharacter>()) && flyingSparkling.gameObject.activeSelf)
                flyingSparkling.gameObject.SetActive(false);

        }
    }

    private void OnDisable()
    {
        onEndDrag();
        onPointerExit();
        PotionFound -= onPotionFound;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LevelManager.Instance.GetCurrentCharacter().CurrentState != Character.CharacterState.Idle || PotionCounter <= 0)
            return;

        flyingPotion = Instantiate<Image>(potionImage, parentCanvas.transform);
        flyingPotion.GetComponent<Canvas>().sortingLayerName = GameManager.SORTING_LAYER_NAME_UI;
        flyingSparkling = flyingPotion.transform.GetChild(0).GetComponent<Image>();
        flyingSparkling.GetComponent<Canvas>().sortingLayerName = GameManager.SORTING_LAYER_NAME_UI;
        flyingSparkling.gameObject.SetActive(false);
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (flyingPotion == null)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        flyingPotion.transform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag();
    }

    //I need my version without event parameters, to call it onDisable too
    private void onEndDrag()
    {
        if (flyingPotion == null)
            return;

        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
        Destroy(flyingPotion.gameObject);
        flyingPotion = null;

        //This prevents using potions while already moving with keyboard arrows
        //Must be done after the destroy of the flying potion
        if (LevelManager.Instance.GetCurrentCharacter().CurrentState != Character.CharacterState.Idle)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject obj = LevelManager.GetGameObjectAtLocation(mousePosition);
        if (obj != null && obj.CompareTag(GameManager.TAG_PLAYER))
        {
            PlayerCharacter targetPlayer = obj.GetComponent<PlayerCharacter>();
            targetPlayer.ReceivedPotion();
            PotionCounter--;
            Instantiate<SpecialEffect>(redSparkling, targetPlayer.transform);
            PlayerCharacter.UseItem(LevelManager.Instance.turnManager.ActiveCharacterIdCurrentTurn());
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LevelManager.Instance.GetCurrentCharacter().CurrentState != Character.CharacterState.Idle)
            return;

        if (GameManager.Instance.getMouseIcon() != GameManager.MouseIcon.Default)
            return;

        loopingRedSparkling.gameObject.SetActive(true);
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Selectable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit();
    }

    public void onPointerExit()
    {
        if (GameManager.Instance.getMouseIcon() != GameManager.MouseIcon.Selectable)
            return;

        if (flyingPotion == null)
            GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);

        loopingRedSparkling.gameObject.SetActive(false);
    }


    private void onPotionFound()
    {
        PotionCounter++;
    }

}
