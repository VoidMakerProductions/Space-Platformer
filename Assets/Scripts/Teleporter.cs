using UnityEngine;
using System.Collections;

public class Teleporter : Interactable
{

    public Transform target;

    public override void Interact()
    {
        insiders.ForEach(x => {
            x.transform.position = target.position;
        });
    }
}
