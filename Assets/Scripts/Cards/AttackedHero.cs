using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AttackedHero : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        Player1Hero,
        Player2Hero
    }
    public HeroType Type;
    private GameManager _manager;

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
        if(card&&card.CanAttack)
        {
            if(card._cardPlaceType==FieldType.Player1Table&&Type==HeroType.Player2Hero)
            {
                _manager.DamageHero(card, false);
                card.CanAttack = false;
            }
            else if(card._cardPlaceType == FieldType.Player2Table && Type == HeroType.Player1Hero)
            {
                _manager.DamageHero(card, true);
                card.CanAttack = false;
            }
        }
    }
}
