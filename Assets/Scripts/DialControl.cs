using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialControl : MonoBehaviour
{
    public CanvasGroup window;
    public Text text;
    // Use this for initialization
    void Start()
    {
        window.alpha = 0f;
        window.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string text) {
        bool active = text != "";
        window.alpha = active ? 1f : 0f;
        window.interactable = active;
        this.text.text = text;
    }
}
