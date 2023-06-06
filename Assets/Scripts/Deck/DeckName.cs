using UnityEngine;
using UnityEngine.UI;

public class DeckName : MonoBehaviour
{
    [SerializeField]
    private Text _deckName = null;
    [SerializeField]
    private InputField _inputDeckName;
    private string text;

    public void SaveInputText()
    {
        text = _inputDeckName.text;
    }

    public void ShowName()
    {
        _deckName.text = text;
        _inputDeckName = null;
    }
}
