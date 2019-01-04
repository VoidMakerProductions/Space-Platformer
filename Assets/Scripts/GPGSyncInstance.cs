using UnityEngine;
using System;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;


public class GPGSyncInstance : MonoBehaviour
{
    public int id;
    public int type;
    public Rigidbody2D self;
    Vector2 prevVelocity = new Vector2(0,0);
    bool sendDestroyMsg = true;
    public bool isPlayer;
    public bool createMsgSent = false;
    public HealthKeeper healthKeeper;
    int prevHP;
    PlayerControl pc;
    Participant me;
    public SVariations SyncVariation;
    [Range(0f,20f)]public float PositionSendRate;
   
    float nextSend;
    public enum SVariations {
        Position,
        Velocity,
        Both
    }
    // Use this for initialization
    void Start()
    {
        if (Syncer.Instance.singlplayer) {
            enabled = false;
            return;
        }

        if (healthKeeper) {
            prevHP = healthKeeper.maxHP;
            healthKeeper.SetHP(prevHP);
        }
            
        if (!isPlayer)
            Syncer.Instance.Add(this);
        
        if (createMsgSent) return;
        gameObject.name = GetInstanceID().ToString();
        byte[] message = new byte[17];
        message[0] = (byte)Syncer.MessageType.Instantiate;
        int i = 1;
        foreach (byte b in BitConverter.GetBytes(GetInstanceID()))
        {
            message[i] = b;
            i++;//1-4
        }
        foreach (byte b in BitConverter.GetBytes(transform.position.x))
        {
            message[i] = b;
            i++;//5-8
        }
        foreach (byte b in BitConverter.GetBytes(transform.position.y))
        {
            message[i] = b;
            i++;//9-12
        }
        foreach (byte b in BitConverter.GetBytes(type))
        {
            message[i] = b;
            i++;//13-16
        }
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
        pc = GetComponent<PlayerControl>();
        me = PlayGamesPlatform.Instance.RealTime.GetSelf();
    }

    // Update is called once per frame
    void Update()
    {
        if (Syncer.Instance.singlplayer) {
            enabled = false;
            return;
        }
        if (healthKeeper) {
            if (prevHP != healthKeeper.GetHP()) {
                prevHP = healthKeeper.GetHP();
                bool cancel = false;
                if (pc)
                    cancel = pc.playerid != me.ParticipantId;
                if (!cancel) {
                    byte[] message = new byte[10];
                    message[0] = (byte)Syncer.MessageType.HPUpdate;
                    int i = 1;
                    foreach (byte b in BitConverter.GetBytes(Convert.ToInt32(name)))
                    {
                        message[i] = b;
                        i++;
                    }
                    foreach (byte b in BitConverter.GetBytes(healthKeeper.GetHP()))
                    {
                        message[i] = b;
                        i++;
                    }
                }
            }
        }
        if (self != null&&(SyncVariation==SVariations.Velocity||SyncVariation==SVariations.Both)) {
            if (self.velocity != prevVelocity)
            {
                
                prevVelocity = self.velocity;
                byte[] message = new byte[14];
                message[0] = (byte)Syncer.MessageType.VelocitySync;
                int i = 1;
                foreach (byte b in BitConverter.GetBytes(Convert.ToInt32(name)))
                {
                    message[i] = b;
                    i++;
                }
                foreach (byte b in BitConverter.GetBytes(self.velocity.x))
                {
                    message[i] = b;
                    i++;
                }
                foreach (byte b in BitConverter.GetBytes(self.velocity.y))
                {
                    message[i] = b;
                    i++;
                }
                PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, message);
                Debug.Log("(Nebullarix) RTM msg sent: VelocitySync");
            }
        }
        if(SyncVariation!=SVariations.Velocity&&PositionSendRate>0){
            if (Time.time > nextSend) {
                bool cancel = false;
                if (pc)
                    cancel = pc.playerid != me.ParticipantId;
                if (!cancel) {
                    nextSend = Time.time + (1f / PositionSendRate);
                    byte[] message = new byte[14];
                    message[0] = (byte)Syncer.MessageType.PosSync;
                    int i = 1;
                    foreach (byte b in BitConverter.GetBytes(Convert.ToInt32(name)))
                    {
                        message[i] = b;
                        i++;
                    }
                    foreach (byte b in BitConverter.GetBytes(transform.position.x))
                    {
                        message[i] = b;
                        i++;
                    }
                    foreach (byte b in BitConverter.GetBytes(transform.position.y))
                    {
                        message[i] = b;
                        i++;
                    }

                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, message);
                    message = new byte[10];
                    message[0] = (byte)Syncer.MessageType.RotSync;
                    i = 1;
                    foreach (byte b in BitConverter.GetBytes(GetInstanceID()))
                    {
                        message[i] = b;
                        i++;
                    }
                    foreach (byte b in BitConverter.GetBytes(transform.rotation.eulerAngles.z))
                    {
                        message[i] = b;
                        i++;
                    }
                    PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, message);
                    Debug.Log("(Nebullarix) RTM msg sent: RotSync");
                }
                
            }
        }
        
    }

    public void SetHP(int HP) {
        prevHP = HP;
        if (healthKeeper)
            healthKeeper.SetHP(HP);
    }
    public void SetRot(float angle) {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SendPlayerSettings(int spriteIndex,char gender) {
        byte[] msg = new byte[16];
        msg[0] = (byte)Syncer.MessageType.PlayerSettings;
        int i = 1;
        foreach (byte b in BitConverter.GetBytes(Convert.ToInt32(name)))
        {
            msg[i] = b;
            i++;
        }
        foreach (byte b in BitConverter.GetBytes(spriteIndex))
        {
            msg[i] = b;
            i++;
        }
        foreach (byte b in BitConverter.GetBytes(gender))
        {
            msg[i] = b;
            i++;
        }
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, msg);
    }
    public void Destroy(bool sendMessage = false) {
        sendDestroyMsg = sendMessage;
        Destroy(gameObject);
        
    }
    public void SetPosition(Vector3 pos) {
        transform.position = pos;
    }
    public void SetVelocity(Vector2 velocity) {
        prevVelocity = velocity;
        self.velocity = velocity;
    }
    private void OnDestroy()
    {
        if (Syncer.Instance.singlplayer)
            return;
        if (sendDestroyMsg) {
            byte[] message = new byte[8];
            message[0] = (byte)Syncer.MessageType.Destroy;
            int i = 1;
            foreach (byte b in BitConverter.GetBytes(Convert.ToInt32(name)))
            {
                message[i] = b;
                i++;
            }
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
        }
        
        Syncer.Instance.Remove(id);
    }


}
