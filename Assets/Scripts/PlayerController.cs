using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun
{
    public float speed = 10;
    public GameObject bulletPrefab;
    public Transform weaponLocation;
    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

        transform.Rotate(0, Input.GetAxis("Horizontal"), 0);
        characterController.SimpleMove(transform.forward * Input.GetAxis("Vertical") * speed);

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, weaponLocation.position, Quaternion.identity);
            Destroy(bullet, 4);

            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.VelocityChange);
        }
    }
}