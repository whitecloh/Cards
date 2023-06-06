using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
    {
        [SerializeField]
        private GameObject _frontCard = null;
        [SerializeField]
        private MeshRenderer _picture = null;
        [SerializeField]
        private TextMeshPro _cost = null;
        public int _costInt;
        [SerializeField]
        private TextMeshPro _name = null;
        [SerializeField]
        private TextMeshPro _description = null;
        [SerializeField]
        private TextMeshPro _attack = null;
        public int _attackInt;
        [SerializeField]
        private TextMeshPro _type = null;
        [SerializeField]
        private TextMeshPro _health = null;
        public int _healthsInt;
        private MenuManager _menuManager;
        [SerializeField]
        private GameObject _highlightObj = null;
        [SerializeField]
        private GameObject _tauntObj = null;
        public Canvas _mainCanvas;
        public bool _onDrag;
        public bool _isDraggable;
        public Vector3 _standartCardScale;
        private Vector3 _standartCardPosition;
        public Transform _defaultParentCard;
        public Transform _defaultTempCardParent;
        private GameObject TempCardGO;
        private CreateDeck _createDeck;
        public FieldType _cardPlaceType;
        public bool CanAttack=false;

        public void Configuration(Material picture, CardPropertiesData data, string description)
        {
            _picture.sharedMaterial = picture;
            _cost.text = data.Cost.ToString();
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _type.text = data.Type == CardUnitType.None ? string.Empty : data.Type.ToString();
            _health.text = data.Health.ToString();
            _healthsInt = int.Parse(_health.text);
            _attackInt = int.Parse(_attack.text);
            _costInt = int.Parse(_cost.text);

            AbilitiesTauntAndCharge();
            AbilitiesBattlecry();
            if (Ability == TypeByDescription.Taunt)
            {
                _tauntObj.SetActive(true);
            }
            else
            {
                _tauntObj.SetActive(false);
            }
        }

        private void Awake()
        {

            if (PlayerDeckStatic.SceneNumber == 0)
            {
                _mainCanvas = GameObject.Find("MenuCanvas").GetComponent<Canvas>();
                _standartCardScale = transform.localScale;
                TempCardGO = GameObject.Find("MenuShadowCard");
            }
            else
            {
                _mainCanvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
                TempCardGO = GameObject.Find("ShadowCard");
                _standartCardScale = new Vector3(70, 1, 100);
            }
            _onDrag = false;
            _isDraggable = true;
            DeHighlightCard();

        }
        public void ChangeAttackState(bool can)
        {
            CanAttack = can;
        }
        public void HighlightCard()
        {
            _highlightObj.SetActive(true);
        }
        public void DeHighlightCard()
        {
            _highlightObj.SetActive(false);
        }
        public void GetDamage(int damage)
        {
            _healthsInt -= damage;
            _health.text = _healthsInt.ToString();
        }
        public void RefreshData()
        {
            _attack.text = _attackInt.ToString();
            _health.text = _healthsInt.ToString();
        }
        public void CheckForAvailability(int currentMana)
        {
            GetComponent<CanvasGroup>().alpha = currentMana >= _costInt ? 1 : 0.5f;
        }
        public void AbilitiesTauntAndCharge()
        {
            if (_description.text.Contains("Taunt"))
            {
                Ability = TypeByDescription.Taunt;
            }
            if (_description.text.Contains("Charge"))
            {
                Ability = TypeByDescription.Charge;
            }

        /*if (_description.text.Contains("..."))
            {
                this.Ability = TypeByDescription.Aura;
            }*/
        }
        public int[] AbilitiesBattlecry()
        {
            int[] i=new int[2];
            if (_description.text.Contains("Battlecry"))
            {
                Ability = TypeByDescription.Battlecry;
                if (_description.text.Contains("damage"))
                {
                    Battlecry = TypeOfBattlecry.DealDamage;
                    foreach(char c in _description.text)
                    {
                        if (Char.IsDigit(c))
                        {
                            i[0] = int.Parse(c.ToString());
                            break;
                        }
                    }
                }
                if (_description.text.Contains("Health"))
                {
                    Battlecry = TypeOfBattlecry.RestoreHealths;
                    foreach (char c in _description.text)
                    {
                        if (Char.IsDigit(c))
                        {
                            i[0] = int.Parse(c.ToString());
                        }
                    }
                }
                if (_description.text.Contains("Draw"))
                {
                    Battlecry = TypeOfBattlecry.DrawCard;
                    foreach (char c in _description.text)
                    {
                        if (Char.IsDigit(c))
                        {
                            i[0] = int.Parse(c.ToString());
                        }
                    }
                    if (i == null) i[0] = 1;
                }
                if (_description.text.Contains("Summon"))
                {
                    Battlecry = TypeOfBattlecry.Summon;
                }
                if(_description.text.Contains("Gain")|| _description.text.Contains("Give"))
                {
                    Battlecry = TypeOfBattlecry.Buff;
                    foreach (char c in _description.text)
                    {
                        if (Char.IsDigit(c))
                        {
                            for (int j = 0; j < 2; j++)
                            { i[j] = int.Parse(c.ToString()); }
                        }
                    }
                }
            }
            return i;
        }
        public bool HasAbility
        {
            get
            {
                return Ability!=TypeByDescription.None;
            }
        }

        public bool IsAlive
        {
            get
            {
                return _healthsInt>0;
            }
        }
        public CardStateType State { get; set; }
        public TypeByDescription Ability;
        public TypeOfBattlecry Battlecry;
        public TypeOfAura Aura;
        [ContextMenu("Switch Visual")]
        public void SwitchVisual() => _frontCard.SetActive(!IsFrontCard);
        public bool IsFrontCard => _frontCard.activeSelf;


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;
            _onDrag = true;
            _defaultParentCard =_defaultTempCardParent= transform.parent;           
            _standartCardPosition = transform.position;           
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            switch (State)
            {
                case CardStateType.InHand:
                    _mainCanvas.sortingOrder = 1;
                    TempCardGO.transform.SetParent(_defaultParentCard);
                    break;
                case CardStateType.OnTable:                    
                    break;
                case CardStateType.InMenu:
                    transform.localScale /= 2f;
                    break;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;
            Vector3 position = eventData.position;
            position.z = _mainCanvas.planeDistance;

            switch (State)
            {
                case CardStateType.InHand:
                    transform.position = _mainCanvas.worldCamera.ScreenToWorldPoint(position);
                    if (TempCardGO.transform.parent != _defaultTempCardParent)
                    {
                        TempCardGO.transform.SetParent(_defaultTempCardParent);
                    }
                    CheckPosition();
                    break;
                case CardStateType.OnTable:
                    transform.position = _mainCanvas.worldCamera.ScreenToWorldPoint(position);
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDraggable == false) return;  
            _onDrag = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            _mainCanvas.sortingOrder = 0;
            
            if (transform.parent == _defaultParentCard)
            {
                transform.position = _standartCardPosition;
            }
            else
            {
                transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
                transform.localPosition = new Vector3(0, 0, 0);
                _defaultParentCard = transform.parent;
            }
            TempCardGO.transform.SetParent(_mainCanvas.transform);
            TempCardGO.transform.localPosition = new Vector3(1100, 0, -100);

            transform.localScale = _standartCardScale;

            switch (State)
            {
                case CardStateType.InHand:
                    break;
                case CardStateType.OnTable:
                    break;
                case CardStateType.InMenu:
                    transform.localScale *= 2f;
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InHand:
                    if (_onDrag == false&&_isDraggable==true)
                    {
                        transform.localScale *= 1.5f;
                        transform.position += new Vector3(0f, 5f, 0f);
                    }                   
                    break;
                case CardStateType.OnTable:
                    if (_onDrag == false && _isDraggable == true)
                    {
                        transform.localScale *= 1.5f;
                        transform.position += new Vector3(0f, 5f, 0f);
                    }       
                    break;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            switch (State)
            {

                case CardStateType.InHand:
                    if (_onDrag == false && _isDraggable == true)
                    {
                        transform.localScale = _standartCardScale;
                        transform.position -= new Vector3(0f, 5f, 0f);
                    }
                    break;
                case CardStateType.OnTable:
                    if (_onDrag == false && _isDraggable == true)
                    {
                        transform.localScale = _standartCardScale;
                        transform.position -= new Vector3(0f, 5f, 0f);
                    }
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch(State)
            {
                case CardStateType.InMenu:
                    _createDeck = GameObject.Find("CreateDeckMenu").GetComponent<CreateDeck>();
                    _menuManager = GameObject.Find("MenuCanvas").GetComponent<MenuManager>();
                    for (int i = 0; i < _menuManager._deckPositions.Length; i++)
                    {
                       if (_menuManager._deckPositions[i].childCount ==0)
                        {
                            transform.position = _menuManager._deckPositions[i].position;
                            transform.SetParent(_menuManager._deckPositions[i]);
                            transform.localScale = _standartCardScale;
                            transform.position += new Vector3(0, 20f, 0);
                            State = CardStateType.InHand;
                            _createDeck._cardsOnMenuTable--;                                                         
                            _createDeck._deck.RemoveAt(_createDeck._deck.Count - 1);
                            break;
                        }
                        
                    }
                    break;
                case CardStateType.OnStartMenu:

                    var bannedCard = GameObject.Find("BannedCard");
                    if (transform.parent.childCount == 1)
                    {
                        var ban = Instantiate(bannedCard);
                        ban.transform.SetPositionAndRotation(transform.position, _mainCanvas.transform.rotation);
                        ban.transform.SetParent(transform.parent);
                    }
                    else
                    {
                       Destroy(transform.parent.GetChild(1).gameObject);
                    }
                        break;
                    
            }
            Debug.Log(State);
        }

        private void CheckPosition()
        {
            int newIndex = _defaultTempCardParent.childCount;
            for (int i = 0; i < _defaultTempCardParent.childCount; i++)
            {
                if (transform.position.x < _defaultTempCardParent.GetChild(i).position.x)
                {
                    newIndex = i;

                    if (TempCardGO.transform.GetSiblingIndex() < newIndex) newIndex--;
                    break;
                }
            }
            TempCardGO.transform.SetSiblingIndex(newIndex);
        }
    }

}