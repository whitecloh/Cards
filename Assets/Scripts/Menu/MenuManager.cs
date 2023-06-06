using Cards.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cards {
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Button[] _allDecks = null;
        [SerializeField]
        private Button _createDeck = null;
        [SerializeField]
        private Button[] _heroPeak = null;

        [SerializeField]
        private Button[] _startMenu = null;
        public Button _gameStart;
        [SerializeField]
        private Button _deckReady = null;

        [SerializeField]
        private GameObject _startMenuGO = null;
        [SerializeField]
        private GameObject _allDecksGO = null;
        [SerializeField]
        private GameObject _createDeckGO = null;
        [SerializeField]
        private GameObject _heroPeakGo = null;


        private Material _baseMat;
        private CardPropertiesData[] _allCards;
        [SerializeField]
        private CardPackConfiguration[] _packs = null;

        [SerializeField]
        private Card _cardPrefab = null;
        [Space]
        public Transform[] _deckPositions;

        private Card[] _playerCards;

        public string _deckName;
 
        public InputField _inputDeckName;

        public Button[] _allReadyDecks;


        private CreateDeck _createdDeck;
        public List<Card> _playerDeck;
        private int _deckIndex;
        [SerializeField]
        private Transform _pickedDeck = null;       
        public Transform _pickedHero;
        public Heroes _hero;
        [SerializeField]
        private Transform _heroTransform;


        public void Awake()
        {
            PlayerDeckStatic.SceneNumber = 0;
            _createdDeck = GameObject.Find("CreateDeckMenu").GetComponent<CreateDeck>();
            _gameStart.interactable = false;
            IEnumerable<CardPropertiesData> cards = new List<CardPropertiesData>();

            foreach (var pack in _packs)
            {
                cards = pack.UnionProperties(cards);
            }
            _allCards = cards.ToArray();

            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"))
            {
                renderQueue = 3006
            };
        }
        private void Update()
        {
            for(int i =0;i<_allReadyDecks.Length;i++)
            {
                if (_allReadyDecks[i].transform.GetChild(0).GetComponent<Text>().text == "Empty")
                { 
                    _allReadyDecks[i].interactable = false;
                }
                else 
                { 
                    _allReadyDecks[i].interactable = true;
                }
            }
        }
        private void Start()
        {
            _startMenuGO.SetActive(true);
            _allDecksGO.SetActive(false);
            _createDeckGO.SetActive(false);
            _heroPeakGo.SetActive(false);

            for (int i = 0; i < _allDecks.Length; i++)
            {
                _allDecks[i].onClick.AddListener(OpenAllDecks);
            }
            for (int i = 0; i < _heroPeak.Length; i++)
            {
                _heroPeak[i].onClick.AddListener(OpenHeroPeak);
            }
            for (int i = 0; i < _startMenu.Length; i++)
            {
                _startMenu[i].onClick.AddListener(OpenStartMenu);
            }

            _createDeck.onClick.AddListener(OpenDeckCreate);
            _createDeck.interactable = false;

            _gameStart.onClick.AddListener(GameStart);
            
        }

        public void OpenAllDecks()
        {
            _createdDeck._cardsOnMenuTable = 0;
            _startMenuGO.SetActive(false);
            _allDecksGO.SetActive(true);
            _createDeckGO.SetActive(false);
            _heroPeakGo.SetActive(false);
        }
        private void OpenHeroPeak()
        {
            _startMenuGO.SetActive(false);
            _allDecksGO.SetActive(false);
            _createDeckGO.SetActive(false);
            _heroPeakGo.SetActive(true);

        }
        public void OpenDeckCreate()
        {
            _startMenuGO.SetActive(false);
            _allDecksGO.SetActive(false);
            _createDeckGO.SetActive(true);
            _heroPeakGo.SetActive(false);
            _playerCards = CreateDeck(_deckPositions);
            _deckReady.interactable = false;
            Text _sideName = GameObject.Find("Side").GetComponent<Text>();
            _sideName.text = _deckName;
        }

        public void DeckCreating(Heroes hero)
        {
            IEnumerable<CardPropertiesData> cards = new List<CardPropertiesData>();
            foreach (var pack in _packs)
            {
                        if(pack._sideType==hero._side||pack._sideType==SideType.Common)
                {
                    cards = pack.UnionProperties(cards);
                }
        }
        _allCards = cards.ToArray();
            Debug.Log(_allCards.Length);
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"))
            {
                renderQueue = 3006
            };
            _deckName = hero.name;
            _createDeck.interactable = true;

        }
        private void OpenStartMenu()
        {
            _startMenuGO.SetActive(true);
            _allDecksGO.SetActive(false);
            _createDeckGO.SetActive(false);
            _heroPeakGo.SetActive(false);
            _inputDeckName.text = default;
        }
        private void GameStart()
        {
            var hero = Instantiate(_hero);
            hero.transform.SetParent(_pickedHero);

            _playerDeck = _createdDeck._decks[_deckIndex];
            for (int i =0;i<_playerDeck.Count;i++)
            {
                _playerDeck[i].transform.SetParent(_pickedDeck);
            }
            PlayerDeckStatic._playerDeck = _playerDeck.ToArray();
            PlayerDeckStatic._player1Hero = hero;

                DontDestroyOnLoad(_pickedDeck);
                DontDestroyOnLoad(_pickedHero);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = true;
            loadOperation.completed += (operation) => SceneManager.UnloadSceneAsync("Menu");
           // SceneManager.LoadScene("Game");
        }
        public void PickDeck(int i)
        {
            _deckIndex = i;
            Debug.Log(_deckIndex);
        }
    private Card[] CreateDeck(Transform[] parent)
        {
            var deck = new Card[_deckPositions.Length];

            for (int i = 0; i < _deckPositions.Length; i++)
            {
                deck[i] = Instantiate(_cardPrefab, parent[i]);
                deck[i].State = CardStateType.InHand;
                var random = _allCards[i];
                var picture = new Material(_baseMat)
                {
                    mainTexture = random.Texture
                };
                deck[i].Configuration(picture, random, CardUtility.GetDescriptionById(random.Id));
            }
            return deck;
        }
}
}
