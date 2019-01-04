using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using GooglePlayGames.BasicApi;

public class Syncer : MonoBehaviour,RealTimeMultiplayerListener
{
    public AudioClip[] soundtracks;
    public int[] stIndexes;
    int stIndex;
    public Image bar;
    public bool singlplayer;
    public string Gateway="TheVeryStart";
    public GameObject[] registeredPrefabs;
    public GameObject playerPrefab;
    public Dictionary<int, GPGSyncInstance> insts = new Dictionary<int, GPGSyncInstance>();
    public Dictionary<string, int> players = new Dictionary<string, int>();
    public WeaponType[] weaponTypes;
    List<string> participantIDs = new List<string>();
    int i = 0;
    List<GameObject> pcs;
    public static Syncer Instance { get; private set; }

    // Use this for initialization
    void Start()
    {
        
        Instance = this;
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        pcs = new List<GameObject>();
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.Activate();
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
        if (stIndexes[scene.buildIndex] != stIndex) {
            stIndex = stIndexes[scene.buildIndex];
            BackgroundMusic.clip = soundtracks[stIndex];
            BackgroundMusic.Play();
        }
        
        insts = new Dictionary<int, GPGSyncInstance>();
        
        if (scene.buildIndex > 1) {
            //CreatePlayers();
            if (!singlplayer) {
                pcs.Clear();
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


    public Transform GetRandomTrackTarget() {
        List<GameObject> alive = new List<GameObject>();
        foreach (GameObject pc in pcs) {
            if (pc.GetComponent<HealthKeeper>().GetHP() > 0) {
                alive.Add(pc);
            }
        }
        if (alive.Count < 1) {
            return null;
        }
        return alive.ToArray()[UnityEngine.Random.Range(0, alive.Count)].transform;

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
        pcs.Add(go);
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
    public bool CheckIfAllPlayersDead() {
        foreach (GameObject p in pcs) {
            if (p.GetComponent<HealthKeeper>().GetHP() > 0)
                return false;
        }

        return true;
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
        int objectId,sprIndex,len;
        char g;
        float x, y;
        string s;
        GameObject go;
        try
        {
            switch (mt)
            {
                case MessageType.PlayerSettings:
                    objectId = BitConverter.ToInt32(data,1);
                    sprIndex = BitConverter.ToInt32(data, 5);
                    g = BitConverter.ToChar(data,9);
                    go = GameObject.Find(objectId.ToString());
                    go.GetComponent<PlayerControl>().gender = g;
                    go.GetComponent<PlayerControl>().SetSprite(sprIndex);
                    pcs.Add(go);
                    break;
                case MessageType.AddPlayer:
                    objectId = BitConverter.ToInt32(data, 1);
                    len = BitConverter.ToInt32(data, 5);
                    s = System.Text.Encoding.ASCII.GetString(data, 9, len);
                    players[s] = objectId;
                    Debug.Log("(Nebullarix) playeradd: " + s + " objectid: " + objectId);
                    break;
                case MessageType.Talking:
                    
                    len = BitConverter.ToInt32(data, 1);
                    s = System.Text.Encoding.UTF8.GetString(data, 5, len);
                    Time.timeScale = s == "" ? 1f : 0f;
                    Camera.main.GetComponent<DialControl>().SetText(s);
                    Debug.Log("(Nebullarix) TalkingText: " + s );
                    break;
                case MessageType.Destroy:
                    objectId = BitConverter.ToInt32(data, 1);
                    Debug.Log("(Nebullarix) destroyId: " + objectId);
                    go = GameObject.Find(objectId.ToString());
                    go.GetComponent<GPGSyncInstance>().Destroy();
                    break;
                case MessageType.Instantiate:
                    objectId = BitConverter.ToInt32(data, 1);
                    int objectType = BitConverter.ToInt32(data, 13);
                    x = BitConverter.ToSingle(data, 2);
                    y = BitConverter.ToSingle(data, 6);
                    Vector3 pos = new Vector3(x, y);
                    Debug.Log("(Nebullarix) createId: " + objectId + " at " + pos + "type " + objectType);
                    go = Instantiate(registeredPrefabs[objectType], pos, Quaternion.identity);
                    go.GetComponent<GPGSyncInstance>().createMsgSent = true;
                    go.name = objectId.ToString();
                    break;
                case MessageType.VelocitySync:
                    objectId = BitConverter.ToInt32(data, 1);
                    x = BitConverter.ToSingle(data, 5);
                    y = BitConverter.ToSingle(data, 9);
                    Vector2 newVel = new Vector2(x, y);
                    Debug.Log("(Nebullarix) SyncVelId: " + objectId + " newVel " + newVel);
                    go = GameObject.Find(objectId.ToString());
                    go.GetComponent<GPGSyncInstance>().SetVelocity(newVel);
                    break;
                case MessageType.PosSync:
                    objectId = BitConverter.ToInt32(data, 1);
                    x = BitConverter.ToSingle(data, 5);
                    y = BitConverter.ToSingle(data, 9);
                    Vector3 newPos = new Vector3(x, y);
                    go = GameObject.Find(objectId.ToString());
                    go.GetComponent<GPGSyncInstance>().SetPosition(newPos);
                    Debug.Log("(Nebullarix) SyncPosId: " + objectId + " newPos " + newPos);
                    break;
                case MessageType.RotSync:
                    objectId = BitConverter.ToInt32(data, 1);
                    x = BitConverter.ToSingle(data, 5);                  
                    go = GameObject.Find(objectId.ToString());
                    go.GetComponent<GPGSyncInstance>().SetRot(x);
                    Debug.Log("(Nebullarix) SyncRotId: " + objectId + " angle:  " + x);
                    break;
                case MessageType.HPUpdate:
                    objectId = BitConverter.ToInt32(data, 1);
                    go = GameObject.Find(objectId.ToString());
                    sprIndex =  BitConverter.ToInt32(data, 5);
                    go.GetComponent<GPGSyncInstance>().SetHP(sprIndex);
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
        RotSync,
        Instantiate,
        Destroy,
        ChildSyncPos,
        ChildSyncRot,
        PlayerSettings,
        HPUpdate,
        Talking
    }
}


