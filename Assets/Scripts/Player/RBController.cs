using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RBController : MonoBehaviour
{
    [Range(0, 20)] public float speed = 1;
    [Range(0, 20)] public float jump = 1;
    public ForceMode forceMode;

    Vector3 inputDirection;
    Vector3 velocity;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        inputDirection = Vector3.zero;
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");

        velocity = inputDirection * speed;
        //transform.position += velocity * Time.deltaTime;

        rb.AddForce(velocity, forceMode);

        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
    }
}
