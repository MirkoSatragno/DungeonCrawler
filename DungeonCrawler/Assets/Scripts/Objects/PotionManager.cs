using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PotionManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeReference]
    private Image potionImage;
    [SerializeReference]
    private SpecialEffect redSparkling;
    [SerializeReference]
    private Image loopingRedSparkling;
    
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


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PotionCounter <= 0)
            return;
        flyingPotion = Instantiate<Image>(potionImage, parentCanvas.transform);
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
        if (flyingPotion == null)
            return;

        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
        Destroy(flyingPotion.gameObject);
        flyingPotion = null;

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
        loopingRedSparkling.gameObject.SetActive(true);
        GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Selectable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(flyingPotion == null)
            GameManager.Instance.setMouseIcon(GameManager.MouseIcon.Default);
        
        loopingRedSparkling.gameObject.SetActive(false);
    }
}
