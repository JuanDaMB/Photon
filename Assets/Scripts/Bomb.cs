using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool Active = false;
    public bool Exploded = false;
    public float t = 1f;

    public void Init()
    {
        Active = true;
        Exploded = false;
        t = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active && !Exploded) return;
        t -= Time.deltaTime;
        if (!(t < 0f)) return;
        Exploded = true;
        Explode();
    }

    private void Explode()
    {
        foreach (PlayerObservable observable in FindObjectsOfType<PlayerObservable>())
        {
            if (Vector3.Distance(observable.transform.position, transform.position) < 3f)
            {
                observable.TakeDamage();
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
