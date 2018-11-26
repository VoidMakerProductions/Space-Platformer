using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : NetworkBehaviour
{
    public Animator animator;
    protected int players_inside = 0;
    public abstract void Interact();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            players_inside++;
            if (animator) {
                animator.SetBool("active", true);
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            players_inside--;
            if (players_inside == 0) {
                if (animator)
                {
                    animator.SetBool("active", false);
                }
            }
            
        }
    }
}
