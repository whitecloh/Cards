using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {
        public Card[] _cardsInHand;
        [SerializeField]
        private Transform[] _positions = null;
        [SerializeField]
        private Transform[] _startCardPos = null;
        private bool _isSwitchVisual;
        private int _indexLastNullPositionInHand;

        public FieldType _cardPlaceType;

        private void Start()
        {
            _cardsInHand = new Card[_positions.Length];
        }
        public bool SetStartHand(Card[] card)
        {
            if (card == null) return true;

            for (int i = 0; i < 3; i++)
            {
                card[i].SwitchVisual();
                card[i]._cardPlaceType = FieldType.Player1Table;
                card[i].transform.SetParent(_startCardPos[i]);
                StartCoroutine(MoveInHand(card[i], _startCardPos[i]));
            }
            return true;
        }

        public bool SetNewCard (Card card)
        {
            if (card == null) return true;
            
            int indexDeckToHand = GetLastPosition();
            _indexLastNullPositionInHand = indexDeckToHand;
            if (indexDeckToHand==-1)
            {
                Destroy(card.gameObject);
                return false;
            }
            Debug.Log($"index - {indexDeckToHand} ");
            _cardsInHand[indexDeckToHand] = card;
            _cardsInHand[indexDeckToHand].transform.SetParent(_positions[indexDeckToHand]);
            _cardsInHand[indexDeckToHand]._defaultParentCard = _positions[indexDeckToHand];
            _isSwitchVisual = true;
            StartCoroutine(MoveInHand(card, _positions[indexDeckToHand]));

            return true;
        }

        public int GetLastPosition()
        {
            for(int i =0; i<_cardsInHand.Length;i++)
            {
                if (_cardsInHand[i] == null) return i;
            }
            return -1;
        }
        private int GetLastPositionForMoveInsideHand(int n)
        {
            for (int i = 0; i < _cardsInHand.Length; i++)
            {
                if (_cardsInHand[i] == null) return i+n;
            }
            return _cardsInHand.Length+n;
        }

        public void MoveInsideHandMethod(Transform _pos)
        {
           for (int i=0;i<_positions.Length;i++)
            {
                if(_pos==_positions[i])
                {
                    _indexLastNullPositionInHand = GetLastPositionForMoveInsideHand(-1);
                    Debug.Log(_indexLastNullPositionInHand);
                    for (int j = i; j < _indexLastNullPositionInHand ; j++)
                    {    
                        _isSwitchVisual = false;
                        StartCoroutine(MoveInHand(_cardsInHand[j+1], _positions[j]));
                        _cardsInHand[j] = _cardsInHand[j+1];
                        _cardsInHand[j+1].transform.SetParent(_positions[j]);                       
                    }
                    _cardsInHand[_indexLastNullPositionInHand] = null;
                    _cardsInHand[_positions.Length - 1] = null;
                }
            }

        }
        private IEnumerator MoveInHand(Card card , Transform parent)
        {
            float timeToEnd = 1f;
            if(_isSwitchVisual)
            {
                //card.SwitchVisual();
            }
            var time = 0f;
            var startPosition = card.transform.position;
            var endPosition = parent.position;
            while (time<timeToEnd)
            {
                card.transform.position = Vector3.Lerp(startPosition, endPosition,time);
                time += Time.deltaTime;
                yield return null;
            }
            if (card._cardPlaceType == FieldType.Player1Hand || card._cardPlaceType == FieldType.Player2Hand)
            { card.State = CardStateType.InHand; }
            else
            {
                card.State = CardStateType.OnStartMenu;
            }
            EventSystem _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            _eventSystem.enabled = true;
        }
    }
}
