using UnityEngine;
using System.Collections;

public class EnterTalk : MonoBehaviour
{
    public DialObject[] dialObjects;
    public DialControl control;
    int current = 0;
    private void Start()
    {
        control = Camera.main.GetComponent<DialControl>();
    }


    public void Next() {
        
        DialObject dial = dialObjects[current];
        current = dial.next;
        string text = current >= 0 ? dial.translatedText.GetCustomizedString() : "";
        control.SetText(text);
    }


    public void Option1()
    {
        DialObject dial = dialObjects[current];
        current = dial.option1;
        string text = current >= 0 ? dial.translatedText.GetCustomizedString() : "";
        control.SetText(text);
    }
    public void Option2()
    {
        DialObject dial = dialObjects[current];
        current = dial.option2;
        string text = current >= 0 ? dial.translatedText.GetCustomizedString() : "";
        control.SetText(text);
    }
    public void Option3()
    {
        DialObject dial = dialObjects[current];
        current = dial.option3;
        string text = current >= 0 ? dial.translatedText.GetCustomizedString() : "";
        control.SetText(text);
    }
}
