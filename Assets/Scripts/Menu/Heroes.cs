using Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Heroes : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Transform _shadow = null;

        public SideType _side;
        public TextMeshPro _hp;
        private MenuManager _menuManager;
        private GameManager _gameManager;

    private void Awake()
    {
        if(PlayerDeckStatic.SceneNumber==1)
        {
            _gameManager = GameObject.Find("Board").GetComponent<GameManager>();
        }
        else
        {
            _menuManager = GameObject.Find("MenuCanvas").GetComponent<MenuManager>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
        {
            Heroes hero = eventData.pointerPress.GetComponent<Heroes>();
        if (PlayerDeckStatic.SceneNumber==0)
        {
            _menuManager._hero = hero;
            _menuManager.DeckCreating(hero);
            _shadow.transform.position = transform.position;
            _shadow.SetParent(transform.parent);
        }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }
    }

