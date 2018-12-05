using UnityEngine;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;

public class Starter : MonoBehaviour
{
    private void Start()
    {
        manager = FindObjectOfType<MyNetworkManager>();
    }
    public MyNetworkManager manager;
    bool started = false;
    public void CreateGame() {
        /* if (!started) {
             manager.StartHost();
             MyNetworkManager.camefrom = "MainMenu";
             started = true;
         }*/
        Syncer.Instance.singlplayer = false;
        PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(0, 10, 0, Syncer.Instance);
        
    }

    public void TestByteConversion() {
        byte[] test = new byte[4];
        int i = 0;
        foreach (byte b in System.BitConverter.GetBytes(10f))
        {
            test[i] = b;
            i++;
        }
        Debug.Log(System.BitConverter.ToSingle(test,0));
    }
}
