using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    public eSpace space = eSpace.Object;
    public eMovement movement = eMovement.Tank;

    public Animator animator;
    public GameObject bulletPrefab;
    public Transform weaponLocation;

    [Min(0.1f)] public float speed = 1;
    public float turnRate = 3;
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

    private void Start()
    {
        velocity *= speed;
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (animator.GetBool("Death") || (GameSession.Instance != null && GameSession.Instance.gameWon)) return;

        // ***

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

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, weaponLocation.position, Quaternion.identity);
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
}
