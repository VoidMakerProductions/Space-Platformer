using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "DialogueObject", menuName = "Game texts/Translated Text", order = 1)]
public class TranslatedText : ScriptableObject
{
    public TextItem[] variants;
    Dictionary<string, string> txt;

    private void OnEnable()
    {
        foreach (TextItem item in variants) {
            txt[item.translation_tag] = item.text;
        }
    }

    public string GetCustomizedString() {
        string tag = PlayerPrefs.GetString("language") + "-" + PlayerPrefs.GetString("gender");
        return txt[tag];
    }
}