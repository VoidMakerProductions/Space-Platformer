using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerControl : MonoBehaviour {

    public string playerid;
    public bool fRight, fLeft, fUp, fDown;
    bool fake=false;
    public Vector2 assumedPosition;
    Vector2 assumedVelocity;
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
    public AudioSource emitter;
    public AudioSource emitter2;

    public int CollectedAspirin;


    public float bootConsumptionPerSecond = 1f;
    public float batteryLockTime = 3f;
    float unlock;
    float nextRestore;
    public float energyRestorePerSecond = 2f;
    public int maxEnergy;
    public int energy { get; private set; }
    float nextBoot;
    Interactable interactable;
    Participant myself;
    Vector3 accumulatedMovement;
    enum Dir {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
	// Use this for initialization
	void Start () {
        if (!Syncer.Instance.singlplayer) { myself = PlayGamesPlatform.Instance.RealTime.GetSelf(); }
        accumulatedMovement = transform.position;
        assumedPosition = transform.position;
        if (!Syncer.Instance.singlplayer)
            if (playerid != myself.ParticipantId)
                return;
        Camera.main.GetComponent<CameraFollow>().setTarget(gameObject.transform);
        foreach (Parallax p in Camera.main.GetComponents<Parallax>()) {
            p.SetTarget(this);
        }
        Camera.main.GetComponent<HPBar>().SetTarget(GetComponent<HealthKeeper>());
        Camera.main.GetComponent<HPBar>().SetTarget(this);
        energy = maxEnergy;
    }
   /* public override void OnStartLocalPlayer()
    {
        
    }*/
    // Update is called once per frame
    void Update () {
        //if (!isLocalPlayer) return;
        //self.velocity = Syncer.pixelPerfectVector(self.velocity);
        
        if (!Syncer.Instance.singlplayer)
            if(playerid != myself.ParticipantId) return;

        if (CrossPlatformInputManager.GetButtonDown("Thrust")) {
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {


                if (localGravDir==Dir.LEFT){
                    GetComponent<SpriteRenderer>().flipX = true;
                }if (localGravDir==Dir.RIGHT) {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                Vector2 v = new Vector2(0, thrust);
                if(!fUp) self.AddForce(v);
                Cmd_Poof(-v);
                nt = Time.time + cooldown;
            }
            if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            {
                if (localGravDir == Dir.LEFT)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                if (localGravDir == Dir.RIGHT)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                Vector2 v = new Vector2(0, -thrust);
                if (!fDown)
                {
                    self.AddForce(v);
                }
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
                if (!fRight) { 
                    self.AddForce(v);
                }
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
                if (!fLeft) self.AddForce(v);
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
        if (CrossPlatformInputManager.GetButton("Fire2"))
        {
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

                    ReportAchievmentBoots();
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
                    ReportAchievmentBoots();

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
                    ReportAchievmentBoots();
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

                    ReportAchievmentBoots();
                }
            }
            {
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.up, 0); ;
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
                if (hit2D.collider == null || energy < 1)
                {
                    localGrav.force = Vector2.zero;
                }
                else
                {
                    if (Time.time >= nextBoot)
                    {
                        nextBoot = Time.time + 1f / bootConsumptionPerSecond;
                        SpendEnergy(1,1f);
                    }
                }
            }

        }
        else {
            if (Time.time >= nextRestore && Time.time >= unlock) {
                nextRestore = Time.time + 1f / energyRestorePerSecond;
                energy++;
                if (energy > maxEnergy) {
                    energy = maxEnergy;
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

        if (CrossPlatformInputManager.GetButtonDown("Restart")) {
            if (!Syncer.Instance.singlplayer)
                PlayGamesPlatform.Instance.RealTime.LeaveRoom();
            else
                SceneManager.LoadScene(1);
            //Destroy(gameObject);
        }
        if (!fake)
        {
            assumedVelocity = self.velocity;
            assumedPosition = transform.position;
        }
        else {
            assumedPosition += assumedVelocity*Time.deltaTime;
        }
        
        if (fLeft && self.velocity.x < 0 || fRight && self.velocity.x > 0) {
            fake = true;
            self.velocity =new Vector2(0, self.velocity.y);
        }
        if (fDown && self.velocity.y < 0 || fUp && self.velocity.y > 0) {
            fake = true;
            self.velocity = new Vector2(self.velocity.x, 0);
        }
        if (!fDown && !fUp && !fLeft && !fRight) {
            fake = false;
        }

    }
    private void LateUpdate()
    {
        Vector3 pp = Syncer.pixelPerfectVector((Vector2)transform.position);
        
        accumulatedMovement += (Vector3)(self.velocity*Time.deltaTime);

        if ((transform.position - accumulatedMovement).sqrMagnitude >= (1f / 8f) * (1f / 8f)) {
            transform.position = Syncer.pixelPerfectVector(accumulatedMovement);
        }

        assumedPosition = Syncer.pixelPerfectVector(assumedPosition);
        //self.velocity = Syncer.pixelPerfectVector(self.velocity);
    }

    public void SpendEnergy(int amount = 1,float locktime = 0f) {
        energy -= amount;
        unlock = Time.time + locktime;
        if (energy < 1) {
            energy = 0;
            unlock += batteryLockTime;
        }
    }
    void ReportAchievmentBoots() {
        if (Social.localUser.authenticated) {
            PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_so_there_is_down_in_space,
                100.0, (bool success) =>
                {
                    Debug.Log("(Nebullarix) Boots Unlock: " + success);
                });
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<Interactable>();
        if (collision.gameObject.CompareTag("spacewarp")) {
            Spacewarp spacewarp = collision.GetComponent<Spacewarp>();
            fUp = spacewarp.FakeUp;
            fDown = spacewarp.FakeDown;
            fLeft = spacewarp.FakeLeft;
            fRight = spacewarp.FakeRight;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = null;
        if (collision.gameObject.CompareTag("spacewarp"))
        {
            fUp = false;
            fDown = false;
            fLeft = false;
            fRight = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground")) {
            //Debug.Log("Hit the ground");
            //emitter.Stop();
            emitter2.PlayOneShot(collideSound,1.0f);
            Quaternion rot1 = Quaternion.Euler(0, 0, -90f);
            Quaternion rot = Quaternion.FromToRotation(Vector2.right, collision.GetContact(0).normal);
            rot = rot * rot1;
            float z = Mathf.Round(rot.eulerAngles.z / 45f);
            //Debug.Log(z);
            if (collision.GetContact(0).point.x > transform.position.x) {
                if (z % 2 == 0) {
                    z *= -1f;
                }
                
            }
            rot = Quaternion.Euler(0, 0, z*45f);
            bool diagonal = false;
            if (rot.eulerAngles.z % 90 != 0) {
                rot = rot * Quaternion.Euler(0, 0, -45f);
                diagonal = true;
            }
            Cmd_Dust(collision.GetContact(0).point, rot,diagonal);
        }
        
    }

    //[Command]
    void Cmd_Dust(Vector3 pos, Quaternion rot,bool diagonal) {
        GameObject go = Instantiate(collideEffect, pos, rot);
        go.GetComponent<Animator>().SetBool("diagonal", diagonal);
        //NetworkServer.Spawn(go);
    }

    //[Command]
    void Cmd_Fire() {
        GameObject go = Instantiate(projectile.gameObject,gun.position,Quaternion.Euler(0,0,0));
        //NetworkServer.Spawn(go);
        Vector2 vector2 = (aim.transform.rotation * Vector2.right);
        vector2 = vector2 * blastSquareForce;
        go.GetComponent<Rigidbody2D>().AddForce(vector2);
    }

    //[Command]
    void Cmd_Poof(Vector2 v) {
        GameObject go = Instantiate(effect.gameObject, transform.position+(Vector3)(v.normalized*0.625F), Quaternion.Euler(0, 0, 0));
        //NetworkServer.Spawn(go);
        go.GetComponent<Rigidbody2D>().AddForce(v);
    }

    //[Command]
    void Cmd_Interact() {
        interactable.Interact();
    }

}
