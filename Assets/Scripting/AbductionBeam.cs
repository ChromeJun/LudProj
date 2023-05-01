using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbductionBeam : MonoBehaviour
{
    [SerializeField] int suckMaxCapacity = 5;
    [SerializeField] PlayerController playerController = null;
    [SerializeField] Collider coll = null;
    [SerializeField] GameObject[] abductionEffects = null;
    [SerializeField] Transform rayVisualRoot = null;

    [Header("Suck Settings")]
    [SerializeField] float suckUpForce = 2.0f;
    [SerializeField] float suckHoverDist = 2.25f;
    [SerializeField] float suckHoverOffsetAllowance = 0.5f;
    [SerializeField] float rotationForce = 10.0f;

    [Header("Cargo Pickup Settings")]
    [SerializeField] float pickedUpDist = 1.0f;
    [SerializeField] float shrinkMinPercentage = 0.1f;

    List<Abductable> abductables = new List<Abductable>();
    List<Direction> suckAbductable = new List<Direction>();
    List<Vector3> abductRotations = new List<Vector3>();
    List<Vector3> abductRelativePos = new List<Vector3>();

    Coroutine suckUpRoutine = null;
    Coroutine faceMouseRoutine = null;

    public int CurrCarryingCapacity { get; private set; } = 0;

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

    IEnumerator FaceMouseRoutine()
    {
        while (true)
        {
            yield return null;

            // Get mouse position in world space
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Make sure the object and mouse are at the same Z coordinate
            mouseWorldPosition.z = transform.position.z;

            // Calculate rotation to face the mouse
            Vector3 directionToMouse = mouseWorldPosition - transform.position;
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, angle + 90.0f);

            // Rotate the object towards the mouse
            transform.rotation = targetRotation;
        }
    }

    IEnumerator SuckUpRoutine()
    {
        while (true)
        {
            for (int i = 0; i < abductables.Count; i++)
            {
                abductables[i].RgBody.AddRelativeTorque(abductRotations[i] * rotationForce);

                if (abductables[i].AbductType == AbductType.Cargo)
                {
                    float startDist = Vector3.Distance(new Vector3(abductables[i].StartSuckPos.x, abductables[i].StartSuckPos.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f));

                    float currDist = Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f));

                    float clampedCurrDist = Mathf.Clamp(currDist, 0.0f, startDist);

                    Vector3 directionToTarget = transform.position - abductables[i].transform.position;
                    directionToTarget.Normalize();

                    float t = (clampedCurrDist - pickedUpDist) / (startDist - pickedUpDist);
                    float sizeValueX = (abductables[i].OriginalScale.x * shrinkMinPercentage) + t * (abductables[i].OriginalScale.x - (abductables[i].OriginalScale.x * shrinkMinPercentage));
                    float sizeValueY = (abductables[i].OriginalScale.y * shrinkMinPercentage) + t * (abductables[i].OriginalScale.y - (abductables[i].OriginalScale.y * shrinkMinPercentage));
                    float sizeValueZ = (abductables[i].OriginalScale.z * shrinkMinPercentage) + t * (abductables[i].OriginalScale.z - (abductables[i].OriginalScale.z * shrinkMinPercentage));

                    Vector3 newScale = new Vector3(sizeValueX, sizeValueY, sizeValueZ);

                    abductables[i].transform.localScale = newScale;

                    if (Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) > pickedUpDist)
                    {
                        abductables[i].AddForce(directionToTarget, suckUpForce);
                    }
                    else
                    {
                        int index = i;

                        Abductable removeAbductable = abductables[i];

                        abductables.RemoveAt(index);
                        suckAbductable.RemoveAt(index);
                        abductRotations.RemoveAt(index);
                        abductRelativePos.RemoveAt(index);

                        playerController.IncreaseCargoCount();

                        removeAbductable.Die();
                    }

                    continue;
                }

                if (suckAbductable[i] == Direction.Up)
                {
                    Vector3 directionToTarget = transform.position - abductables[i].transform.position;
                    directionToTarget.Normalize();

                    if (Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) > suckHoverDist)
                    {
                        abductables[i].AddForce(directionToTarget, suckUpForce);
                    }
                    else
                    {
                        abductRelativePos[i] = abductables[i].transform.localPosition;
                        abductables[i].RgBody.velocity = Vector3.zero;
                        suckAbductable[i] = Direction.Still;
                    }
                }
                else if (suckAbductable[i] == Direction.Down)
                {
                    Vector3 directionToTarget = transform.position - abductables[i].transform.position;
                    directionToTarget.Normalize();

                    if (Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) < suckHoverDist)
                    {
                        abductables[i].AddForce(-directionToTarget, suckUpForce);
                    }
                    else
                    {
                        abductRelativePos[i] = abductables[i].transform.localPosition;
                        abductables[i].RgBody.velocity = Vector3.zero;
                        suckAbductable[i] = Direction.Still;
                    }
                }
                else
                {
                    abductables[i].RgBody.velocity = Vector3.zero;
                    abductables[i].transform.localPosition = abductRelativePos[i];

                    if (Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) > suckHoverDist + suckHoverOffsetAllowance)
                    {
                        suckAbductable[i] = Direction.Up;
                    }
                    else if (Vector3.Distance(new Vector3(abductables[i].transform.position.x, abductables[i].transform.position.y, 0.0f),
                                                     new Vector3(transform.position.x, transform.position.y, 0.0f)) < suckHoverDist - suckHoverOffsetAllowance)
                    {
                        suckAbductable[i] = Direction.Down;
                    }
                }
            }

            yield return fixedUpdate;
        }
    }

    public void ActivateRay()
    {
        CurrCarryingCapacity = 0;

        for (int i = 0; i < abductionEffects.Length; i++) abductionEffects[i].SetActive(true);

        if (suckUpRoutine != null) StopCoroutine(suckUpRoutine);
        suckUpRoutine = StartCoroutine(SuckUpRoutine());
    }

    public void DeactivateRay()
    {
        CurrCarryingCapacity = suckMaxCapacity;

        if (suckUpRoutine != null) StopCoroutine(suckUpRoutine);

        for (int i = 0; i < abductionEffects.Length; i++) abductionEffects[i].SetActive(false);
        for (int i = 0; i < abductables.Count; i++) abductables[i].BeginFalling();

        abductables.Clear();
        suckAbductable.Clear();
        abductRotations.Clear();
        abductRelativePos.Clear();

        CurrCarryingCapacity = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Abductable foundAbductable = other.GetComponent<Abductable>();
        if (foundAbductable == null) return;
        if (abductables.Contains(foundAbductable)) return;
        if (foundAbductable.SizeCapacity + CurrCarryingCapacity > suckMaxCapacity) return;

        foundAbductable.StopFalling();

        CurrCarryingCapacity += foundAbductable.SizeCapacity;

        foundAbductable.transform.parent = transform;

        if (Vector3.Distance(new Vector3(foundAbductable.transform.position.x, foundAbductable.transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) > suckHoverDist + suckHoverOffsetAllowance)
        {
            suckAbductable.Add(Direction.Up);
        }
        else if (Vector3.Distance(new Vector3(foundAbductable.transform.position.x, foundAbductable.transform.position.y, 0.0f),
                                         new Vector3(transform.position.x, transform.position.y, 0.0f)) < suckHoverDist - suckHoverOffsetAllowance)
        {
            suckAbductable.Add(Direction.Down);
        }
        else suckAbductable.Add(Direction.Still);

        abductRotations.Add(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
        abductRelativePos.Add(foundAbductable.transform.localPosition);

        foundAbductable.RgBody.velocity = Vector3.zero;
        foundAbductable.RgBody.angularVelocity = Vector3.zero;

        abductables.Add(foundAbductable);
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
        suckAbductable.RemoveAt(index);
        abductRotations.RemoveAt(index);
        abductRelativePos.RemoveAt(index);

        foundAbductable.BeginFalling();
    }
}
