using UnityEngine;
using UnityEditor;

[System.Serializable]
[CreateAssetMenu(fileName = "DialogueObject", menuName = "Game texts/DialogueObject", order = 1)]
public class DialObject : ScriptableObject
{
    public TranslatedText translatedText;
    ContinueVariant continueVariant;
    public int next=-1;
    public int option1=-1;
    public TranslatedText opt1text;
    public int option2=-1;
    public TranslatedText opt2text;
    public int option3=-1;
    public TranslatedText opt3text;

    enum ContinueVariant {
        Button,
        SingleOption,
        TwoOptions,
        THreeOptions
    }
}