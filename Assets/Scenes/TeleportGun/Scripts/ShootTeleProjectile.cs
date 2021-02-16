using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTeleProjectile : MonoBehaviour
{  
    public string[] tags; //a public array of tags that are valid targets for the projectile teleportation, leave blank to make it discrete;

    public int maxRange = 128; //the maximum range visualized by the arc
    int layerMask = 1 << 9;

    public Transform player;
    Transform hand;
    Transform[] tics;

    public Camera firstPersonCamera; //Assign the first 

    public KeyCode shootKey = KeyCode.Mouse0; //Assign in editor what key you use to shoot the projectile.

    public GameObject gunModel; //prefab of gun, spawned at runtime
    public GameObject projectile;
    public GameObject arcTic;

    //Projectile Physics
    public float projectileSpeed = 30;
    public float projectileGravity = 1f;
    public float projectileDestroyTime = 2;
    //Projectile Physics

    public bool ableToShoot = true;

    List<Vector3> trajPoints = new List<Vector3>();
    Vector3 aimDir;
    public Vector3 trajDest; //calculated position of where the projectile will land.

    public GameObject gunCam;
    public GameObject _gunCam;

    void Start()
    {
        //The Camera and Player Transform should be set in the inspector window, if it isn't set, the script will attempt to set the current transform as the Player, and any Camera childed to it as the Camera

        if (firstPersonCamera==null)
        {
            print($"[{this}]: No Camera Assigned, Assigning Camera '{GetComponentInChildren<Camera>().name}' as FPS Camera...");
            firstPersonCamera = this.gameObject.GetComponentInChildren<Camera>(); 
        }
        if (player == null)
        {
            print($"[{this}]: No Player Transform Assigned, Assigning Transform '{GetComponentInChildren<Transform>()}'");
            player = GetComponentInChildren<Transform>();
        }

        //spawn in the 'gun' in the lower right hand side
        GameObject g = Instantiate(gunModel, firstPersonCamera.transform);
        hand = g.transform;

        //this creates the arc visual, showing the trajectory of the projectile
        if (!GameObject.Find("TICS")) { GameObject tics = new GameObject("TICS"); tics.transform.parent = transform; }
        tics = new Transform[maxRange];
        for (int i = 0; i < maxRange; i++) //spawn in trajectory points or 'Tics'
        {
            GameObject t = Instantiate(arcTic, GameObject.Find("TICS").transform);
            tics[i] = t.transform;
        }

        _gunCam = Instantiate(gunCam, firstPersonCamera.transform);

    }

    void Update()
    {
        if (!firstPersonCamera) { print($"No Camera Assigned to [{this}]!"); return; } //print warning if no camera is found

        aimDir = ((firstPersonCamera.transform.position + (firstPersonCamera.transform.forward * 99999)) - hand.transform.position).normalized; // sets the initial trajectory of the projectile
        if (Input.GetKeyDown(shootKey) && ableToShoot) //runs when the assigend 'shoot' key is pressed, be default 'Mouse 0'
        {
            hand.GetComponentInChildren<GunMovement>().Fire();// firing animation function called on gun
            ShootProjectile(projectileSpeed); //spawn projectile, set its initial velocity
        }
    }


    private void LateUpdate()
    {
        if (!firstPersonCamera) { print($"No Camera Assigned to [{this}]!"); return; } //print warning if no camera is found
        ArcVisualizer();
    }

    void ArcVisualizer()
    {
        Vector3 pos = hand.position;
        Vector3 vel = aimDir * projectileSpeed;
        Vector3 grav = new Vector3(0, -projectileGravity, 0) * projectileSpeed;

        trajPoints.Clear();

        Vector3 lastHit = Vector3.zero;
        float lastDist = Mathf.Infinity;

        int ii = 0;

        while (true) //calculate the trajectory based on the projectiles initial velocity
        {
            trajPoints.Add(pos);
            vel = vel + grav * Time.fixedDeltaTime;
            pos = pos + vel * Time.fixedDeltaTime;
            if (ii > 0)
            {
                Debug.DrawLine(trajPoints[ii], trajPoints[ii - 1]);
            }
            if (Physics.Raycast(pos, vel.normalized, out RaycastHit trajHit))
            {
                if (lastDist < trajHit.distance)
                {
                    trajDest = lastHit;
                    break;
                }
                lastHit = trajHit.point;
                lastDist = trajHit.distance;
            }
            else
            {
                if (lastDist != Mathf.Infinity)
                {
                    trajDest = lastHit;
                    break;
                }
            }
            ii++;
            if (ii > maxRange)
            {
                break;
            }
        }

        for (int i = 0; i < maxRange; i++)
        {
            if (i < ii)
            {
                tics[i].position = trajPoints[i];
            }
            tics[i].GetComponent<Renderer>().enabled = i < ii && i>2; // only show points up to where the projectile would land, not through colliders
        }

        iterateTics(ii); // arc visualization, pulse animation
    }

    int currentTic;
    bool waving = true;
    float waveWaitTime = 1f;
    float timeBetweenTics = .05f;

    float ticTimer;
    float ticWaitTimer;

    void iterateTics(int endInt)
    {
        if (waving)
        {
            ticTimer += Time.deltaTime;
            if(ticTimer>=timeBetweenTics)
            {
                if(endInt > 5)
                    tics[currentTic].localScale *= 1.75f;
                currentTic++;
                ticTimer = 0;
            }
            if (currentTic >= endInt-1)
            {
                currentTic = 0;
            }
        }
        else
        {
            ticWaitTimer += Time.deltaTime;
            if (ticWaitTimer>=waveWaitTime) { waving = true; ticWaitTimer = 0; }
        }
    }

    public void ShootProjectile(float spd)
    {
        ableToShoot = false; //makes it so that you can only shoot one projectile at a time;

        GameObject p = Instantiate(projectile, hand.position, Quaternion.identity); //create Projectile;
        p.GetComponent<TeleProjectileScript>().moveDirection = aimDir; //set direction of initial veolcity
        p.GetComponent<TeleProjectileScript>().speed = spd; //set speed of initial velocity
        p.GetComponent<TeleProjectileScript>().player = player; //give reference to this player, so we can reset the 'ableToShoot' boolean from the projectile script upon impact
        p.GetComponent<TeleProjectileScript>().projectileGravity = projectileGravity; //set the gravity magnitude for the projectile
        p.GetComponent<TeleProjectileScript>().projectileDestroyTime = projectileDestroyTime; // if shot off into a void with no colliders, it will be destroyed after a set amount of time.
    }
}
