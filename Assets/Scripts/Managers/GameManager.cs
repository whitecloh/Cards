using Cards.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cards
{

    public class GameManager : MonoBehaviour
    {
        
        private Material _baseMat;
        private CardPropertiesData[] _allCards;
        [SerializeField]
        private CardPackConfiguration[] _packs = null;
        [SerializeField]
        private int _cardsInDeck = 10;
        [SerializeField]
        private Card _cardPrefab = null;
        [Space, SerializeField]
        private Transform _deck1Parent = null;
        [SerializeField]
        private Transform _deck2Parent = null;
        [SerializeField]
        private PlayerHand _player1 = null;
        [SerializeField]
        private PlayerHand _player2 = null;
        [SerializeField]
        private PlayerTable _player1Table = null;
        [SerializeField]
        private PlayerTable _player2Table = null;

        public Card[] _player1Cards;
        private Card[] _player2Cards;

        [SerializeField]
        private Button _endTurn = null;
        [SerializeField]
        private Text _turnTimeTxt = null;
        private int _turnTime = 30;
        [SerializeField]
        private Transform _axisCamera = null;
        [SerializeField]
        private Transform[] _startCards = null;
        [SerializeField]
        private Button _startGame = null;

        public bool _player1turn;
        private Vector3 _tablePosition;
        [SerializeField]
        private Text _mana = null;
        public int _player1ManaPool;
        public int _player2ManaPool;
        private int _manaInt;

        private Heroes _player1Hero;
        [SerializeField]
        private Heroes _player2Hero = null;
        public int _player1Healths;
        public int _player2Healths;

        [SerializeField]
        private Transform _player1HeroPlace = null;
        [SerializeField]
        private GameObject _resultsGO = null;
        [SerializeField]
        private TextMeshPro _resultsTxt = null;
        public int SceneNumber;
        private int _damageNoDeckPlayer1=10;
        private int _damageNoDeckPlayer2=10;

        private void Awake()
        {
            PlayerDeckStatic.SceneNumber = 1;
            _resultsGO.SetActive(false);
            IEnumerable<CardPropertiesData> cards = new List<CardPropertiesData>();
            foreach (var pack in _packs) 
            { 
                cards = pack.UnionProperties(cards); 
            }
            _allCards = cards.ToArray();
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 3006;

        _endTurn.onClick.AddListener(EndTurn);
            _manaInt = 1;
            _player1ManaPool = _manaInt;
            _player2ManaPool = _manaInt;
            ShowMana(_manaInt);
        }
        private void Start()
        {
            _startGame.onClick.AddListener(DeckReady);
            _player1turn = true;

            _player1Cards = CreatePlayerDeck(_deck1Parent);
            _player2Cards = CreateEnemyDeck(_deck2Parent);

            _player1Hero = Instantiate(PlayerDeckStatic._player1Hero,_player1HeroPlace);
            _player1Hero.transform.localPosition = new Vector3(-460, -215, -50);
            _player1Hero.transform.localEulerAngles = new Vector3(90,0,0);
            _player1Hero.GetComponent<AttackedHero>().enabled = true;
            _player2Hero.GetComponent<AttackedHero>().enabled = true;
            _player1Healths = int.Parse(_player1Hero._hp.text);
            _player2Healths = int.Parse(_player2Hero._hp.text);

            GameStart();
        } 

        private void DeckReady()
        {           
            for(int i = 0;i<_startCards.Length;i++)
            {
                if(_startCards[i].transform.childCount>1)
                {
                    Card card = _startCards[i].GetChild(0).GetComponent<Card>();
                    card.transform.SetParent(_deck1Parent);
                    card.SwitchVisual();
                    card.transform.SetAsFirstSibling();
                    card.transform.position = _deck1Parent.transform.position;
                }
                else
                {
                    Card card = _startCards[i].GetChild(0).GetComponent<Card>();
                    card.transform.SetParent(_deck1Parent);
                    card.transform.SetAsLastSibling();
                    card.SwitchVisual();
                    card.transform.position = _deck1Parent.transform.position;
                }
            }
            for(int i =0;i<_cardsInDeck;i++)
            {
                _player1Cards[i] = _deck1Parent.transform.GetChild(i).GetComponent<Card>();
            }

            for (int i = 0; i < 3; i++)
            {
                AddCardPlayer1();
                AddCardPlayer2();
            }
            StartCoroutine(TimeTurn());
        }
        private void GameStart()
        {
            _player1.SetStartHand(_player1Cards);
            
        }
        public void AddCardPlayer1()
        {
            _damageNoDeckPlayer1--;
            Card index = null;
                for (int i = _player1Cards.Length - 1; i >= 0; i--)
                {
                    if (_player1Cards[i] != null)
                    {
                        index = _player1Cards[i];
                    index._isDraggable = true;
                    if (index.IsFrontCard!=true) index.SwitchVisual();

                    index._cardPlaceType = FieldType.Player1Hand;
                        _player1Cards[i] = null;
                        break;
                    }
                }
            _player1.SetNewCard(index);
        }
        public void AddCardPlayer2()
        {
            _damageNoDeckPlayer2--;
                Card index = null;
                for (int i = _player2Cards.Length - 1; i >= 0; i--)
                {
                    if (_player2Cards[i] != null)
                    {
                        index = _player2Cards[i];
                    index._cardPlaceType = FieldType.Player2Hand;
                    index.transform.eulerAngles += new Vector3(0, 180, 0);
                        _player2Cards[i] = null;
                        break;
                    }                
                }
                _player2.SetNewCard(index);
        }

        private Card[] CreateEnemyDeck(Transform parent)
        {
            var deck = new Card[_cardsInDeck];
            var offset = new Vector3(0f, 0f, 0f);
            for (int i=0; i<_cardsInDeck;i++)
            {
                deck[i] = Instantiate(_cardPrefab, parent);
                if (deck[i].IsFrontCard) deck[i].SwitchVisual();
                deck[i].transform.localPosition = offset;
                offset.y += 0.5f;

                var random = _allCards[UnityEngine.Random.Range(0, _allCards.Length)];
                var picture = new Material(_baseMat)
                {
                    mainTexture = random.Texture
                };
                deck[i].Configuration(picture, random, CardUtility.GetDescriptionById(random.Id));
            }
            return deck;
        }
        private Card[] CreatePlayerDeck(Transform parent)
        {
            Debug.Log(_cardsInDeck);
            var deck = new Card[_cardsInDeck];
            var offset = new Vector3(0f, 0f, 0f);
            for (int i = 0; i < _cardsInDeck; i++)
            {
                deck[i] = Instantiate(PlayerDeckStatic._playerDeck[i], parent);
                deck[i].gameObject.SetActive(true);
                deck[i]._mainCanvas = GameObject.Find("GameCanvas").GetComponent<Canvas>();
                deck[i].State = CardStateType.InDeck;
                deck[i].transform.localScale = new Vector3(70, 1, 100);
                if (deck[i].IsFrontCard) deck[i].SwitchVisual();
                deck[i].transform.localPosition = offset;
                offset.y += 0.5f;
            }
            return deck;
        }
        private IEnumerator TimeTurn()
        {
            _turnTime = 30;
            _turnTimeTxt.text = _turnTime.ToString();
            CheckCardForAvailability();
            while(_turnTime-->0)
            {
                _turnTimeTxt.text = _turnTime.ToString();
                yield return new WaitForSeconds(1);
            }
            EndTurn();
        }
        private void EndTurn()
        {
            StopAllCoroutines();
            _axisCamera.eulerAngles += new Vector3(0, 180, 0);

            foreach(var card in _player1Table._cardsOnTablePlayer1)
            {
                card.DeHighlightCard();
            }
            foreach (var card in _player2Table._cardsOnTablePlayer2)
            {
                card.DeHighlightCard();
            }
            if (_player1turn==true)
            {
                AddCardPlayer2();

                if(_damageNoDeckPlayer2<0)
                {
                    _player2Healths = Mathf.Clamp(_player2Healths +_damageNoDeckPlayer2, 0, int.MaxValue);
                    ShowHp();
                }
                _player2ManaPool=_manaInt;
                ShowMana(_player2ManaPool);
                for (int i = 0; i < _player1.GetLastPosition(); i++)
                {                    
                    _player1._cardsInHand[i]._isDraggable = false;
                    _player1._cardsInHand[i].GetComponent<AttackedCard>().enabled = false;
                    if(_player1._cardsInHand[i].IsFrontCard)
                       _player1._cardsInHand[i].SwitchVisual();
                }

                for (int i = 0; i < _player2.GetLastPosition(); i++)
                {
                    _player2._cardsInHand[i]._isDraggable = true;
                    _player2._cardsInHand[i].GetComponent<AttackedCard>().enabled = false;
                    if (_player2._cardsInHand[i].IsFrontCard==false)
                        _player2._cardsInHand[i].SwitchVisual();
                }
                foreach (var card in _player1Table._cardsOnTablePlayer1)
                {
                    card._isDraggable = false;
                }
                foreach (var card in _player2Table._cardsOnTablePlayer2)
                {
                    card._isDraggable = true;
                }

                foreach (var card in _player2Table._cardsOnTablePlayer2)
                {
                    card.ChangeAttackState(true);
                    card.GetComponent<AttackedCard>().enabled = false;
                    card.HighlightCard();
                }
                foreach (var card in _player1Table._cardsOnTablePlayer1)
                {
                    card.ChangeAttackState(false);
                    card.DeHighlightCard();
                    card.GetComponent<AttackedCard>().enabled = true;
                }
                if (_player1Table._cardsOnTablePlayer1.Exists(x => x.Ability == TypeByDescription.Taunt))
                {
                    for (int i = 0; i < _player1Table._cardsOnTablePlayer1.Count; i++)
                    {
                        if (_player1Table._cardsOnTablePlayer1[i].Ability != TypeByDescription.Taunt)
                            _player1Table._cardsOnTablePlayer1[i].GetComponent<AttackedCard>().enabled = false;
                    }
                    _player1Hero.GetComponent<AttackedHero>().enabled = false;
                }
                _tablePosition = _player1Table.transform.position;
                _player1Table.transform.position = _player2Table.transform.position;
                _player2Table.transform.position = _tablePosition;
                _player1Hero.transform.localEulerAngles += new Vector3(180, 180, 0);
                _player2Hero.transform.localEulerAngles += new Vector3(-180, 180, 0);

            }
            else
            {
                AddCardPlayer1();
                if (_damageNoDeckPlayer1 < 0)
                {
                    _player1Healths = Mathf.Clamp(_player1Healths + _damageNoDeckPlayer1, 0, int.MaxValue);
                    ShowHp();
                }
                if(_manaInt<10)_manaInt++;
                _player1ManaPool = _manaInt;
                ShowMana(_player1ManaPool);
                for (int i = 0; i < _player2.GetLastPosition(); i++)
                {
                    _player2._cardsInHand[i]._isDraggable = false;
                    _player2._cardsInHand[i].GetComponent<AttackedCard>().enabled = false;
                    if (_player2._cardsInHand[i].IsFrontCard)
                        _player2._cardsInHand[i].SwitchVisual();
                }
                for (int i = 0; i < _player1.GetLastPosition(); i++)
                {
                    _player1._cardsInHand[i]._isDraggable = true;
                    _player1._cardsInHand[i].GetComponent<AttackedCard>().enabled = false;
                    if (_player1._cardsInHand[i].IsFrontCard==false)
                        _player1._cardsInHand[i].SwitchVisual();
                }
                foreach (var card in _player1Table._cardsOnTablePlayer1)
                {
                    card._isDraggable = true;
                }
                foreach (var card in _player2Table._cardsOnTablePlayer2)
                {
                    card._isDraggable = false;
                }

                foreach (var card in _player1Table._cardsOnTablePlayer1)
                {
                    card.ChangeAttackState(true);
                    card.GetComponent<AttackedCard>().enabled = false;
                    card.HighlightCard();
                }
                foreach (var card in _player2Table._cardsOnTablePlayer2)
                {
                    card.ChangeAttackState(false);
                    card.GetComponent<AttackedCard>().enabled = true;
                    card.DeHighlightCard();
                }
                if (_player2Table._cardsOnTablePlayer2.Exists(x => x.Ability == TypeByDescription.Taunt))
                {
                    for (int i = 0; i < _player2Table._cardsOnTablePlayer2.Count; i++)
                    {
                        if (_player2Table._cardsOnTablePlayer2[i].Ability != TypeByDescription.Taunt)
                            _player2Table._cardsOnTablePlayer2[i].GetComponent<AttackedCard>().enabled = false;
                    }
                    _player2Hero.GetComponent<AttackedHero>().enabled = false;
                }
                _tablePosition = _player2Table.transform.position;
                _player2Table.transform.position = _player1Table.transform.position;
                _player1Table.transform.position = _tablePosition;
                _player1Hero.transform.localEulerAngles += new Vector3(-180, -180, 0);
                _player2Hero.transform.localEulerAngles += new Vector3(180, -180, 0);
            }
            _player1turn = !_player1turn;
            StartCoroutine(TimeTurn());

        }
        public void ShowMana(int mana)
        {
            _mana.text = $"{mana}/10";
        }
        public void ReduceMana(bool player1mana,int cost)
        {
            if(player1mana)
            {
                _player1ManaPool = Mathf.Clamp(_player1ManaPool - cost, 0, int.MaxValue);
                ShowMana(_player1ManaPool);
            }
            else
            {
                _player2ManaPool = Mathf.Clamp(_player2ManaPool - cost, 0, int.MaxValue);
                ShowMana(_player2ManaPool);
            }
        }
        public void CardsFight(Card Player1,Card Player2)
        {
            Player1.GetDamage(Player2._attackInt);
            Player2.GetDamage(Player1._attackInt);
            if(Player1.IsAlive==false)
            {
                if (Player1._cardPlaceType == FieldType.Player1Table)
                {
                    _player1Table._cardsOnTable--;
                }
                else
                {
                    _player2Table._cardsOnTable--;
                }
                DestroyCards(Player1);
            }
            else
            {
                Player1.RefreshData();
            }

            if (Player2.IsAlive==false)
            {
                DestroyCards(Player2);
                if (Player2._cardPlaceType == FieldType.Player1Table)
                {
                    if (!_player1Table._cardsOnTablePlayer1.Exists(x => x.Ability == TypeByDescription.Taunt))
                    {
                        for (int i = 0; i < _player1Table._cardsOnTablePlayer1.Count; i++)
                        {
                            _player1Table._cardsOnTablePlayer1[i].GetComponent<AttackedCard>().enabled = true;
                        }
                    }
                    _player1Hero.GetComponent<AttackedHero>().enabled = true;
                    _player1Table._cardsOnTable--;
                }
                else
                {
                    DestroyCards(Player2);
                    if (!_player2Table._cardsOnTablePlayer2.Exists(x => x.Ability == TypeByDescription.Taunt))
                    {
                        for (int i = 0; i < _player2Table._cardsOnTablePlayer2.Count; i++)
                        {
                            _player2Table._cardsOnTablePlayer2[i].GetComponent<AttackedCard>().enabled = true;
                        }
                    }
                    _player2Hero.GetComponent<AttackedHero>().enabled = true;
                    _player2Table._cardsOnTable--;
                }
            }
            else
            {
               Player2.RefreshData();
            }
        }
        private void DestroyCards(Card card)
        {
            card.GetComponent<Card>().OnEndDrag(null);

           if(_player2Table._cardsOnTablePlayer2.Exists(x=>x==card))
            {
                _player2Table._cardsOnTablePlayer2.Remove(card);
            }
            if (_player1Table._cardsOnTablePlayer1.Exists(x => x == card))
            {
                _player1Table._cardsOnTablePlayer1.Remove(card);
            }
            Destroy(card.gameObject);
        }
        public void DamageHero(Card card,bool isPlayer1Attacked)
        {
            if(isPlayer1Attacked)
            {
                _player1Healths = Mathf.Clamp(_player1Healths - card._attackInt, 0, int.MaxValue);
            }
            else
            {
                _player2Healths = Mathf.Clamp(_player2Healths - card._attackInt, 0, int.MaxValue);
            }
            ShowHp();
            card.DeHighlightCard();
        }
        private void ShowHp()
        {
            _player1Hero._hp.text = _player1Healths.ToString();
            _player2Hero._hp.text = _player2Healths.ToString();
            CheckForResults();
        }

        private void CheckForResults()
        {
            if(_player1Healths==0||_player2Healths==0)
            {
                _resultsGO.SetActive(true);
                 
                StopAllCoroutines();
                
                if(_player1Healths==0)
                {
                    _resultsTxt.text = "Player2 WIN";
                }
                else
                {
                    _resultsTxt.text = "Player1 WIN";
                }
                StartCoroutine(OpenMenuScene());
            }
        }
        private IEnumerator OpenMenuScene()
        {
            yield return new WaitForSeconds(3f);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = true;
            loadOperation.completed += (operation) => SceneManager.UnloadSceneAsync("Game");


        }
        public void CheckCardForAvailability()
        {
            if (_player1turn)
            {
                for (int i = 0; i < _player1.GetLastPosition(); i++)
                {
                    _player1._cardsInHand[i].CheckForAvailability(_player1ManaPool);
                }
            }
            else
            {
                for (int i = 0; i < _player2.GetLastPosition(); i++)
                {
                    _player2._cardsInHand[i].CheckForAvailability(_player2ManaPool);
                }
            }
        }
    }
    }
