using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveData
{
    public float remainingTime;
    public string scene;
    public string camefrom;
    public int hp;
    public int money;

    public override string ToString()
    {
        string s = "";
        s += "remainingTime:" + remainingTime.ToString();
        s += ";scene:" + scene;
        s += ";hp:" + hp.ToString();
        s += ";money:" + money.ToString();
        return s;
    }

    public override bool Equals(object obj)
    {
        var data = obj as SaveData;
        return data != null &&
               remainingTime == data.remainingTime &&
               scene == data.scene &&
               camefrom == data.camefrom &&
               hp == data.hp &&
               money == data.money;
    }

    public override int GetHashCode()
    {
        var hashCode = -751328536;
        hashCode = hashCode * -1521134295 + remainingTime.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(scene);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(camefrom);
        hashCode = hashCode * -1521134295 + hp.GetHashCode();
        hashCode = hashCode * -1521134295 + money.GetHashCode();
        return hashCode;
    }
}


