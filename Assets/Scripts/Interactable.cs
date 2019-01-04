using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    public Animator animator;
    protected int players_inside = 0;
    protected List<GameObject> insiders = new List<GameObject>();
    public abstract void Interact(PlayerControl actor);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            players_inside++;
            insiders.Add(collision.gameObject);
            if (animator) {
                animator.SetBool("active", true);
            }
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.GetComponent<PlayerControl>().interactable = this;
        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            players_inside--;
            insiders.Remove(collision.gameObject);
            if (players_inside == 0) {
                if (animator)
                {
                    animator.SetBool("active", false);
                }
            }
            
        }
    }
}
