using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour
{

    public MyNetworkManager manager;
    bool started = false;
    public void CreateGame() {
        if (!started) {
            manager.StartHost();
            MyNetworkManager.camefrom = "MainMenu";
            started = true;
        }
        
    }
}
