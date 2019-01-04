using UnityEngine;
using System.Collections;

public class HealthKeeper : MonoBehaviour
{
    public int maxHP;
    public AudioClip DmgSound;
    public AudioSource soundEmitter;
    public System.Action onDeath;
    [SerializeField]
    int HP;
    // Use this for initialization
    void Start()
    {
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Hurt(int dmg) {
        HP -= dmg;
        if (HP <= 0) {
            HP = 0;
            if (onDeath!=null)
                onDeath();
        }
        if (DmgSound)
            if (soundEmitter)
                soundEmitter.PlayOneShot(DmgSound);
    }
    public void SetHP(int HP) {
        this.HP = HP;
        if (HP <= 0)
        {
            HP = 0;
            if (onDeath != null)
                onDeath();
        }
        if (HP > maxHP) HP = maxHP;
    }
    public void Heal(int dmg) {
        HP += dmg;
        if (HP > maxHP) HP = maxHP;
    }
    public int GetHP() {
        return HP;
    }
}
