using UnityEngine;
using UnityEngine.UI;

public class SolocideController : MonoBehaviour
{
    private Solocide _solocide;
    private Text _text;
    private readonly bool[] _selected = new bool[8];

    private void Start()
    {
        _solocide = new Solocide();
        _text = GetComponent<Text>();
        Debug.Log(_solocide);
    }

    private void Update()
    {
        _text.text = _solocide.CurrentState();
        
    }
}
