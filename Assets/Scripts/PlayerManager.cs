using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("The current Health of our player")]
    public float health = 10.0f;

    [Tooltip("The Player's UI GameObject Prefab")]
    public GameObject PlayerUiPrefab;

    public float Health { get => health / maxHealth; }
    private float maxHealth = 10.0f;

    void Awake()
    {
        // #Important 
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized 
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        maxHealth = health * 1.0f;

        // #Critical 
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load. 
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        if (PlayerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUiPrefab);

            _uiGo.GetComponent<PlayerUI>().SetTarget(this);
        }

        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        //health -= Time.deltaTime;
        //if (health <= 0) GameManager.Instance.LeaveRoom();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float)stream.ReceiveNext();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (collision.collider.CompareTag("Bullet"))
        {
            health -= 1;
            if (health <= 0) GameManager.Instance.LeaveRoom();
        }
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    void CalledOnLevelWasLoaded(int level)
    {
        GameObject _uiGo = Instantiate(PlayerUiPrefab);

        _uiGo.GetComponent<PlayerUI>().SetTarget(this);
    }

    public override void OnDisable()
    {
        // Always call the base to remove callbacks 
        base.OnDisable();

        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}