using UnityEngine;
using System.Collections;
using System;


[RequireComponent(typeof(SpriteRenderer))]
public abstract class Collectible : MonoBehaviour
{
    
    protected float destroyDelay;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    protected abstract void OnCollect(PlayerControl collector);
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            OnCollect(collision.GetComponent<PlayerControl>());
            Destroy(gameObject, destroyDelay);
            GetComponent<SpriteRenderer>().enabled = false;
            enabled = false;
        }
        
    }
}
