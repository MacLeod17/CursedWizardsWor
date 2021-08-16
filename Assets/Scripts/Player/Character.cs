using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviourPun
{
    public eSpace space = eSpace.Object;
    public eMovement movement = eMovement.Tank;

    public Animator animator;
    public GameObject bulletPrefab;
    public Transform weaponLocation;

    public TMP_Text MyScoreText;
    public TMP_Text OtherScoreText;

    public int score = 0;
    [Min(0.1f)] public float speed = 1;
    public float turnRate = 3;
    [Tooltip("This number of seconds must pass between bullet shots.")]
    public float fireRate = 3;
    public bool isDead = false;

    public enum eSpace
    {
        World,
        Camera,
        Object
    }

    public enum eMovement
    {
        Free,
        Tank,
        Strafe
    }

    CharacterController characterController;
    Rigidbody rb;

    Vector3 inputDirection = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Transform cameraTransform;

    float sinceLastBullet = 0;

    private void Start()
    {
        velocity *= speed;
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;

        MyScoreText = GameObject.Find("MyScoreText").GetComponent<TMP_Text>();
        OtherScoreText = GameObject.Find("OtherScoreText").GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (animator.GetBool("Death") || (GameSession.Instance != null && GameSession.Instance.gameWon)) return;

        // ***
        if (MyScoreText != null && OtherScoreText != null) UpdateScores();
        sinceLastBullet -= Time.deltaTime;

        Quaternion orientation = Quaternion.identity;
        switch (space)
        {
            case eSpace.World:
                break;
            case eSpace.Camera:
                Vector3 forward = cameraTransform.forward;
                forward.y = 0;
                orientation = Quaternion.LookRotation(forward);
                break;
            case eSpace.Object:
                orientation = transform.rotation;
                break;
            default:
                break;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, (-90 * turnRate * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, (90 * turnRate * Time.deltaTime));
        }

        Vector3 direction = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        switch (movement)
        {
            case eMovement.Free:
                direction = orientation * inputDirection;
                rotation = (direction.sqrMagnitude > 0) ? Quaternion.LookRotation(direction) : transform.rotation;
                break;
            case eMovement.Tank:
                direction.z = inputDirection.z;
                direction = orientation * direction;

                rotation = orientation * Quaternion.AngleAxis(inputDirection.x, Vector3.up);
                break;
            case eMovement.Strafe:
                direction = orientation * inputDirection;
                rotation = Quaternion.LookRotation(orientation * Vector3.forward);
                break;
            default:
                break;
        }

        // ***
        inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputDirection = Vector3.forward;
            characterController.Move(direction * speed * Time.deltaTime);
            characterController.Move(velocity * Time.deltaTime);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, turnRate * Time.deltaTime);

        // Animator
        animator.SetFloat("Speed", inputDirection.magnitude);

        if (Input.GetButtonDown("Fire1") && sinceLastBullet <= 0)
        {
            sinceLastBullet = fireRate;

            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, weaponLocation.position, Quaternion.identity);
            Bullet b = bullet.GetComponent<Bullet>();
            b.parent = this.gameObject;
            Destroy(bullet, 4);

            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ghost"))
        {
            OnDeath();
        }

        if (other.gameObject.CompareTag("Win"))
        {
            inputDirection = Vector3.zero;
            velocity = Vector3.zero;
            animator.SetFloat("Speed", inputDirection.magnitude);

            GameSession.Instance.gameWon = true;
        }
    }

    public void OnDeath()
    {
        inputDirection = Vector3.zero;
        velocity = Vector3.zero;
        animator.SetFloat("Speed", inputDirection.magnitude);

        isDead = true;
        animator.SetBool("Death", isDead);
    }

    public void UpdateScores()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (PhotonView.Get(player).IsMine)
            {
                MyScoreText.text = score.ToString("0000");
                continue;
            }
            else if (!PhotonView.Get(player).IsMine)
            {
                Character c = player.GetComponent<Character>();
                OtherScoreText.text = c.score.ToString("0000");
                continue;
            }
        }
    }
}
