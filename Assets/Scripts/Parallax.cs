using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {


    public Material mat;
    public float coeff;
    public float halfXT;
    public float halfYT;
    PlayerControl PC;
	// Use this for initialization
	void Start () {
        
	}
    public void SetTarget(PlayerControl playerControl) {
        PC = playerControl;
    }
	// Update is called once per frame
	void Update () {
        float x=0;
        float y=0;
        if (PC != null) {
            x = (PC.assumedPosition.x * coeff) % halfXT;
            y = (PC.assumedPosition.y * coeff) % halfYT;
        }
        
        /*if (x > halfXT)
        {
            float excess = x - halfXT;
            x = (x * -1) + excess;
        }
        if (x < -halfXT)
        {
            float excess = x + halfXT;
            x = (x * -1) + excess;
        }
        if (y > halfYT)
        {
            float excess = y - halfYT;
            y = (y * -1) + excess;
        }
        if (y < -halfYT)
        {
            float excess = y + halfYT;
            y = (y * -1) + excess;
        }*/
        mat.SetTextureOffset("_MainTex", new Vector2(x, y));
    }
}
