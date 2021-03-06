﻿using UnityEngine;
using System.Collections;

public class SecretAreaTrigger : MonoBehaviour
{

    public GameObject secretHider;

    // Use this for initialization
    void Start()
    {
        if (secretHider == null)//if the secret hider isn't set, then it's probably its parent
        {
            secretHider = transform.parent.gameObject;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == GameManager.playerTag)
        {
            if (secretHider != null && !ReferenceEquals(secretHider, null))
            {
                HiddenArea ha;
                ha = secretHider.GetComponent<HiddenArea>();
                ha.nowDiscovered();
            }
        }
    }
}