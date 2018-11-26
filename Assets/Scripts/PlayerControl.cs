using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerControl : NetworkBehaviour {


    
    public Rigidbody2D self;
    public float thrust = 0.5f;
    public float cooldown = 0.5f;
    public float FireCooldown = 0.5f;
    public float aimSpeed = 0.4f;
    private float nt=0;
    private float nb=0;
    public Rigidbody2D projectile;
    public Rigidbody2D effect;
    public Transform gun;
    public LineRenderer aim;
    public float blastSquareForce = 5f;
    private float rot = 0;
    public ConstantForce2D localGrav;
    public float localGravPower=9.81f;
    public float localGravRange = 14f;
    private Dir localGravDir = Dir.DOWN;
    public AudioClip collideSound;
    public GameObject collideEffect;
    [SerializeField]public Dictionary<string, AudioClip> collideSounds;
    public AudioSource emitter;
    public AudioSource emitter2;
    Interactable interactable;
    enum Dir {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
	// Use this for initialization
	void Start () {
		
	}
    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraFollow>().setTarget(gameObject.transform);
    }
    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer) return;
        if (Time.time >= nt) {
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {
                if (localGravDir==Dir.LEFT){
                    GetComponent<SpriteRenderer>().flipY = true;
                }if (localGravDir==Dir.RIGHT) {
                    GetComponent<SpriteRenderer>().flipY = false;
                }
                Vector2 v = new Vector2(0, thrust);
                self.AddForce(v);
                Cmd_Poof(-v);
                nt = Time.time + cooldown;
            }
            if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            {
                if (localGravDir == Dir.LEFT)
                {
                    GetComponent<SpriteRenderer>().flipY = false;
                }
                if (localGravDir == Dir.RIGHT)
                {
                    GetComponent<SpriteRenderer>().flipY = true;
                }
                Vector2 v = new Vector2(0, -thrust);
                
                self.AddForce(v);
                Cmd_Poof(-v);
                nt = Time.time + cooldown;
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
            {
                if (localGravDir == Dir.DOWN)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                if (localGravDir == Dir.UP)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }


                Vector2 v = new Vector2(thrust, 0);
                self.AddForce(v);
                Cmd_Poof(-v);
                nt = Time.time + cooldown;
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
            {
                if (localGravDir == Dir.DOWN)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                if (localGravDir == Dir.UP)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                Vector2 v = new Vector2(-thrust, 0);
                self.AddForce(v);
                Cmd_Poof(-v);
                nt = Time.time + cooldown;
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0)
            {
                if (!emitter.isPlaying)
                {
                    emitter.Play();
                }
            }
            else {
                emitter.Stop();
            }
        }
        if (CrossPlatformInputManager.GetButton("Fire1")) {
            aim.enabled = true;
            rot =rot+ CrossPlatformInputManager.GetAxis("Aim") * aimSpeed;
            //Debug.Log(CrossPlatformInputManager.GetAxis("Aim"));
            aim.transform.rotation = Quaternion.Euler(0, 0, rot);
            
        }
        if (Time.time >= nb&&CrossPlatformInputManager.GetButtonUp("Fire1")) {
            aim.enabled = false;
            nb = Time.time + FireCooldown;
            Cmd_Fire();
        }
        if (CrossPlatformInputManager.GetButton("Fire2")) {
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.up, localGravRange);
                transform.rotation = Quaternion.Euler(0, 0, 180);
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().flipY = false;
                localGravDir = Dir.UP;
                if (hit2D.collider)
                {
                    localGrav.force = Vector2.up * localGravPower;
                    
                    
                }
            }
            if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, localGravRange);
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().flipY = false;
                localGravDir = Dir.DOWN;
                if (hit2D.collider)
                {
                    localGrav.force = Vector2.down * localGravPower;
                    
                    
                }
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
            {
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.right, localGravRange);
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().flipY = false;
                localGravDir = Dir.RIGHT;
                if (hit2D.collider)
                {
                    localGrav.force = Vector2.right * localGravPower;
                    
                }
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
            {
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.left, localGravRange);
                bool f = GetComponent<SpriteRenderer>().flipX;
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().flipY = false;
                localGravDir = Dir.LEFT;
                if (hit2D.collider)
                {
                    localGrav.force = Vector2.left * localGravPower;
                    

                }
            }
            {
                RaycastHit2D hit2D= Physics2D.Raycast(transform.position, Vector2.up, 0); ;
                switch (localGravDir)
                {
                    case Dir.UP:
                        hit2D = Physics2D.Raycast(transform.position, Vector2.up, localGravRange);
                        break;
                    case Dir.DOWN:
                        hit2D = Physics2D.Raycast(transform.position, Vector2.down, localGravRange);
                        break;
                    case Dir.LEFT:
                        hit2D = Physics2D.Raycast(transform.position, Vector2.left, localGravRange);
                        break;
                    case Dir.RIGHT:
                        hit2D = Physics2D.Raycast(transform.position, Vector2.right, localGravRange);
                        break;
                }
                if (hit2D.collider == null) {
                    localGrav.force = Vector2.zero;
                }
            }
            
        }
        if (CrossPlatformInputManager.GetButtonUp("Fire2")){
            localGrav.force = Vector2.zero;
        }
        if (CrossPlatformInputManager.GetButtonDown("Fire3")) {
            if (interactable != null) {
                Cmd_Interact();
            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<Interactable>();
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = null;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground")) {
            //Debug.Log("Hit the ground");
            //emitter.Stop();
            emitter2.PlayOneShot(collideSound,1.0f);
            Quaternion rot1 = Quaternion.Euler(0, 0, -90f);
            Quaternion rot = Quaternion.FromToRotation(Vector2.right, collision.GetContact(0).normal);
            
            Cmd_Dust(collision.GetContact(0).point,rot*rot1);
        }
        
    }

    [Command]
    void Cmd_Dust(Vector3 pos, Quaternion rot) {
        GameObject go = Instantiate(collideEffect, pos, rot);
        NetworkServer.Spawn(go);
    }

    [Command]
    void Cmd_Fire() {
        GameObject go = Instantiate(projectile.gameObject,gun.position,Quaternion.Euler(0,0,0));
        NetworkServer.Spawn(go);
        Vector2 vector2 = (aim.transform.rotation * Vector2.right);
        vector2 = vector2 * blastSquareForce;
        go.GetComponent<Rigidbody2D>().AddForce(vector2);
    }

    [Command]
    void Cmd_Poof(Vector2 v) {
        GameObject go = Instantiate(effect.gameObject, transform.position+(Vector3)(v.normalized*0.625F), Quaternion.Euler(0, 0, 0));
        NetworkServer.Spawn(go);
        go.GetComponent<Rigidbody2D>().AddForce(v);
    }

    [Command]
    void Cmd_Interact() {
        interactable.Interact();
    }

}
