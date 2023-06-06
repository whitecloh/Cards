using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cards
{
    public class CreateDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
    {
        public int _cardsOnMenuTable;
        [SerializeField]
        private Button _deckReady = null;
        private Card _card;
        public List<Card> _deck;
        private MenuManager _manager;
        public List<List<Card>> _decks;
        private string _inputName;
        private int index;
        private void Awake()
        {
            _deck = new List<Card>();
            _decks = new List<List<Card>>();
            _manager = GameObject.Find("MenuCanvas").GetComponent<MenuManager>();
            _deckReady.onClick.AddListener(DeckReady);
        }
        private void Update()
        {
            if(_cardsOnMenuTable<10)
            {
                _deckReady.interactable = false;
            }
        }
        public void OnDrop(PointerEventData eventData)
        {
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card)
            {
                if (card._onDrag == true && _cardsOnMenuTable < 10)
                {
                    card.transform.SetParent(transform);
                    card.transform.localScale = card._standartCardScale;
                    card.State = CardStateType.InMenu;

                    if (transform != card._defaultParentCard)
                    { 
                        _cardsOnMenuTable++;

                       _deck.Add(card);
                    }
                }
                if (_cardsOnMenuTable == 10)
                {
                    _deckReady.interactable = true;
                   
                }

            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card && _cardsOnMenuTable < 10)
            {

                card._defaultTempCardParent = transform;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            if (eventData.pointerDrag == null) return;
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card && card._defaultTempCardParent == transform)
            {
                card._defaultTempCardParent = card._defaultParentCard;
            }
        }
        private void DeckReady()
        {
            for (int i = 0; i < _deck.Count; i++)
            { 
                _card = Instantiate(_deck[i], _manager._allReadyDecks[_decks.Count].transform.GetChild(2).transform);
                _card.gameObject.SetActive(false);

            }
            _manager._hero = Instantiate(_manager._hero, _manager._allReadyDecks[_decks.Count].transform);
            _manager._hero.transform.localPosition = new Vector3(-350, 0, 0);
            _manager._hero.transform.localScale = new Vector3(120, 1, 160);
            _inputName = _manager._inputDeckName.text;
            _manager._inputDeckName.text = default;

            _decks.Add(_deck);
            Debug.Log(_decks[index].Count);
            index++;
            if (_decks.Count > 0)
            {
                _manager._allReadyDecks[_decks.Count - 1].transform.GetChild(0).GetComponent<Text>().text = _manager._deckName;
                if(_inputName=="")
                {
                    _inputName = _manager._deckName;
                }
                _manager._allReadyDecks[_decks.Count - 1].transform.GetChild(1).GetComponent<Text>().text = _inputName;
            }

           var objects = GameObject.FindGameObjectsWithTag("Card");
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(false);
            }
            _deck = new List<Card>();
            _manager.OpenAllDecks();
        }

    }
}
