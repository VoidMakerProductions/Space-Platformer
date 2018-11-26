using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class AutoReadyPlayer : NetworkLobbyPlayer
{

    // Use this for initialization
    void Start()
    {
        readyToBegin = true;
        SendReadyToBeginMessage();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
