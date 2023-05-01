using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;
    }
}
