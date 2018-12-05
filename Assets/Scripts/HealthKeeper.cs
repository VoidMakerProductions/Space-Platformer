using UnityEngine;
using System.Collections;

public class HealthKeeper : MonoBehaviour
{
    public int maxHP;
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
        if (HP < 0) HP = 0;
    }

    public void Heal(int dmg) {
        HP += dmg;
        if (HP > maxHP) HP = maxHP;
    }
    public int GetHP() {
        return HP;
    }
}
