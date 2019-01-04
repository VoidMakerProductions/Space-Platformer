using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public int depth = -20;
    public float speed = 0.1f;
    public float SnapTreshold = 10f;
    public bool LockOn = false;
    public float maxWait=1f;
    float pursueTime=0f;
    public float ignoreRadius;
    Rigidbody2D targetRB;
    public float MaxTargetVelocity;
    bool caughtUp;
    // Update is called once per frame
    void LateUpdate()
    {
        if (pursueTime == 0f && targetRB.velocity.sqrMagnitude < (0.25f * 0.25f)) {
            pursueTime = Time.time+maxWait;
            //Debug.Log(pursueTime);
        }
        if (targetRB.velocity.sqrMagnitude > (0.25f * 0.25f)) {
            pursueTime = 0f;
        }
        
        float sqrDist = ((Vector2)playerTransform.position - (Vector2)transform.position).sqrMagnitude;
        if (playerTransform != null)
        {
            if (sqrDist < SnapTreshold && targetRB.velocity.sqrMagnitude < (MaxTargetVelocity * MaxTargetVelocity) && !LockOn && !Ptime)
            {
                
                if (sqrDist > (ignoreRadius * ignoreRadius) && targetRB.velocity.sqrMagnitude < (MaxTargetVelocity * MaxTargetVelocity) )
                {
                    Vector3 intendedPosition = transform.position + (Vector3)((Vector2)(playerTransform.position - transform.position)).normalized * ignoreRadius + new Vector3(0, 0, depth);
                    transform.position = Vector3.Lerp(transform.position, intendedPosition, speed * 4);
                }
                else
                    caughtUp = false;
            }
            else if ((targetRB.velocity.sqrMagnitude >= (MaxTargetVelocity * MaxTargetVelocity) ||Ptime)&& !caughtUp)
            {
                //Debug.Log("Moving Fast or Waiting Long");
                float multiplier = Ptime ? 2f : (targetRB.velocity.sqrMagnitude / (MaxTargetVelocity * MaxTargetVelocity));
                transform.position = Vector3.Lerp(transform.position, playerTransform.position, speed *multiplier ) + new Vector3(0, 0, depth);
                if (sqrDist < (1f))
                {

                    caughtUp = true;
                }
                else
                {
                    caughtUp = false;
                }
            }
            else
            {
                transform.position = playerTransform.position + new Vector3(0, 0, depth);
            }
            
            transform.position = (Vector3)Syncer.pixelPerfectVector((Vector2)transform.position) + new Vector3(0, 0, depth);
            
        }


        
    }
    bool Ptime { get {
            return pursueTime > 0 && Time.time >= pursueTime;
        } }
    public void setTarget(Transform target)
    {
        playerTransform = target;
        targetRB = target.GetComponent<Rigidbody2D>();
    }
}