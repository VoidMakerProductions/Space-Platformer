using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;

public class Syncer : MonoBehaviour,RealTimeMultiplayerListener
{
    public Image bar;
    public bool singlplayer;
    public string Gateway="TheVeryStart";
    public GameObject[] registeredPrefabs;
    public GameObject playerPrefab;
    public Dictionary<int, GPGSyncInstance> insts = new Dictionary<int, GPGSyncInstance>();
    public Dictionary<string, int> players = new Dictionary<string, int>();
    List<string> participantIDs = new List<string>();
    int i = 0;
    
    public static Syncer Instance { get; private set; }

    // Use this for initialization
    void Start()
    {
        
        Instance = this;
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        
    }

    public static Vector2 pixelPerfectVector(Vector2 vector, float pixelsPerUnit = 8f) {
        Vector2 vector2 = new Vector2(
            Mathf.RoundToInt(vector.x * pixelsPerUnit),
            Mathf.RoundToInt(vector.y * pixelsPerUnit)
            );
        return vector2 / pixelsPerUnit;
    }
    private void OnLevelFinishedLoading(Scene scene,LoadSceneMode mode)
    {
        i = 0;
        Debug.Log(scene.buildIndex);
        insts = new Dictionary<int, GPGSyncInstance>();
        
        if (scene.buildIndex > 1) {
            //CreatePlayers();
            if (!singlplayer) {

                CreateLocalPlayer();
            }
            else {
                Transform spawn = GameObject.Find(Gateway).transform;
                Vector3 pos = spawn != null ? spawn.position : new Vector3();
                Instantiate(playerPrefab, pos, Quaternion.identity);
            }
        }
       
    }
    public void Remove(int id) {
        insts[id] = null;
    }


    void CreatePlayers() {
        Transform spawn = GameObject.Find(Gateway).transform;
        Vector3 pos = spawn != null ? spawn.position : new Vector3();
        
        
        
        foreach (Participant participant in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants()) {
            Vector3 rand = UnityEngine.Random.insideUnitCircle;
            bool already = false;
            try
            {
                already = insts[players[participant.ParticipantId]] != null;
            }
            catch (KeyNotFoundException ) {
            }
            if (already) {
                continue;
            }
            GameObject go = Instantiate(playerPrefab, pos+rand, Quaternion.identity);
            go.GetComponent<PlayerControl>().playerid = participant.ParticipantId;
            GPGSyncInstance gPG = go.GetComponent<GPGSyncInstance>();
            players[participant.ParticipantId] = Add(gPG);

        }
    }
    void CreateLocalPlayer() {
        Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
        string id = myself.ParticipantId;
        Transform spawn = GameObject.Find(Gateway).transform;
        Vector3 pos = spawn != null ? spawn.position : new Vector3();
        GameObject go = Instantiate(playerPrefab, pos, Quaternion.identity);
        go.GetComponent<PlayerControl>().playerid = id;
        GPGSyncInstance gPG = go.GetComponent<GPGSyncInstance>();
        
        players[id] = Add(gPG);


        byte[] msg;
        byte[] mid = System.Text.Encoding.ASCII.GetBytes(id);
        msg = new byte[mid.Length + 3];
        msg[0] = (byte)MessageType.AddPlayer;
        msg[1] = (byte)players[id];
        msg[2] = (byte)mid.Length;
        i = 3;
        foreach (byte b in mid) {
            msg[i] = b;
            i++;
        }
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, msg);

    }
    public int Add(GPGSyncInstance s,int id=0) {
        if (id == 0)
        {
            int l = i;
            while (insts[l] != null)
            {
                l++;
            }
            insts[l] = s;
            s.id = l;
            i++;
            return l;
        }
        else {
            if (insts[id] != null)
                throw new ArgumentException("Id already taken!");
            insts[id] = s;
            insts[id].id = id;
            return id;

        }
        
    }

    public void OnRoomSetupProgress(float percent)
    {
        /*if(bar==null)
            return;
        float width = Mathf.Floor(percent / 20f) * 8;
        Rect r = bar.rectTransform.rect;
        r.width = width;
        bar.rectTransform.rect.Set(r.x, r.y, r.width, r.height);*/
    }

    public void OnRoomConnected(bool success)
    {
        if (success) {
            SceneManager.LoadScene("SampleScene");
        }
            

    }

    public void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnParticipantLeft(Participant participant)
    {
        Destroy(insts[players[participant.ParticipantId]].gameObject);
        //participantIDs.Remove(participant.ParticipantId);
    }

    public void OnPeersConnected(string[] participantIds)
    {
        participantIDs.AddRange(participantIDs);
        //CreatePlayers();
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        foreach (string pid in participantIds) {
            participantIDs.Remove(pid);
        }
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        MessageType mt = (MessageType)(int)data[0];
        Debug.Log("(Nebullarix), RTM received: "+mt+"len "+data.GetLength(0));
        int objectId;
        float x, y;
        GameObject go;
        try
        {
            switch (mt)
            {
                case MessageType.AddPlayer:
                    objectId = (int)data[1];
                    int len = (int)data[2];
                    string s = System.Text.Encoding.ASCII.GetString(data, 3, len);
                    players[s] = objectId;
                    Debug.Log("(Nebullarix) playeradd: " + s+" objectid: "+objectId);
                    break;
                case MessageType.Destroy:
                    objectId = (int)data[1];
                    Debug.Log("(Nebullarix) destroyId: " + objectId);
                    insts[objectId].Destroy();
                    break;
                case MessageType.Instantiate:
                    objectId = (int)data[1];
                    int objectType = (int)data[10];
                    x = BitConverter.ToSingle(data, 2);
                    y = BitConverter.ToSingle(data, 6);
                    Vector3 pos = new Vector3(x, y);
                    Debug.Log("(Nebullarix) createId: " + objectId + " at " + pos + "type " + objectType);
                    go = Instantiate(registeredPrefabs[objectType], pos, Quaternion.identity);
                    go.GetComponent<GPGSyncInstance>().createMsgSent = true;
                    break;
                case MessageType.VelocitySync:
                    objectId = (int)data[1];
                    x = BitConverter.ToSingle(data, 2);
                    y = BitConverter.ToSingle(data, 6);
                    Vector2 newVel = new Vector2(x, y);
                    Debug.Log("(Nebullarix) SyncVelId: " + objectId + "newVel " + newVel);
                    insts[objectId].SetVelocity(newVel);
                    break;
                case MessageType.PosSync:
                    objectId = (int)data[1];
                    x = BitConverter.ToSingle(data, 2);
                    y = BitConverter.ToSingle(data, 6);
                    Vector3 newPos = new Vector3(x, y);
                    insts[objectId].SetVelocity(newPos);
                    Debug.Log("(Nebullarix) SyncPosId: " + objectId + "newPos " + newPos);
                    break;
            }
        }
        catch (NullReferenceException ex) {
            Debug.Log(ex.Message);
            Debug.Log(ex.Source);
            Debug.Log(ex.StackTrace);
        }
    }

    public enum MessageType
    {
        AddPlayer,
        VelocitySync,
        PosSync,
        Instantiate,
        Destroy
    }
}


