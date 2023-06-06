using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards 
{
    public class PlayerTable : MonoBehaviour, IDropHandler,IPointerEnterHandler,IPointerExitHandler
    {
        public int _cardsOnTable;
        public List<Card> _cardsOnTablePlayer1;
        public List<Card> _cardsOnTablePlayer2;
        [SerializeField]
        private PlayerHand _playerHand = null;
        public FieldType _defaultCardPlaceType;
        [SerializeField]
        private GameManager _gameManager = null;

        public void OnDrop(PointerEventData eventData)
        {
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card&&card.State!=CardStateType.InDeck)
            {
                if ((_gameManager._player1turn == true && card._cardPlaceType == FieldType.Player1Hand&&_defaultCardPlaceType==FieldType.Player1Table && _gameManager._player1ManaPool>=card._costInt) || 
                    (_gameManager._player1turn == false && card._cardPlaceType == FieldType.Player2Hand&& _defaultCardPlaceType == FieldType.Player2Table && _gameManager._player2ManaPool >= card._costInt))
                {
                    if (card._onDrag == true && _cardsOnTable < 7)
                    {
                        card.transform.SetParent(transform);
                        card.transform.localScale = card._standartCardScale;
                        card.State = CardStateType.OnTable;
                        card._cardPlaceType = _defaultCardPlaceType;
                        if(card.Ability==TypeByDescription.Charge)
                        {
                            card.CanAttack = true;
                            card.HighlightCard();
                        }
                        if (transform != card._defaultParentCard && card._cardPlaceType == FieldType.Player1Table)
                        {
                            _gameManager.ReduceMana(true, card._costInt);
                            _cardsOnTablePlayer1.Add(card);
                            _cardsOnTable++;
                        }
                        else if (transform != card._defaultParentCard && card._cardPlaceType == FieldType.Player2Table)
                        {
                            _gameManager.ReduceMana(false, card._costInt);
                            _cardsOnTablePlayer2.Add(card);
                            _cardsOnTable++;
                        }
                        //_gameManager.CheckCardForAvailability();
                        _playerHand.MoveInsideHandMethod(card._defaultParentCard);
                    }

                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card.State ==CardStateType.OnTable && card._cardPlaceType != _defaultCardPlaceType) return;
            if ((_gameManager._player1turn == true && card._cardPlaceType == FieldType.Player1Hand && _defaultCardPlaceType == FieldType.Player1Table) ||
                    (_gameManager._player1turn == false && card._cardPlaceType == FieldType.Player2Hand && _defaultCardPlaceType == FieldType.Player2Table))
            {
                if (card && _cardsOnTable < 7)
                {
                    card._defaultTempCardParent = transform;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card && card._defaultTempCardParent==transform)
            {
                card._defaultTempCardParent = card._defaultParentCard;
            }
        }
    }
}
