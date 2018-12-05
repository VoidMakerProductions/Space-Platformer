using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Drawer", menuName = "ScriptableObjects/PixelToTileConvertor", order = 1)]
public class PixelToTiles : ScriptableObject
{
    
    ColorPtt transfers = new ColorPtt();
    public PTT[] Transfers;
    public Texture2D blueprint;
    public Vector3Int offset;
    public Tilemap background;
    public Tilemap foreground;
    public static PixelToTiles current;
    private void Awake()
    {
        if (Transfers == null) {
            return;
        }
        foreach (PTT p in Transfers) {
            transfers[p.color] = p;
        }
        
    }

    
    
    public static void DoIt()
    {
        if (current == null) {
            EditorUtility.DisplayDialog("PixelToTiles", "Set The Drawer First!", "OK", "");
            return;
        }
        
        for (int i = 0; i < current.blueprint.width; i++)
        {
            for (int j = 0; j < current.blueprint.height; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                pos += current.offset;
                Color key = current.blueprint.GetPixel(i, j);
                //Debug.Log(key);
                
                current.background.SetTile(pos, current.transfers[key].back);
                current.foreground.SetTile(pos, current.transfers[key].fore);
            }
        }
    }

    public void GetAllColorsFromBlueprint() {
        if (blueprint == null) {
            return;
        }
        HashSet<Color> colors = new HashSet<Color>();
        foreach (Color c in blueprint.GetPixels()) {
            colors.Add(c);
        }
        Transfers = new PTT[colors.Count];
        int i = 0;
        foreach (Color c in colors) {
            Transfers[i] = new PTT(c);
            i++;
        }
    }
    public void SetThisAsCurrent() {
        if (Transfers == null)
        {
            return;
        }
        foreach (PTT p in Transfers)
        {
            transfers[p.color] = p;
        }
        current = this;
    }

    
    [System.Serializable]
    public class ColorPtt : Dictionary<Color, PTT> { }

    [System.Serializable]
    public class PTT {
        public PTT(Color color) {
            this.color = color;
        }
        public Color color;
        public TileBase back;
        public TileBase fore;
    }
}