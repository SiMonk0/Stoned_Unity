﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{

    private BoxCollider2D coll;
    public float gravityScale = 9.81f;
    private Vector3 gravityVector;

    // Use this for initialization
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        gravityVector = -transform.up.normalized * gravityScale;
    }

    void FixedUpdate()
    {
        List<Collider2D> c2ds = new List<Collider2D>();
        c2ds = GameManager.gravityColliderList;
        foreach (Collider2D c2d in c2ds)
        {
            if (coll.OverlapPoint(c2d.bounds.center))
            {
                if ( ! ReferenceEquals(c2d.gameObject, null))
                {
                    Rigidbody2D rb2d = c2d.gameObject.GetComponent<Rigidbody2D>();
                    Vector3 vector = gravityVector * rb2d.mass;
                    rb2d.AddForce(vector);
                }
            }
        }
        //Check to see if the camera rotation needs updated
        if (Camera.main.transform.rotation != transform.rotation)
        {
            //Check to see if Merky is in this GravityZone
            if (coll.OverlapPoint(GameManager.getPlayerObject().GetComponent<PolygonCollider2D>().bounds.center))
            {
                Camera.main.transform.rotation = transform.rotation;
            }
        }
    }
}