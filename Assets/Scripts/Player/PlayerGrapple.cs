using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Rigidbody rb;
    Camera cam;

    public LayerMask grappleLayer;
    float grappleDuration;
    float grappleDurationMax = 3;
    bool grappling;
    public float maxGrappleSpeed = 30f;


    RaycastHit hit;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(grappleDuration <= 0)
            {
                if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 30, grappleLayer))
                {
                    lineRenderer.enabled = true;
                    grappleDuration = grappleDurationMax;
                }
            }
            else
            {
                grappleDuration = 0;
            }
        }

        if(grappleDuration > 0 && hit.point != null)
        {
            Vector3 vecToGrapple = hit.point - transform.position;
            if(rb.velocity.magnitude < maxGrappleSpeed)
            {
                rb.AddForce((vecToGrapple) * 0.15f);
            }
            lineRenderer.SetPosition(0, transform.position); //Try not to put on cam directly, it may blind player
            lineRenderer.SetPosition(1, hit.point);
            grappleDuration -= Time.deltaTime;

            
            if(Vector3.Angle(vecToGrapple, cam.transform.forward) > 90) //break off grapple if you are looking perpendicular or greater to the grappple point
            {
                grappleDuration = 0;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
