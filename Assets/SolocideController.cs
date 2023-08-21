using UnityEngine;

public class SolocideController : MonoBehaviour
{
    private Solocide _solocide;

    private void Start()
    {
        _solocide = new Solocide();
        Debug.Log(_solocide);
    }
}
