using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] DeathType defaultDeathType = DeathType.Debris;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            FindObjectOfType<Typewriter>().SetDeathType(defaultDeathType);
            FindObjectOfType<DeathOverlay>().ShowCanvas();
        }

        Abductable foundAbductable = other.GetComponentInParent<Abductable>();
        if (foundAbductable != null)
        {
            if (foundAbductable.AbductType != AbductType.Cargo) return;

            FindObjectOfType<Typewriter>().SetDeathType(DeathType.Cargo);
            FindObjectOfType<DeathOverlay>().ShowCanvas();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerController>() != null)
        {
            FindObjectOfType<Typewriter>().SetDeathType(defaultDeathType);
            FindObjectOfType<DeathOverlay>().ShowCanvas();
        }

        Abductable foundAbductable = collision.gameObject.GetComponentInParent<Abductable>();
        if (foundAbductable != null)
        {
            if (foundAbductable.AbductType != AbductType.Cargo) return;

            FindObjectOfType<Typewriter>().SetDeathType(DeathType.Cargo);
            FindObjectOfType<DeathOverlay>().ShowCanvas();
        }
    }
}
