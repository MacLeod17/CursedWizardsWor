using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrailCollision : MonoBehaviour
{
    [SerializeField]
    ParticleSystem partSystem;

    void Start()
    {
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null)
        {
            Debug.Log("Ground Found");
            var collision = partSystem.collision;
            collision.enabled = true;
            partSystem.collision.AddPlane(ground.transform);
        }
    }
}
