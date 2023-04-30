using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [ContextMenuItem("Display Borders", "DisplayBorders")]

    [Header("BoundarySettings")]
    [SerializeField] Vector2 startTrackOffsets = new Vector2(6.0f, 10.0f);
    [SerializeField] Vector2 stopTrackOffsets = new Vector2(9.0f, 10.0f);
    [SerializeField] Vector2 stickTrackOffsets = new Vector2(1.0f, 1.0f);
    [SerializeField] Vector2 horizontalLimits = new Vector2(-20.0f, 20.0f);
    [SerializeField] Vector2 verticalLimits = new Vector2(5.0f, 100.0f);

    [Header("Movement Settings")]
    [SerializeField] float horizontalForce = 15.0f;
    [SerializeField] float verticalForce = 15.0f;
    [SerializeField] float stickHorizontalForce = 17.0f;
    [SerializeField] float stickVerticalForce = 17.0f;
    [SerializeField] float reverseHorizontalForce = 30.0f;
    [SerializeField] float reverseVerticalForce = 30.0f;

    [Header("Border Markers - FOR DEBUGGING")]
    [SerializeField] Transform[] rightMarkers = null;
    [SerializeField] Transform[] leftMarkers = null;
    [SerializeField] Transform[] topMarkers = null;
    [SerializeField] Transform[] bottomMarkers = null;

    Camera cam = null;
    Rigidbody rgBody = null;
    PlayerController playerController = null;

    Coroutine trackPlayerRoutine = null;
    Coroutine followPlayerRoutine = null;
    Coroutine clampViewRoutine = null;

    WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

    private void Awake()
    {
        cam = GetComponent<Camera>();
        rgBody = GetComponent<Rigidbody>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);
        trackPlayerRoutine = StartCoroutine(TrackPlayerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void DisplayBorders()
    {
        cam = GetComponent<Camera>();

        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));

        rightMarkers[0].position = new Vector3(topRight.x, cam.transform.position.y, topRight.z + 1.0f);
        rightMarkers[1].position = new Vector3(topRight.x - startTrackOffsets.x, cam.transform.position.y, topRight.z + 1.0f);
        rightMarkers[2].position = new Vector3(topRight.x - stopTrackOffsets.x, cam.transform.position.y, topRight.z + 1.0f);
        rightMarkers[3].position = new Vector3(topRight.x - stickTrackOffsets.x, cam.transform.position.y, topRight.z + 1.0f);

        leftMarkers[0].position = new Vector3(topLeft.x, cam.transform.position.y, topLeft.z + 1.0f);
        leftMarkers[1].position = new Vector3(topLeft.x + startTrackOffsets.x, cam.transform.position.y, topLeft.z + 1.0f);
        leftMarkers[2].position = new Vector3(topLeft.x + stopTrackOffsets.x, cam.transform.position.y, topLeft.z + 1.0f);
        leftMarkers[3].position = new Vector3(topLeft.x + stickTrackOffsets.x, cam.transform.position.y, topLeft.z + 1.0f);

        topMarkers[0].position = new Vector3(cam.transform.position.x, topLeft.y, topRight.z + 1.0f);
        topMarkers[1].position = new Vector3(cam.transform.position.x, topLeft.y - startTrackOffsets.y, topRight.z + 1.0f);
        topMarkers[2].position = new Vector3(cam.transform.position.x, topLeft.y - stopTrackOffsets.y, topRight.z + 1.0f);
        topMarkers[3].position = new Vector3(cam.transform.position.x, topLeft.y - stickTrackOffsets.y, topRight.z + 1.0f);

        bottomMarkers[0].position = new Vector3(cam.transform.position.x, bottomLeft.y, topRight.z + 1.0f);
        bottomMarkers[1].position = new Vector3(cam.transform.position.x, bottomLeft.y + startTrackOffsets.y, topRight.z + 1.0f);
        bottomMarkers[2].position = new Vector3(cam.transform.position.x, bottomLeft.y + stopTrackOffsets.y, topRight.z + 1.0f);
        bottomMarkers[3].position = new Vector3(cam.transform.position.x, bottomLeft.y + stickTrackOffsets.y, topRight.z + 1.0f);
    }

    void StartFollowingPlayer()
    {
        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);

        if (followPlayerRoutine != null) StopCoroutine(followPlayerRoutine);
        followPlayerRoutine = StartCoroutine(FollowPlayerRoutine());

        if (clampViewRoutine != null) StopCoroutine(clampViewRoutine);
        clampViewRoutine = StartCoroutine(ClampViewRoutine());
    }

    void StopFollowingPlayer(bool startTrackingPlayer = true)
    {
        if (followPlayerRoutine != null) StopCoroutine(followPlayerRoutine);

        if (!startTrackingPlayer) return;

        if (trackPlayerRoutine != null) StopCoroutine(trackPlayerRoutine);
        trackPlayerRoutine = StartCoroutine(TrackPlayerRoutine());
    }

    IEnumerator ClampViewRoutine()
    {
        while (true)
        {
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, horizontalLimits.x, horizontalLimits.y),
                                                    Mathf.Clamp(cam.transform.position.y, verticalLimits.x, verticalLimits.y), cam.transform.position.z);

            yield return fixedUpdate;
        }
    }

    IEnumerator TrackPlayerRoutine()
    {
        while (true)
        {
            yield return null;

            Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));

            if (playerController.transform.position.x > topRight.x - startTrackOffsets.x || playerController.transform.position.x < topLeft.x + startTrackOffsets.x ||
                playerController.transform.position.y > topLeft.y - startTrackOffsets.y || playerController.transform.position.y < bottomLeft.y + startTrackOffsets.y)
            {
                /*
                if (cam.transform.position.x - 0.5f <= horizontalLimits.x && playerController.transform.position.x < topLeft.x + startTrackOffsets.x)
                {
                    continue;
                }
                if (cam.transform.position.x + 0.5f >= horizontalLimits.y && playerController.transform.position.x > topRight.x - startTrackOffsets.x)
                {
                    continue;
                }
                if (cam.transform.position.y - 0.5f <= verticalLimits.x && playerController.transform.position.y < bottomLeft.y + startTrackOffsets.y)
                {
                    continue;
                }
                if (cam.transform.position.y + 0.5f >= verticalLimits.y && playerController.transform.position.y > topLeft.y - startTrackOffsets.y)
                {
                    continue;
                }
                */
                break;
            }
        }

        StartFollowingPlayer();
    }

    IEnumerator FollowPlayerRoutine()
    {
        bool isMovingLeft = false;
        bool IsMovingRight = false;
        bool IsMovingUp = false;
        bool IsMovingDown = false;

        while (true)
        {
            Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
            Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
            Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));

            if (playerController.transform.position.x < topLeft.x + startTrackOffsets.x && !isMovingLeft) isMovingLeft = true;
            if (playerController.transform.position.x > topRight.x - startTrackOffsets.x && !IsMovingRight) IsMovingRight = true;
            if (playerController.transform.position.y < bottomLeft.y + startTrackOffsets.y && !IsMovingDown) IsMovingDown = true;
            if (playerController.transform.position.y > topLeft.y - startTrackOffsets.y && !IsMovingUp) IsMovingUp = true;

            if (isMovingLeft)
            {
                if (playerController.transform.position.x <= topLeft.x + stickTrackOffsets.x)
                    rgBody.AddForce(Vector3.left * stickHorizontalForce);
                else
                    rgBody.AddForce(Vector3.left * horizontalForce);

                if (playerController.transform.position.x >= topLeft.x + stopTrackOffsets.x)
                {
                    rgBody.AddForce(Vector3.right * reverseHorizontalForce);
                    isMovingLeft = false;
                }
            }
            
            if (IsMovingRight)
            {
                if (playerController.transform.position.x >= topRight.x - stickTrackOffsets.x)
                    rgBody.AddForce(Vector3.right * stickHorizontalForce);
                else
                    rgBody.AddForce(Vector3.right * horizontalForce);

                if (playerController.transform.position.x <= topRight.x - stopTrackOffsets.x)
                {
                    rgBody.AddForce(Vector3.left * reverseHorizontalForce);
                    IsMovingRight = false;
                }
            }

            if (IsMovingDown)
            {
                if (playerController.transform.position.y <= bottomLeft.y + stickTrackOffsets.y)
                    rgBody.AddForce(Vector3.down * stickVerticalForce);
                else
                    rgBody.AddForce(Vector3.down * verticalForce);

                if (playerController.transform.position.y >= bottomLeft.y + stopTrackOffsets.y)
                {
                    rgBody.AddForce(Vector3.up * reverseVerticalForce);
                    IsMovingDown = false;
                }
            }

            if (IsMovingUp)
            {
                if (playerController.transform.position.y >= topRight.y - stickTrackOffsets.y)
                    rgBody.AddForce(Vector3.up * stickVerticalForce);
                else
                    rgBody.AddForce(Vector3.up * verticalForce);

                if (playerController.transform.position.y <= topRight.y - stopTrackOffsets.y)
                {
                    rgBody.AddForce(Vector3.down * reverseVerticalForce);
                    IsMovingUp = false;
                }
            }


            if (!isMovingLeft && !IsMovingRight && !IsMovingUp && !IsMovingDown) break;

            yield return fixedUpdate;
        }

        StopFollowingPlayer();
    }

}
