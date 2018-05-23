using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class PlayerShooting : NetworkBehaviour {

    [SerializeField]
    float shotCooldown = 0.3f;
    [SerializeField]
    int killsToWin = 5;
    [SerializeField]
    Transform firePosition;
    [SerializeField]
    ShotEffectsManager shotEffects;

    [SyncVar (hook = "OnScoreChanged")]
    int score;

    Player player;
    float elapsedTime;
    bool canShoot;
    public LineRenderer lazerLine;


    void Start()
    {
        player = GetComponent<Player>();
        //initialize shoot effects
        shotEffects.Initialize();

        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };

       // lazerLine = new LineRenderer();

        if (lazerLine == null)
            Debug.Log("null");

        lazerLine.SetPositions(initLaserPositions);
        lazerLine.startWidth = 0.01f;
        lazerLine.endWidth = 0.01f;

        if (isLocalPlayer)
            canShoot = true;
    }

    [ServerCallback]
    private void OnEnable()
    {
        score = 0;
    }

    void Update()
    {
        if (!canShoot)
            return;

        elapsedTime += Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && elapsedTime >shotCooldown)
        {
            elapsedTime = 0.0f;
            CmdFireShot(firePosition.position, firePosition.forward);
        }
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            endPosition = raycastHit.point;
        }

        lazerLine.SetPosition(0, targetPosition);
        lazerLine.SetPosition(1, endPosition);
    }

    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        ShootLaserFromTargetPosition(origin, direction, 35.0f);



        bool result = Physics.Raycast(ray, out hit, 50f);

        if(result)
        {
            //HP calc
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if (enemy != null)
            {
                bool wasKillShot = enemy.TakeDamage();

                if (wasKillShot && ++score >= killsToWin)
                    player.Won();
            }
        }

        RpcProcessShotEffects(result, hit.point);
    }

    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        shotEffects.PlayShotEffects();
        //PlayShotEffects
        if(playImpact)
        {
            //Play point
            shotEffects.PlayImpactEffect(point);
        }
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if(isLocalPlayer)
            PlayerCanvas.canvas.SetKills(value);
        
    }

    public void FireAsBot()
    {
        CmdFireShot(firePosition.position, firePosition.forward);
    }

}
