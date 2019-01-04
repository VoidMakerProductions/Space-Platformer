using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float lifetime;
    public int damage;
    public string TargetTag;
    public GameObject effect;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag)) {
            HealthKeeper hk = collision.gameObject.GetComponent<HealthKeeper>();
            if (hk) hk.Hurt(damage);
        }
        if (effect) Instantiate(effect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
