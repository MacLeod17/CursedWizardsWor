using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Range(0, 2)] public float sensitivity = 1;
    [Range(1, 5)] public float speed = 2;
    public GameObject hitMarker;
    public GameObject explosion;

    void Update()
    {
        // Rotation
        if (Input.GetMouseButton(1))
        {
            Vector3 rotate = Vector3.zero;
            rotate.y = Input.GetAxis("Mouse X");
            rotate.x = Input.GetAxis("Mouse Y");

            transform.eulerAngles += rotate * sensitivity;

            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }

        // Quaternion
        //Vector3 dir = transform.rotation * Vector3.right;
        //Debug.DrawRay(transform.position, dir * 100, Color.red);

        // Translate
        Vector3 translate = Vector3.zero;
        translate.x = Input.GetAxis("Horizontal");
        translate.z = Input.GetAxis("Vertical");

        transform.position += transform.rotation * translate * speed * Time.deltaTime;

        //// Ray Cast
        //Ray ray = new Ray(transform.position, transform.forward);
        //if (Physics.Raycast(ray, out RaycastHit hitInfo))
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        GameObject gameObject = Instantiate(explosion, hitInfo.point, Quaternion.identity);
        //        Destroy(gameObject, 3);
        //    }

        //    //hitMarker.transform.position = hitInfo.point;
        //    //Debug.Log(hitInfo.collider.gameObject);
        //}
    }
}
