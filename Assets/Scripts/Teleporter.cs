using UnityEngine;
using System.Collections;

public class Teleporter : Interactable
{

    public Transform target;

    public override void Interact(PlayerControl  actor)
    {
        insiders.ForEach(x => {
            x.transform.position = target.position;
            PlayerControl pc = x.GetComponent<PlayerControl>();
            if (pc) {
                pc.isTeleported = true;
                pc.self.velocity = Vector2.zero;
            }
                
        });
    }
}
