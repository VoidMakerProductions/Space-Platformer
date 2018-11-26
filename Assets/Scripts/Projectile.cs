using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float lifetime;
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
        Destroy(gameObject);
    }
}
