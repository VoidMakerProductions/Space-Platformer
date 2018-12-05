using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PixelToTiles))]
public class PixelToTilesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PixelToTiles my_target = (PixelToTiles)target;
        if (GUILayout.Button("Get All Colors from blueprint"))
        {
            my_target.GetAllColorsFromBlueprint();
        }
        if (GUILayout.Button("Draw on Map")){
            my_target.SetThisAsCurrent();
            PixelToTiles.DoIt();
        }
    }

   
}