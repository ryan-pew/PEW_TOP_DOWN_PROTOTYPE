using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> {}

public class Player : NetworkBehaviour {

    [SyncVar (hook = "OnNameChanged")] public string playerName;
    [SyncVar (hook = "OnColorChanged")] public Color playerColor;


    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 5f;

    static List<Player> players = new List<Player>();

    GameObject mainCamera;
    NetworkAnimator anim;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        anim = GetComponent<NetworkAnimator>();
        EnablePlayer();
    }

    [ServerCallback]
    void OnEnable()
    {
        if (!players.Contains(this))
            players.Add(this);
    }

    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this))
            players.Remove(this);
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.HideReticule();
            mainCamera.SetActive(true);
        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));

    }

    public void Death()
    {
        if(isLocalPlayer || playerControllerId == -1)
            anim.SetTrigger("Died");

        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("YOU DIE!");
            //PlayerCanvas.canvas.PlayDeathAudio();
        }

        DisablePlayer();
        Invoke("Respawn", respawnTime);
    }

    private void Respawn()
    {

        if (isLocalPlayer || playerControllerId == -1)
            anim.SetTrigger("Restart");


        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        EnablePlayer();
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        //Set text
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
       // gameObject.name = playerName;

    }

    public override void OnStartClient()
    {
        OnNameChanged(playerName);
        OnColorChanged(playerColor);
    }

    [Server]
    public void Won()
    {
        //tell other players
        for (int i = 0; i < players.Count; i++)
            players[i].RpcGameOver(netId, name);

        //go back to lobby after 5 seconds
        Invoke("BackToLobby", 5f);
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        DisablePlayer();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(isLocalPlayer)
        {
            //update UI
            if (netId == networkID)
                PlayerCanvas.canvas.WriteGameStatusText("You Won!");
            else
                PlayerCanvas.canvas.WriteGameStatusText("Game Over! \n" + name + "Won" );

        }
    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }
}
