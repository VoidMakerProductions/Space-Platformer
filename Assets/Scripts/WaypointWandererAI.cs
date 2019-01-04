using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody2D))]
public class WaypointWandererAI : MonoBehaviour
{
    Rigidbody2D self;
    public Transform[] waypoints;
    int currentWaypoint = 0;
    public float velocity;
    public float arriveTolerance = 0.5f;
    public float departTime = 0.5f;
    public Rotation rotation;
    float depart = 0;
    public Animator animator;
    public HealthKeeper healthKeeper;
    public enum Rotation {
        None,
        NinetyDegLock,
        Full
    }
    // Use this for initialization
    void Start()
    {
        self = GetComponent<Rigidbody2D>();
        if (healthKeeper) {
            healthKeeper.onDeath += () => {
                Destroy(gameObject);
            };
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (depart <= Time.time) {
            Vector2 dir = waypoints[currentWaypoint].position - transform.position;
            if (dir.magnitude <= arriveTolerance) {
                depart = Time.time + departTime;
                self.velocity = Vector2.zero;
                if (animator) {
                    animator.SetBool("moving",false);
                }
                currentWaypoint++;
                if (currentWaypoint == waypoints.Length) {
                    currentWaypoint = 0;
                }
                return;
            }
            switch (rotation) {
                case Rotation.Full:
                    transform.rotation = Quaternion.FromToRotation(transform.position, waypoints[currentWaypoint].position);
                    break;
                case Rotation.NinetyDegLock:
                    if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
                    {
                        float angle = dir.x > 0 ? -90f : 90f;
                        transform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    else {
                        float angle = dir.y > 0 ? 0f : 180f;
                        transform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    break;
            }
            self.velocity = dir.normalized * velocity;
            if (animator)
            {
                animator.SetBool("moving", true);
            }
        }
    }
}
