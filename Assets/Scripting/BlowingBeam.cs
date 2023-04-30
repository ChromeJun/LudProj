using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowingBeam : MonoBehaviour
{
    [SerializeField] Collider coll = null;
    [SerializeField] GameObject[] abductionEffects = null;
    [SerializeField] Transform rayVisualRoot = null;

    [Header("Blow Settings")]
    [SerializeField] float blowMaxForce = 30.0f;
    [SerializeField] float rotationForce = 10.0f;
    [SerializeField] float minForcePercentage = 0.2f;

    List<Abductable> abductables = new List<Abductable>();
    List<Vector3> abductRotations = new List<Vector3>();

    Coroutine faceMouseRoutine = null;
    Coroutine blowRoutine = null;

    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    private void OnEnable()
    {
        DeactivateRay();

        if (faceMouseRoutine != null) StopCoroutine(faceMouseRoutine);
        faceMouseRoutine = StartCoroutine(FaceMouseRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ActivateRay()
    {
        for (int i = 0; i < abductionEffects.Length; i++) abductionEffects[i].SetActive(true);

        if (blowRoutine != null) StopCoroutine(blowRoutine);
        blowRoutine = StartCoroutine(BlowRoutine());
    }

    public void DeactivateRay()
    {
        if (blowRoutine != null) StopCoroutine(blowRoutine);

        for (int i = 0; i < abductionEffects.Length; i++) abductionEffects[i].SetActive(false);
        for (int i = 0; i < abductables.Count; i++) abductables[i].BeginFalling();

        abductables.Clear();
    }

    IEnumerator FaceMouseRoutine()
    {
        while (true)
        {
            yield return null;

            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 directionToCursor = cursorWorldPosition - transform.position;
            if (directionToCursor.sqrMagnitude < Mathf.Epsilon) continue;
            float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;

            // Apply the rotation to the object
            transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
        }
    }

    IEnumerator BlowRoutine()
    {
        float collHeight = coll.bounds.size.y;

        while (true)
        {
            for (int i = 0; i < abductables.Count; i++)
            {
                float currDist = Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f));

                float clampedCurrDist = Mathf.Clamp(currDist, 0.0f, collHeight);

                float minForce = blowMaxForce * minForcePercentage;
                float blowForce = blowMaxForce + (minForce - blowMaxForce) * (clampedCurrDist / collHeight);

                Vector3 directionToTarget = transform.position - abductables[i].transform.position;
                directionToTarget.Normalize();

                abductables[i].AddForce(-directionToTarget, blowForce);

                abductables[i].RgBody.AddRelativeTorque(abductRotations[i] * rotationForce);
            }

            yield return fixedUpdate;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Abductable foundAbductable = other.GetComponent<Abductable>();
        if (foundAbductable == null) return;
        if (abductables.Contains(foundAbductable)) return;

        abductRotations.Add(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
        abductables.Add(foundAbductable);

        if (foundAbductable.IsFalling) foundAbductable.StopFalling();
    }

    private void OnTriggerExit(Collider other)
    {
        Abductable foundAbductable = other.GetComponent<Abductable>();
        if (foundAbductable == null) return;
        if (!abductables.Contains(foundAbductable)) return;

        int index = 0;
        for (int i = 0; i < abductables.Count; i++)
        {
            if (abductables[i] == foundAbductable)
            {
                index = i;
                break;
            }
        }

        abductables.RemoveAt(index);
        abductRotations.RemoveAt(index);

        foundAbductable.BeginFalling();
    }
}
