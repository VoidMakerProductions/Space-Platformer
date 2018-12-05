using UnityEngine;
using System.Collections;

public class Shocker : MonoBehaviour
{
    public Animator anim;
    public int damage = 1;
    public float shockCooldown=1f;
    public string TargetTag = "Player";
    float nextShock;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag)) {
            anim.SetBool("active", false);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag)) {
            if (Time.time >= nextShock) {
                HealthKeeper Hk = collision.GetComponent<HealthKeeper>();
                if (Hk != null)
                {
                    nextShock = Time.time + shockCooldown;
                    anim.SetBool("active", true);
                    Hk.Hurt(damage);
                }
                else {
                    anim.SetBool("active", false);
                }
            }
            else
            {
                anim.SetBool("active", false);
            }
        }
        else
        {
            anim.SetBool("active", false);
        }
    }
}
