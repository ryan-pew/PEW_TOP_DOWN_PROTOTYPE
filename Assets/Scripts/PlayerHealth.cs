using UnityEngine.Networking;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField] int maxHealth = 3;

    [SyncVar (hook = "OnHealthChanged")]
    int health;

    Player player;

    void Awake()
    {
        player = GetComponent<Player>();    
    }

    [ServerCallback]
    void Start()
    {
        health = maxHealth;
    }

    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;    
    }

    [Server]
    public bool TakeDamage()
    {
        bool died = false;
        if(health <= 0)
            return died;

        health--;
        died = health <= 0;
        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if(isLocalPlayer)
            PlayerCanvas.canvas.FlashDamageEffect();

        if (died)
            player.Death();
    }

    void OnHealthChanged(int value)
    {
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetHealth(value);
    }
}
