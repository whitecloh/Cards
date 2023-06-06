using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckPeakAndPlay : MonoBehaviour, ISelectHandler
{
    private int _deckNumberInDecksList;
    [SerializeField]
    private MenuManager _manager = null;

    public void OnSelect(BaseEventData eventData)
    {
        _deckNumberInDecksList = eventData.selectedObject.transform.GetSiblingIndex();
        _manager.PickDeck(_deckNumberInDecksList);

        _manager._gameStart.interactable = true;
    }
}
