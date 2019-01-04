using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SceneChange : Interactable
{
    public string Destination;
    public string Source;
    public override void Interact(PlayerControl actor)
    {
        MyNetworkManager.camefrom = Source;
        if (players_inside== NetworkManager.singleton.numPlayers) {
            NetworkManager.singleton.ServerChangeScene(Destination);
        }
    }

    
}
