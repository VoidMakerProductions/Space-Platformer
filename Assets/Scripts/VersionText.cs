using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionText : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UnityEngine.UI.Text>().text = Application.version;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
