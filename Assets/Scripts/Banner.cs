using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banner : MonoBehaviour
{
    public GameObject otherGoal;
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerObservable>().laps++;
            otherGoal.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
