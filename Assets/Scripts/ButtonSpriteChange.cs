using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class ButtonSpriteChange : MonoBehaviour {
    public Image Target;
    public Sprite sp1;
    public Sprite sp2;
    public ButtonHandler bh;
    public float DoubleClickInterval = 0.5f;
    float prevClick = -100f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Toggle() {
        if (Time.time - prevClick <= DoubleClickInterval)
        {
            bool b = Target.sprite == sp1;
            Target.sprite = b ? sp2 : sp1;
            if (b)
            {
                bh.SetDownState();
            }
            else
            {
                bh.SetUpState();
            }
        }
        else {
            prevClick = Time.time;
        }
        
    }
}
