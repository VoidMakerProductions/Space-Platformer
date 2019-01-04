using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using GooglePlayGames;

public class InteractTalk : Interactable
{

    
    public DialControl control;
    public TextAsset dialogue;
    XDocument doc;
    PlayerControl player;
    IEnumerable<XElement> items;
    string translationTag;
    int current = 0;
    private void Start()
    {
        control = Camera.main.GetComponent<DialControl>();
        doc = XDocument.Parse(dialogue.text);
        XElement d = doc.Element("dialogue");
        items = d.Elements("item");
        translationTag = PlayerPrefs.GetString("language") + "-" + PlayerPrefs.GetString("gender");
    }

    
    public void Next()
    {

        XElement item;
        if (current >= 0)
        {
            item = Enumerable.ElementAt(items, current);
            current = System.Convert.ToInt32(item.Attribute("next").Value);
        }
        string text = "";
        if (current >= 0) {
            item = Enumerable.ElementAt(items, current);
            text = item.Element(translationTag).Value;
        }
        
        
        control.SetText(text);
        if (text == "") {
            player.currentTalk = null;
            Time.timeScale = 1f;
        }
        if (!Syncer.Instance.singlplayer)
        {
            byte[] txt = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] message = new byte[txt.Length + 5];
            message[0] = (byte)Syncer.MessageType.Talking;
            
            int i = 1;
            foreach (byte b in System.BitConverter.GetBytes(txt.Length)) 
            {
                message[i] = b;
                i++;
            }
            foreach (byte b in txt)
            {
                message[i] = b;
                i++;
            }
            foreach (byte b in txt)
            {
                message[i] = b;
                i++;
            }
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
        }
    }


    

    public override void Interact(PlayerControl actor)
    {
        translationTag = PlayerPrefs.GetString("language") + '-' + actor.gender;
        XElement item;
        string text;
        if (current >= 0) {
            item = Enumerable.ElementAt(items, current);
            text = item.Element(translationTag).Value;
        }
        else {
            text = "";
        }
        control.SetText(text);
        
        Time.timeScale = 0f;
        actor.currentTalk = this;
        actor.altCurrentTalk = null;
        player = actor;
        if (!Syncer.Instance.singlplayer) {
            byte[] txt = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] message = new byte[txt.Length + 2];
            message[0] = (byte)Syncer.MessageType.Talking;
            message[1] = (byte)txt.Length;
            int i = 2;
            foreach (byte b in txt) {
                message[i] = b;
                i++;
            }
            PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, message);
        }
        
    }
}
