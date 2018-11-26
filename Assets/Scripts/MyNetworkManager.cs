using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MyNetworkManager : NetworkLobbyManager
{
    public static string camefrom;



    public override void OnServerSceneChanged(string sceneName)
    {
        if (camefrom == "MainMenu") return;
        foreach (var lobbys in lobbySlots)
        {
            if (lobbys == null) continue;
            var controllerId = lobbys.GetComponent<NetworkIdentity>().playerControllerId;
            Transform startPos = GameObject.Find(camefrom).transform;
            GameObject gamePlayer;
            if (startPos != null)
            {
                gamePlayer = (GameObject)Instantiate(gamePlayerPrefab, startPos.position, startPos.rotation);
            }
            else
            {
                gamePlayer = (GameObject)Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
            }
            OnLobbyServerSceneLoadedForPlayer(lobbys.gameObject, gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbys.GetComponent<NetworkIdentity>().connectionToClient, gamePlayer, controllerId);
        }

    }
}
