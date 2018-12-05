using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public int depth = -20;
    public float speed = 0.1f;
    public float SnapTreshold = 10f;
    public float ignoreRadius;
    Rigidbody2D targetRB;
    public float MaxTargetVelocity;
    bool caughtUp;
    // Update is called once per frame
    void Update()
    {
        
        if (playerTransform != null)
        {
            if (((Vector2)playerTransform.position - (Vector2)transform.position).sqrMagnitude < SnapTreshold && targetRB.velocity.sqrMagnitude < (MaxTargetVelocity * MaxTargetVelocity))
            {
                if (((Vector2)playerTransform.position - (Vector2)transform.position).sqrMagnitude > (ignoreRadius * ignoreRadius))
                    
                    transform.position = Vector3.Lerp(transform.position, playerTransform.position, speed) + new Vector3(0, 0, depth);
            } else if (targetRB.velocity.sqrMagnitude >= (MaxTargetVelocity * MaxTargetVelocity)&&!caughtUp) {
                
                transform.position = Vector3.Lerp(transform.position, playerTransform.position, speed) + new Vector3(0, 0, depth);
                if (((Vector2)playerTransform.position - (Vector2)transform.position).sqrMagnitude < (0.125*0.125))
                {

                    caughtUp = true;
                }
                else {
                    caughtUp = false;
                }
            }
            else {
                transform.position = playerTransform.position + new Vector3(0, 0, depth);
            }
            
            transform.position = (Vector3)Syncer.pixelPerfectVector((Vector2)transform.position) + new Vector3(0, 0, depth);
            
        }


        
    }

    public void setTarget(Transform target)
    {
        playerTransform = target;
        targetRB = target.GetComponent<Rigidbody2D>();
    }
}