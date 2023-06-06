using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackedCard : MonoBehaviour, IDropHandler
{
    private GameManager _manager;
    public bool isEnable;

    private void Update()
    {
        if (this.enabled)
        {
            isEnable = true;
        }
        else { isEnable = false; }
    }

    private void Awake()
    {
        if (PlayerDeckStatic.SceneNumber == 1)
        {
            _manager = GameObject.Find("Board").GetComponent<GameManager>();
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();
        if (card && card.CanAttack && 
            (transform.GetComponent<Card>()._cardPlaceType==FieldType.Player1Table||            
             transform.GetComponent<Card>()._cardPlaceType==FieldType.Player2Table)
            )
        {
            if(transform.GetComponent<Card>()._cardPlaceType!=card._cardPlaceType)
            {
                card.ChangeAttackState(false);
                card.DeHighlightCard();
                _manager.CardsFight(card, transform.GetComponent<Card>());
            }
        }
    }
}
