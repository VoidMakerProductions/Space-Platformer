using UnityEngine;
using UnityEditor;


[System.Serializable]
[CreateAssetMenu(fileName = "DialogueObject", menuName = "Game texts/Single LG line", order = 1)]
public class TextItem : ScriptableObject
{
    public string translation_tag;
    public string text;
}