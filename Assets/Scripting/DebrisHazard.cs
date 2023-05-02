using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisHazard : Hazard
{
    Rigidbody rgBody = null;

    private void Awake()
    {
        if (rgBody == null) rgBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        if (transform.position.y < -8.4f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (rgBody.velocity.y > -0.1f && rgBody.velocity.y < 0.1f &&
            rgBody.velocity.x > -0.1f && rgBody.velocity.x < 0.1f)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
