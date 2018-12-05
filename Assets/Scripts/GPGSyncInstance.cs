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
    public SVariations SyncVariation;
    [Range(0f,10f)]public float PositionSendRate;
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
        
        Debug.Log("Somehow getting past singlplayer");
        if (!isPlayer)
            Syncer.Instance.Add(this);
        if (createMsgSent) return;
        byte[] message = new byte[11];
        message[0] = (byte)Syncer.MessageType.Instantiate;
        message[1] = (byte)id;
        int i = 2;
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
        message[10] = (byte)type;
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
    }

    // Update is called once per frame
    void Update()
    {
        if (Syncer.Instance.singlplayer) {
            enabled = false;
            return;
        }
        if (self != null&&(SyncVariation==SVariations.Velocity||SyncVariation==SVariations.Both)) {
            if (self.velocity != prevVelocity)
            {
                prevVelocity = self.velocity;
                byte[] message = new byte[10];
                message[0] = (byte)Syncer.MessageType.VelocitySync;
                message[1] = (byte)id;
                int i = 2;
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
                nextSend = Time.time + (1f / PositionSendRate);
                byte[] message = new byte[10];
                message[0] = (byte)Syncer.MessageType.PosSync;
                message[1] = (byte)id;
                int i = 2;
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
                Debug.Log("(Nebullarix) RTM msg sent: PosSync");
            }
        }
        
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
        if (sendDestroyMsg) {
            byte[] message = new byte[2];
            message[0] = (byte)Syncer.MessageType.Destroy;
            message[1] = (byte)id;
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
        }
        
        Syncer.Instance.Remove(id);
    }


}
